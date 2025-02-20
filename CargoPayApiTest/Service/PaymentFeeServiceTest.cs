using CargoPayAPI.Service;

namespace CargoPayAPITest.Service
{
    public class PaymentFeeServiceTest
    {
        private readonly PaymentFeeService _paymentFeeService;

        public PaymentFeeServiceTest()
        {
            _paymentFeeService = new PaymentFeeService();
        }

        [Fact]
        public void GetCurrentFee_InitialValue_ReturnsOne()
        {
            // Act
            var fee = _paymentFeeService.GetCurrentFee();

            // Assert
            Assert.Equal(1.0m, fee);
        }

        [Fact]
        public void ForceUpdateFee_ChangesFeeValue()
        {
            // Arrange
            var initialFee = _paymentFeeService.GetCurrentFee();

            // Act
            _paymentFeeService.ForceUpdateFee();
            var updatedFee = _paymentFeeService.GetCurrentFee();

            // Assert
            Assert.NotEqual(initialFee, updatedFee);
        }

        [Fact]
        public async Task ForceUpdateFee_CalledMultipleTimes_ChangesFeeEachTime()
        {
            // Arrange
            var previousFee = _paymentFeeService.GetCurrentFee();
            decimal newFee = previousFee;

            // Act
            for (int i = 0; i < 5; i++)
            {
                _paymentFeeService.ForceUpdateFee();
                newFee = _paymentFeeService.GetCurrentFee();
                await Task.Delay(50);
            }
            // Assert
            Assert.NotEqual(previousFee, newFee);
        }
    }
}
