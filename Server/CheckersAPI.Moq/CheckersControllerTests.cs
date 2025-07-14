using CheckersAPI.Controllers;
using CheckersAPI.Data;
using CheckersAPI.Models;
using CheckersAPI.Tests;

namespace CheckersAPI.Moq {
    public class CheckersControllerTests {
        private readonly TestHelper _helper;
        private readonly DataContext dataContext;
        private UserModel _user;

        public CheckersControllerTests(AuthenticateController authenticateController) {
            _helper = new TestHelper(authenticateController);
            dataContext = _helper.dataContext;

            _helper.InserUser();
            //_user = _helper.LoginUser("TestUser", "Ontwikkeling123@");
        }


        [Fact]
        public async Task CreateNewCheckersGame_CreateValid() {
            var user = dataContext.Users.First();

            Assert.Equal("TestUser", user.UserName);
        }
    }
}