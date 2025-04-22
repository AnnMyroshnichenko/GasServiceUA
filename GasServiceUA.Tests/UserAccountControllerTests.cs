using GasServiceUA.Areas.UserAccount.Controllers;
using GasServiceUA.Areas.UserAccount.Models;
using GasServiceUA.Data;
using GasServiceUA.Models;
using GasServiceUA.Repositories;
using GasServiceUA.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Formats.Tar;
using System.Linq.Expressions;
using System.Security.Claims;

namespace GasServiceUA.Tests
{
    public class UserAccountControllerTests
    {
        private readonly Mock<IRepository<User>> _mockUserRepository;
        private readonly Mock<IRepository<MeterReading>> _mockMeterReadingRepository;
        private readonly Mock<IRepository<Bill>> _mockBillRepository;
        private readonly Mock<IRepository<Payment>> _mockPaymentRepository;
        private readonly Mock<IRepository<Tariff>> _mockTariffRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UserAccountController _controller;

        public UserAccountControllerTests() {
            _mockUserRepository = new Mock<IRepository<User>>();
            _mockMeterReadingRepository = new Mock<IRepository<MeterReading>>();
            _mockBillRepository = new Mock<IRepository<Bill>>();
            _mockPaymentRepository = new Mock<IRepository<Payment>>();
            _mockTariffRepository = new Mock<IRepository<Tariff>>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(uow => uow.Context).Returns(new Mock<AppDbContext>().Object);
            _mockUnitOfWork.Setup(uow => uow.SaveChanges()).Verifiable();

            _controller = new UserAccountController(
                _mockUnitOfWork.Object,
                _mockUserRepository.Object,
                _mockMeterReadingRepository.Object,
                _mockBillRepository.Object,
                _mockPaymentRepository.Object,
                _mockTariffRepository.Object
            );
        }

        [Fact]
        public void Index_OnCall_ReturnsViewWithModel()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user }};

            var u = new User { Id = 1 };
            var meterReadings = new List<MeterReading> { new MeterReading() };
            var bills = new List<Bill> { new Bill() };
            var payments = new List<Payment> { new Payment() };
             
            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()
            )).Returns(new List<User> { u });

            _mockMeterReadingRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<MeterReading, bool>>>(),
                It.IsAny<Func<IQueryable<MeterReading>, IOrderedQueryable<MeterReading>>>(),
                It.IsAny<string>()
            )).Returns(meterReadings);

            _mockBillRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<Bill, bool>>>(),
                It.IsAny<Func<IQueryable<Bill>, IOrderedQueryable<Bill>>>(),
                It.IsAny<string>()
            )).Returns(bills);

            _mockPaymentRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                It.IsAny<Func<IQueryable<Payment>, IOrderedQueryable<Payment>>>(),
                It.IsAny<string>()
            )).Returns(payments);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as UserAccountViewModel;
            Assert.NotNull(model);
        }

        [Fact]
        public void SendMeterReadings_WithValidArguments_ReturnsJson()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            long startMeterReading = 1000;
            long endMeterReading = 1500;

            var tariff = new Tariff { CostPerGasUnit = 1.5f };
            var u = new User { Id = 1, Tariffs = tariff, Balance = 0};

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()))
                .Returns(new List<User> { u }.AsQueryable());

            // Act
            var result = _controller.SendMeterReadings(startMeterReading, endMeterReading);

            // Assert
            Assert.IsType<JsonResult>(result);
            Assert.Equal("Meter readings successfully sent", result.Value);
        }

        [Fact]
        public void SendMeterReadings_WithValidArguments_CreatesMeterReading() 
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var meterReadings = new List<MeterReading> { new MeterReading() };

            _mockMeterReadingRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<MeterReading, bool>>>(),
                It.IsAny<Func<IQueryable<MeterReading>, IOrderedQueryable<MeterReading>>>(),
                It.IsAny<string>()
            )).Returns(meterReadings);
            
            var startMeterReading = 1000L;
            var endMeterReading = 1500L;
            
            var tariff = new Tariff { CostPerGasUnit = 1.5f };
            var u = new User { Id = 1, Tariffs = tariff, Balance = 0 };

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(), 
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(), 
                It.IsAny<string>()))
                .Returns(new List<User> { u }.AsQueryable());

            // Act
            _controller.SendMeterReadings(startMeterReading, endMeterReading);

            // Assert
            _mockMeterReadingRepository.Verify(repo => repo.Create(It.Is<MeterReading>(m =>
                m.StartMeterReading == startMeterReading &&
                m.EndMeterReading == endMeterReading &&
                m.UsersId == 1
            )), Times.Once);
        }

        [Theory]
        [InlineData(10, 100, 180)]
        [InlineData(10, 10, 0)]
        [InlineData(98754532, 98754632, 200)]
        public void CalcCost_WithValidArguments_ReturnsCorrectCost(long startMeterReading, long endMeterReading, float expected)
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var tariff = new Tariff { CostPerGasUnit = 2f };
            var u = new User { Id = 1, Tariffs = tariff, Balance = 0 };

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()))
                .Returns(new List<User> { u }.AsQueryable());

            // Act
            var actual = _controller.CalcCost(startMeterReading, endMeterReading);

            // Assert
            Assert.Equal(actual, expected);
        }

        [Theory]
        [InlineData(-2, 100)]
        [InlineData(10, -10)]
        [InlineData(0, 76554633)]
        [InlineData(0, 0)]
        [InlineData(10, 9)]
        [InlineData(-390, -763513)]
        public void CalcCost_WithInvalidArguments_ThrowsInvalidOperationException(long startMeterReading, long endMeterReading)
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var tariff = new Tariff { CostPerGasUnit = 2f };
            var u = new User { Id = 1, Tariffs = tariff, Balance = 0 };

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()))
                .Returns(new List<User> { u }.AsQueryable());

            // Act
            Exception ex = Record.Exception(() => _controller.CalcCost(startMeterReading, endMeterReading));

            // Assert
            Assert.IsType<InvalidOperationException>(ex);

        }

        [Fact]
        public void CreateBill_WithValidArguments_AddsNewBill()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var tariff = new Tariff { CostPerGasUnit = 2f };
            var u = new User { Id = 1, Tariffs = tariff, Balance = 0 };

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()))
                .Returns(new List<User> { u }.AsQueryable());

            var testMeterReading = new MeterReading {StartDate = DateTime.Now, EndDate = DateTime.Now, UsersId = 1, StartMeterReading = 76543258, EndMeterReading = 96346824};

            // Act
            _controller.CreateBill(testMeterReading);

            // Assert
            _mockBillRepository.Verify(repo => repo.Create(It.Is<Bill>(b =>
                b.StartDate == testMeterReading.StartDate &&
                b.EndDate == testMeterReading.EndDate &&
                b.UsersId == testMeterReading.UsersId &&
                b.MeterReadingsId == testMeterReading.MeterReadingsId &&
                b.Cost == _controller.CalcCost(testMeterReading.StartMeterReading, testMeterReading.EndMeterReading)
            )), Times.Once);
        }
    }
}