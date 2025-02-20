using CargoPayAPI.Data;
using CargoPayAPI.Model;
using CargoPayAPI.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace CargoPayAPITest.Service
{
    public class AuthServiceTest
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;
        private readonly AppDbContext _dbContext;

        public AuthServiceTest()
        {            
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(config => config["Jwt:Key"]).Returns("SuperSecretKeyThatIsAtLeast32CharsLong");
            _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("testIssuer");
            _configurationMock.Setup(config => config["Jwt:Audience"]).Returns("testAudience");
                        
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new AppDbContext(options);
            
            var testUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234") 
            };
            _dbContext.Users.Add(testUser);
            _dbContext.SaveChanges();

            _authService = new AuthService(_configurationMock.Object, _dbContext);
        }

        [Fact]
        public void Authenticate_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var username = "admin";
            var password = "1234";

            // Act
            var token = _authService.Authenticate(username, password);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
          
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configurationMock.Object["Jwt:Key"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configurationMock.Object["Jwt:Issuer"],
                ValidAudience = _configurationMock.Object["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var claims = principal.Claims.ToList();

            Assert.Contains(claims, c => c.Type == ClaimTypes.Name && c.Value == username);
            Assert.Contains(claims, c => c.Type == ClaimTypes.NameIdentifier && Guid.TryParse(c.Value, out _));
        }

        [Fact]
        public void Authenticate_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var username = "admin";
            var password = "wrongpassword";

            // Act
            var token = _authService.Authenticate(username, password);

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public void Authenticate_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var username = "nonexistent";
            var password = "password";

            // Act
            var token = _authService.Authenticate(username, password);

            // Assert
            Assert.Null(token);
        }
    }
}
