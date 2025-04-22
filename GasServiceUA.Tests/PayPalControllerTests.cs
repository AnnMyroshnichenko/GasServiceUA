using GasServiceUA.Areas.UserAccount.Controllers;
using GasServiceUA.Data;
using GasServiceUA.Models;
using GasServiceUA.Repositories;
using GasServiceUA.Services;
using GasServiceUA.UnitOfWork;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PaypalCheckoutExample.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GasServiceUA.Tests
{
    public class PayPalControllerTests
    {
        private readonly Mock<IPayPalService> _mockPaypalService;
        private readonly Mock<ICurrencyConverterService> _mockCurrencyConverterService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IRepository<Payment>> _mockPaymentRepository;
        private Mock<IRepository<User>> _mockUserRepository;
        private readonly PaypalController _controller;

        public PayPalControllerTests()
        {
            _mockPaypalService = new Mock<IPayPalService>();
            _mockCurrencyConverterService = new Mock<ICurrencyConverterService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPaymentRepository = new Mock<IRepository<Payment>>();
            _mockUserRepository = new Mock<IRepository<User>>();

            _mockUnitOfWork.Setup(uow => uow.Context).Returns(new Mock<AppDbContext>().Object);
            _mockUnitOfWork.Setup(uow => uow.SaveChanges()).Verifiable();

            _controller = new PaypalController (
                _mockPaypalService.Object,
                _mockCurrencyConverterService.Object,
                _mockUnitOfWork.Object,
                _mockPaymentRepository.Object,
                _mockUserRepository.Object
            );
        }

        [Fact]
        public async Task Order_WithValidArguments_ReturnsOkResult()
        {
            // Arrange
            string paymentSum = "1000";
            string convertedPrice = "27.5";
            string currency = "USD";
            string reference = "INV436845865473737808";
            var orderResponse = new CreateOrderResponse{ status = "Created", id = "Order123"};
            
            _mockCurrencyConverterService.Setup(s => s.GetCurrencyExchange("UAH", "USD", paymentSum))
                .Returns(convertedPrice);

            _mockPaypalService.Setup(c => c.CreateOrder(convertedPrice, currency, It.IsAny<string>()))
                .ReturnsAsync(orderResponse);

            // Act
            var result = await _controller.Order(CancellationToken.None, paymentSum);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orderResponse, okResult.Value);
        }

        [Fact]
        public async Task Order_WhenExceptionThrown_ReturnsBadRequestResult()
        {
            // Arrange
            string paymentSum = "1000";
            string errorMessage = "Currency conversion failed";

            _mockCurrencyConverterService.Setup(s => s.GetCurrencyExchange("UAH", "USD", paymentSum))
                .Throws(new Exception(errorMessage));

            // Act
            var result = await _controller.Order(CancellationToken.None, paymentSum);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null));
        }

        [Fact]
        public async Task Capture_WithValidArguments_ReturnsOkResponse()
        {
            // Arrange
            string orderId = "ORD123";
            string paymentSum = "1000";
            var expectedErrorMessage = "Payment capture was not successful.";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var u = new User { Id = 1, Balance = 3000 };

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()
            )).Returns(new List<User> { u });

            var purchaseUnit = new PurchaseUnit() { reference_id = "reference_id" };
            var captureOrderResponse = new CaptureOrderResponse() { purchase_units = new List<PurchaseUnit>() { purchaseUnit }, status = "COMPLETED" };

            _mockPaypalService.Setup(c => c.CaptureOrder(orderId))
                .ReturnsAsync(captureOrderResponse);

            // Act
            var result = await _controller.Capture(orderId, paymentSum, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CaptureOrderResponse>(okResult.Value);
        }

        [Fact]
        public async Task Capture_WhenRequestStatusIsNotCompleted_ReturnsBadRequest()
        {
            // Arrange
            string orderId = "ORD123";
            string paymentSum = "1000";
            var expectedErrorMessage = "Payment capture was not successful.";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var u = new User { Id = 1, Balance = 3000 };

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()
            )).Returns(new List<User> { u });

            var purchaseUnit = new PurchaseUnit() { reference_id = "reference_id" };
            var captureOrderResponse = new CaptureOrderResponse() { purchase_units = new List<PurchaseUnit>() { purchaseUnit }, status = "" };

            _mockPaypalService.Setup(c => c.CaptureOrder(orderId))
                .ReturnsAsync(captureOrderResponse);

            // Act
            var result = await _controller.Capture(orderId, paymentSum, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expectedErrorMessage, badRequestResult.Value.ToString());
        }


        [Fact]
        public async Task Capture_WithValidArgumentsAndResponseStatusCompleted_CreatesPayment()
        {
            // Arrange
            string paymentSum = "1000";
            string orderId = "ORD123";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var u = new User { Id = 1, Balance = 3000 };

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()
            )).Returns(new List<User> { u });

            var purchaseUnit = new PurchaseUnit() { reference_id = "reference_id" };
            var captureOrderResponse = new CaptureOrderResponse() { purchase_units = new List<PurchaseUnit>() { purchaseUnit }, status = "COMPLETED" };

            _mockPaypalService.Setup(c => c.CaptureOrder(orderId))
                .ReturnsAsync(captureOrderResponse);

            // Act
            var result = await _controller.Capture(orderId, paymentSum, CancellationToken.None);

            // Assert
            _mockPaymentRepository.Verify(repo => repo.Create(It.Is<Payment>(p =>
                p.UsersId == u.Id &&
                p.Sum == Convert.ToSingle(paymentSum) &&
                p.Date.Date == DateTime.Now.Date
            )), Times.Once);
        }

        [Fact]
        public async Task Capture_WithValidArgumentsAndResponseStatusCompleted_UpdatesUserBalance()
        {
            // Arrange
            string paymentSum = "1000";
            string orderId = "ORD123";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var u = new User { Id = 1, Balance = 3000 };

            _mockUserRepository.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<string>()
            )).Returns(new List<User> { u });

            var purchaseUnit = new PurchaseUnit() { reference_id = "reference_id" };
            var captureOrderResponse = new CaptureOrderResponse() { purchase_units = new List<PurchaseUnit>() { purchaseUnit }, status = "COMPLETED" };

            _mockPaypalService.Setup(c => c.CaptureOrder(orderId))
                .ReturnsAsync(captureOrderResponse);

            // Act
            var result = await _controller.Capture(orderId, paymentSum, CancellationToken.None);

            // Assert
            _mockUserRepository.Verify(user => user.Update(u.Id, It.Is<User>(user =>
                user.Id == u.Id &&
                user.Balance == u.Balance
            )), Times.Once);
        }

        [Fact]
        public void Success_OnCall_ReturnsView()
        {
            // Arrange

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            // Act
            var result = _controller.Success();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
