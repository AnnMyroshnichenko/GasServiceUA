using GasServiceUA.Areas.UserAccount.Models;
using GasServiceUA.Data;
using GasServiceUA.Models;
using GasServiceUA.Repositories;
using GasServiceUA.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Claims;

namespace GasServiceUA.Areas.UserAccount.Controllers
{
    [Area("UserAccount")]
    [Authorize]
    public class UserAccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IRepository<User> _userRepository;
        private IRepository<MeterReading> _meterReadingRepository;
        private IRepository<Bill> _billRepository;
        private IRepository<Payment> _paymentRepository;
        private IRepository<Tariff> _tariffRepository;

        public UserAccountController(IUnitOfWork unitOfWork,
                                 IRepository<User> userRepository,
                                 IRepository<MeterReading> meterReadingRepository,
                                 IRepository<Bill> billRepository,
                                 IRepository<Payment> paymentRepository,
                                 IRepository<Tariff> tariffRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _meterReadingRepository = meterReadingRepository;
            _billRepository = billRepository;
            _paymentRepository = paymentRepository;
            _tariffRepository = tariffRepository;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _userRepository.Get(
                u => u.Id.ToString().Equals(userId), 
                null, 
                "Tariffs"
            ).FirstOrDefault();

            if (user == null)
            {
                return View("PageNotFound");
            }

            var meterReadings = _meterReadingRepository.Get(
                m => m.UsersId.ToString().Equals(userId), 
                q => q.OrderByDescending(x => x.StartDate)
            );

            var bills = _billRepository.Get(
                m => m.UsersId.ToString().Equals(userId), 
                null, 
                "MeterReadings"
            );

            var payments = _paymentRepository.Get(x => x.UsersId.ToString().Equals(userId), 
                x => x.OrderByDescending(x => x.Date)
            );

            UserAccountViewModel accountViewModel = new UserAccountViewModel 
            {
                User = user, 
                MeterReadings = meterReadings, 
                Bills = bills, 
                Payments = payments
            };

            return View(accountViewModel);
        }


        [HttpPost]
        public JsonResult SendMeterReadings(long startMeterReading, long endMeterReading) {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var meterReading = new MeterReading() 
            { 
                StartMeterReading = startMeterReading, 
                EndMeterReading = endMeterReading,
                UsersId = int.Parse(userId), 
                StartDate = startDate, 
                EndDate = endDate
            };

            _meterReadingRepository.Create(meterReading);

            CreateBill(meterReading);

            return Json("Meter readings successfully sent"); 
        }


        [HttpPost]
        public float CalcCost(long startMeterReading, long endMeterReading)
        {
            if (startMeterReading > endMeterReading || startMeterReading <= 0 || endMeterReading <= 0)
            {
                throw new InvalidOperationException("Initial meter readings can't be greater than end meter readings");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tariffPrice = _userRepository.Get(
                x => x.Id.ToString().Equals(userId),
                null, 
                "Tariffs"
            ).FirstOrDefault().Tariffs.CostPerGasUnit;

            var cost = (endMeterReading - startMeterReading) * tariffPrice;
            return cost;
        }


        [HttpPost]
        public JsonResult CreateBill(MeterReading meterReading)
        {

            var bill = new Bill()
            {
                StartDate = meterReading.StartDate,
                EndDate = meterReading.EndDate,
                UsersId = meterReading.UsersId,
                MeterReadingsId = meterReading.MeterReadingsId,
                Cost = CalcCost(meterReading.StartMeterReading, meterReading.EndMeterReading)
            };

            _billRepository.Create(bill);
            
            var user = _userRepository.Get(u => u.Id == bill.UsersId).FirstOrDefault();
            user.Balance += bill.Cost;
            _userRepository.Update(user.Id, user);

            return Json("Bill successfully added");
        }
    }
}
