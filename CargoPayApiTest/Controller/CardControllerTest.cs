using CargoPayAPI.Controllers;
using CargoPayAPI.Model;
using CargoPayAPI.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CargoPayAPITest.Controller
{
    public  class CardControllerTest
    {
        private readonly Mock<ICardService> _cardServiceMock;
        private readonly CardController _cardController;
        public CardControllerTest()
        {
            _cardServiceMock = new Mock<ICardService>();
            _cardController = new CardController(_cardServiceMock.Object);
        }

        [Fact]
        public async Task CreateCard_ReturnsOkResult()
        {
            // Arrange
            var expectedCard = new Card { CardNumber = "123456789012345", Balance = 1000 };
            _cardServiceMock.Setup(service => service.CreateCardAsync()).ReturnsAsync(expectedCard);

            // Act
            var result = await _cardController.CreateCard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCard = Assert.IsType<Card>(okResult.Value);
            Assert.Equal(expectedCard.CardNumber, returnedCard.CardNumber);
            Assert.Equal(expectedCard.Balance, returnedCard.Balance);
        }

        [Fact]
        public async Task GetBalance_ValidCard_ReturnsOkResult()
        {
            // Arrange
            string cardNumber = "123456789012345";
            decimal expectedBalance = 500;
            _cardServiceMock.Setup(service => service.GetBalanceAsync(cardNumber)).ReturnsAsync(expectedBalance);

            // Act
            var result = await _cardController.GetBalance(cardNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedBalance, okResult.Value);
        }

        [Fact]
        public async Task GetBalance_InvalidCard_ReturnsBadRequest()
        {
            // Act
            var result = await _cardController.GetBalance("");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetBalance_NonExistentCard_ReturnsNotFound()
        {
            // Arrange
            string cardNumber = "123456789012345";
            _cardServiceMock.Setup(service => service.GetBalanceAsync(cardNumber)).ReturnsAsync((decimal?)null);

            // Act
            var result = await _cardController.GetBalance(cardNumber);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Pay_ValidPayment_ReturnsOkResult()
        {
            // Arrange
            string cardNumber = "123456789012345";
            decimal amount = 100;
            _cardServiceMock.Setup(service => service.PayAsync(cardNumber, amount)).ReturnsAsync(true);

            // Act
            var result = await _cardController.Pay(cardNumber, amount);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment successful", okResult.Value);
        }

        [Fact]
        public async Task Pay_InvalidCard_ReturnsBadRequest()
        {
            // Act
            var result = await _cardController.Pay("", 100);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Pay_ZeroAmount_ReturnsBadRequest()
        {
            // Act
            var result = await _cardController.Pay("123456789012345", 0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Pay_FailedPayment_ReturnsBadRequest()
        {
            // Arrange
            string cardNumber = "123456789012345";
            decimal amount = 100;
            _cardServiceMock.Setup(service => service.PayAsync(cardNumber, amount)).ReturnsAsync(false);

            // Act
            var result = await _cardController.Pay(cardNumber, amount);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
