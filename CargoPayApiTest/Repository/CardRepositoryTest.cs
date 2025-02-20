using CargoPayAPI.Data;
using CargoPayAPI.Repository;
using CargoPayAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CargoPayAPITest.Respository
{
    public class CardRepositoryTest
    {
        private readonly AppDbContext _context;
        private readonly Mock<IPaymentFeeService> _paymentFeeServiceMock;
        private readonly CardRepository _cardRepository;

        public CardRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new AppDbContext(options);
            _paymentFeeServiceMock = new Mock<IPaymentFeeService>();

            _cardRepository = new CardRepository(_context, _paymentFeeServiceMock.Object);
        }

        [Fact]
        public async Task CreateCardAsync_ValidCard_ReturnsCard()
        {
            // Arrange
            string cardNumber = "123456789012345";

            // Act
            var createdCard = await _cardRepository.CreateCardAsync(cardNumber);

            // Assert
            Assert.NotNull(createdCard);
            Assert.Equal(cardNumber, createdCard.CardNumber);
            Assert.Equal(1000, createdCard.Balance);
        }

        [Fact]
        public async Task GetCardAsync_CardExists_ReturnsCard()
        {
            // Arrange
            string cardNumber = "123456789012345";
            await _cardRepository.CreateCardAsync(cardNumber);

            // Act
            var card = await _cardRepository.GetCardAsync(cardNumber);

            // Assert
            Assert.NotNull(card);
            Assert.Equal(cardNumber, card.CardNumber);
        }

        [Fact]
        public async Task GetCardAsync_CardDoesNotExist_ReturnsNull()
        {
            // Act
            var card = await _cardRepository.GetCardAsync("999999999999999");

            // Assert
            Assert.Null(card);
        }

        [Fact]
        public async Task PayAsync_SufficientBalance_ReturnsTrue()
        {
            // Arrange
            string cardNumber = "123456789012345";
            var card = await _cardRepository.CreateCardAsync(cardNumber);
            _paymentFeeServiceMock.Setup(p => p.GetCurrentFee()).Returns(0.1m); 

            // Act
            var result = await _cardRepository.PayAsync(cardNumber, 500); 

            // Assert
            Assert.True(result);
            var updatedCard = await _cardRepository.GetCardAsync(cardNumber);
            Assert.Equal(450, updatedCard.Balance); 
        }

        [Fact]
        public async Task PayAsync_InsufficientBalance_ReturnsFalse()
        {
            // Arrange
            string cardNumber = "123456789012345";
            var card = await _cardRepository.CreateCardAsync(cardNumber);
            _paymentFeeServiceMock.Setup(p => p.GetCurrentFee()).Returns(0.5m);

            // Act
            var result = await _cardRepository.PayAsync(cardNumber, 900); 

            // Assert
            Assert.False(result);
            var updatedCard = await _cardRepository.GetCardAsync(cardNumber);
            Assert.Equal(1000, updatedCard.Balance); 
        }
    }
}
