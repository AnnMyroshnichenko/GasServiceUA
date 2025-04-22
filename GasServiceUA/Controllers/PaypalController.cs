using GasServiceUA.Data;
using GasServiceUA.Models;
using GasServiceUA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using GasServiceUA.UnitOfWork;
using GasServiceUA.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaypalCheckoutExample.Controllers
{
    [Authorize]
    public class PaypalController : Controller
    {
        private readonly IPayPalService _paypalService;
        private readonly ICurrencyConverterService _currencyConverterService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<User> _userRepository;

        public PaypalController(IPayPalService paypalService,
            ICurrencyConverterService currencyConverterService,
            IUnitOfWork unitOfWork,
            IRepository<Payment> paymentRepository,
            IRepository<User> userRepository)
        {
            _paypalService = paypalService;
            _unitOfWork = unitOfWork;
            _currencyConverterService = currencyConverterService;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            ViewBag.ClientId = _paypalService.ClientId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Order(CancellationToken cancellationToken, string paymentSum)
        {
            try
            {
                var price = _currencyConverterService.GetCurrencyExchange("UAH", "USD", paymentSum);
                var currency = "USD";

                var reference = "INV" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(1000, 9999);

                var response = await _paypalService.CreateOrder(price, currency, reference);

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Capture(string orderId, string paymentSum, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalService.CaptureOrder(orderId);

                if (response.status != "COMPLETED")
                {
                    return BadRequest("Payment capture was not successful.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _userRepository.Get(u => u.Id.ToString().Equals(userId)).FirstOrDefault();

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var sum = Convert.ToSingle(paymentSum);
                user.Balance -= sum;
                _userRepository.Update(user.Id, user);

                var payment = new Payment()
                {
                    UsersId = int.Parse(userId),
                    Sum = sum,
                    Date = DateTime.Now
                };

                _paymentRepository.Create(payment);

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                return BadRequest("Payment capture was not successful.");
            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
