using CargoPayAPI.Controllers;
using CargoPayAPI.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CargoPayAPITest.Controller
{
    public class AuthControllerTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;
        public AuthControllerTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController( _authServiceMock.Object );
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new LoginRequest { Username = "admin", Password = "1234" };
            var expectedToken = "fake-jwt-token";

            _authServiceMock.Setup(service => service.Authenticate(request.Username, request.Password))
                            .Returns(expectedToken);

            // Act
            var result = _authController.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);            
            var response = okResult.Value.GetType().GetProperty("Token").GetValue(okResult.Value, null);

            Assert.NotNull(response);
            Assert.Equal(expectedToken, response);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequest { Username = "wronguser", Password = "wrongpassword" };

            _authServiceMock.Setup(service => service.Authenticate(request.Username, request.Password))
                            .Returns<string>(null);

            // Act
            var result = _authController.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }
    }
}
