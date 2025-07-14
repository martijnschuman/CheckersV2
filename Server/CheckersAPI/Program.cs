using CheckersAPI.Controllers;
using CheckersAPI.Data;
using CheckersAPI.Hubs;
using CheckersAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

// Load ENV variabels
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// For ENV
ConfigurationManager configuration = builder.Configuration;

// For Entity Framework
// builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// For Identity
builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// For CORS protection
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options => {
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder => {
                          builder
                            .WithOrigins("http://localhost:3000") // specifying the allowed origin
                            .WithOrigins("https://se-showcase.vercel.app") // specifying the allowed origin
                            .WithOrigins("https://dev.martijnschuman.nl") // specifying the allowed origin
                            .WithOrigins("https://martijnschuman.nl") // specifying the allowed origin
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithExposedHeaders("content-disposition");
                      });
});

// Adding Authentication
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => {
    // add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {securityScheme, new string[] { }}
    });
});

builder.Services.AddAuthentication();

// Adds a HTTPClient for IHttpClientFactory 
builder.Services.AddHttpClient();

builder.Services.AddSignalR();
builder.Services.AddScoped<DataContext>();
builder.Services.AddScoped<CheckersController>();

// ConfigureServices method
var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

// Enabled CORS protection
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

// Configure method
app.MapHub<CheckersHub>("/checkersHub");

// Creates a new logger configuration and writes the logs to the console and a file
Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/AccessLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();


using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}

app.Run();
