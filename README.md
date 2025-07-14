# Checkers – Showcase Platform
Een fullstack webapplicatie ontwikkeld in het kader van het semester Web Development (Windesheim). Het platform stelt gebruikers in staat om online damspellen te spelen, prestaties bij te houden en beheeracties uit te voeren.

## Inhoud
- [Functionaliteit](#functionaliteit)
- [Architectuur](#architectuur)
- [Gebruikte technologieën](#gebruikte-technologieën)
- [Systeemrollen](#systeemrollen)
- [Installatie](#installatie)
- [Security](#security)
- [API-documentatie](#api-documentatie)

---

## Functionaliteit
Gebruikers kunnen:
- Een account aanmaken en inloggen met 2FA-beveiliging
- Een damspel starten of deelnemen via een unieke code
- Realtime damspel spelen tegen een tegenstander
- Spelgeschiedenis bekijken met resultaten (winst/verlies/gelijkspel)
- Beheerders kunnen inactieve spellen annuleren

Gebruikte communicatieprotocollen:
- HTTP voor reguliere interacties
- WebSocket voor realtime spelcommunicatie

---

## Architectuur
Het platform bestaat uit drie hoofdcomponenten:

- **Client (Next.js)**: UI gebouwd met React en TailwindCSS
- **Server (ASP.NET Core Web API)**: REST API en SignalR Hubs
- **Database (SQL Server)**: Opslag van gebruikers, spellen, zetten en sessies

Deployment gebeurt via Docker en is getest op Skylab met gebruik van Cloudflare voor DNS en HTTPS.

---

## Gebruikte technologieën
### Client
- **Next.js 14.1.0**
- TailwindCSS, Flowbite, FontAwesome
- js-cookie voor cookiebeheer

### Server (.NET 8)
- Entity Framework Core
- JWT-authenticatie + 2FA via `TwoFactorAuth.Net`
- Serilog voor logging
- Swagger (via Swashbuckle) voor API-documentatie

### Database
- SQL Server (relationeel)

---

## Systeemrollen
| Rol        | Beschrijving |
|------------|-------------|
| **Uitdager** | Kan een nieuw damspel starten en delen via code |
| **Damspeler** | Kan deelnemen aan een bestaand spel met code |
| **Beheerder** | Kan inactieve spellen annuleren na 7 dagen |

---

## Installatie
1. Clone deze repository:
   ```bash
   git clone https://github.com/gebruikersnaam/checkers-showcase.git
   cd checkers-showcase
    ```

2. Start het project met Docker Compose:
   ```bash
   docker-compose up --build
   ```

3. Bezoek de applicatie via:
   * Frontend: [http://localhost:3000](http://localhost:3000)
   * API/Swagger: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

Zorg dat `.env`-bestanden voor zowel client als server correct zijn ingevuld met:
* Connection string
* JWT-secret
* 2FA instellingen

---

## Security
Het platform implementeert meerdere beveiligingsmaatregelen:

* Inputvalidatie op client & server
* Prepared SQL-statements tegen injecties
* JWT authenticatie en 2FA voor gebruikers
* Rolgebaseerde toegang voor beheerfuncties

---

## API-documentatie
De volledige API is te raadplegen via Swagger:
👉 [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

---

## Auteur

* Martijn Schuman

```
