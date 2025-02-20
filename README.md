CargoPayAPISolution
CargoPayAPI is a RESTful API developed in C# that allows managing cards, 
processing payments, and calculating dynamic fees. It is designed to be secure, scalable, and easy to use.

Key Features:
JWT Authentication: Token-based security to protect endpoints.

Card Management:
Create cards with unique 15-digit numbers.
Check the balance of a card.
Process payments with a dynamic fee applied.

Dynamic Fees:
The payment fee changes every 24 hours (simulating the UFE service).
The fee is calculated by multiplying the previous fee by a random number between 0 and 2.

Database: Uses SQL Server with Entity Framework Core for data persistence.

Requirements
.NET 6 SDK
SQL Server
Visual Studio 2022

SETUP:
Connection String:
Open the appsettings.json file and configure the connection string to your SQL Server instance:

"ConnectionStrings": {
  "Sql": "Server=YOUR_SERVER;Database=CargoPayDb;Integrated Security=True;TrustServerCertificate=True"
}

JWT Configuration:
In the same file (appsettings.json), configure the JWT values:

"Jwt": {
  "Key": "YourSuperSecureSecretKey",
  "Issuer": "YourIssuer",
  "Audience": "YourAudience"
}

Migrations:
Run migrations to create the database:
bash:
dotnet ef database update

Run the API:
Start the API with the following command:
dotnet run

ENDPONTS
Authentication
Login:
POST /api/auth/login
Body:
{
  "username": "admin",
  "password": "1234"
}
Response:
{
  "token": "YourJWTToken"
}

Card Management
Create Card:
POST /api/card/create
Response:
{
  "id": "Guid",
  "cardNumber": "123456789012345",
  "balance": 1000.00
}

Check Balance:
GET /api/card/balance?cardNumber=123456789012345

Response:
1000.00

Process Payment:

POST /api/card/pay
Body:
{
  "cardNumber": "123456789012345",
  "amount": 100.00
}
Response:
"Payment successful"
