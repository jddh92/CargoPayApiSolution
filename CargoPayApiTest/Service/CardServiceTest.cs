using CargoPayAPI.Model;
using CargoPayAPI.Repository.Interfaces;
using CargoPayAPI.Service;
using Moq;

namespace CargoPayAPITest.Service
{
    public class CardServiceTest
    {
        private readonly Mock<ICardRepository> _cardRepositoryMock;
        private readonly CardService _cardService;

        public CardServiceTest()
        {
            _cardRepositoryMock = new Mock<ICardRepository>();
            _cardService = new CardService( _cardRepositoryMock.Object );
        }

        [Fact]
        public async Task CreateCardAsync_ReturnsNewCard()
        {
            // Arrange
            var expectedCard = new Card { CardNumber = "123456789012345", Balance = 1000 };

            _cardRepositoryMock.Setup(repo => repo.GetCardAsync(It.IsAny<string>()))
                               .ReturnsAsync((Card)null); 

            _cardRepositoryMock.Setup(repo => repo.CreateCardAsync(It.IsAny<string>()))
                               .ReturnsAsync(expectedCard);

            // Act
            var result = await _cardService.CreateCardAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCard.CardNumber, result.CardNumber);
            Assert.Equal(expectedCard.Balance, result.Balance);
        }

        [Fact]
        public async Task GetBalanceAsync_CardExists_ReturnsBalance()
        {
            // Arrange
            var cardNumber = "123456789012345";
            var expectedBalance = 500m;
            var card = new Card { CardNumber = cardNumber, Balance = expectedBalance };

            _cardRepositoryMock.Setup(repo => repo.GetCardAsync(cardNumber))
                               .ReturnsAsync(card);

            // Act
            var result = await _cardService.GetBalanceAsync(cardNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBalance, result);
        }

        [Fact]
        public async Task GetBalanceAsync_CardDoesNotExist_ReturnsNull()
        {
            // Arrange
            var cardNumber = "123456789012345";

            _cardRepositoryMock.Setup(repo => repo.GetCardAsync(cardNumber))
                               .ReturnsAsync((Card)null); 

            // Act
            var result = await _cardService.GetBalanceAsync(cardNumber);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task PayAsync_SuccessfulPayment_ReturnsTrue()
        {
            // Arrange
            var cardNumber = "123456789012345";
            var amount = 200m;

            _cardRepositoryMock.Setup(repo => repo.PayAsync(cardNumber, amount))
                               .ReturnsAsync(true);

            // Act
            var result = await _cardService.PayAsync(cardNumber, amount);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PayAsync_FailedPayment_ReturnsFalse()
        {
            // Arrange
            var cardNumber = "123456789012345";
            var amount = 200m;

            _cardRepositoryMock.Setup(repo => repo.PayAsync(cardNumber, amount))
                               .ReturnsAsync(false);

            // Act
            var result = await _cardService.PayAsync(cardNumber, amount);

            // Assert
            Assert.False(result);
        }
    }
}
