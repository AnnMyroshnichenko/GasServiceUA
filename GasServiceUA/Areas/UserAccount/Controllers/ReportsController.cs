using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GasServiceUA.Areas.UserAccount.Models;
using GasServiceUA.Models;
using GasServiceUA.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SelectPdf;
using GasServiceUA.Helpers;
using GasServiceUA.Services;
using GasServiceUA.UnitOfWork;
using GasServiceUA.Repositories;


namespace GasServiceUA.Areas.UserAccount.Controllers
{
    [Area("UserAccount")]
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentCreatorService _documentCreatorService;
        private IRepository<User> _userRepository;
        private IRepository<MeterReading> _meterReadingRepository;
        private IRepository<Bill> _billRepository;
        private IRepository<Payment> _paymentRepository;

        public ReportsController(IUnitOfWork unitOfWork, 
            IDocumentCreatorService documentCreatorService, 
            IRepository<User> userRepository,
            IRepository<MeterReading> meterReadingRepository, 
            IRepository<Bill> billRepository, 
            IRepository<Payment> paymentRepository)
        {
            _unitOfWork = unitOfWork;
            _documentCreatorService = documentCreatorService;
            _userRepository = userRepository;
            _meterReadingRepository = meterReadingRepository;
            _billRepository = billRepository;
            _paymentRepository = paymentRepository;
        }

        public ActionResult GenerateMeterReadingsReport(DateTime fromDate, DateTime toDate)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = _userRepository.Get(
                x => x.Id.ToString().Equals(userId), 
                null, 
                "Tariffs"
            ).FirstOrDefault();

            IEnumerable<MeterReading> meterReadings = _meterReadingRepository.Get(
                x => x.UsersId.ToString().Equals(userId) &&
                x.StartDate.Date >= fromDate.Date && 
                x.StartDate.Date <= toDate.Date,
                x => x.OrderByDescending(x => x.StartDate)
            );

            IEnumerable<Bill> bills = _billRepository.Get(
                x => x.UsersId.ToString().Equals(userId),
                null,
                "MeterReadings"
            );

            MeterReadingsReportViewModel model = new MeterReadingsReportViewModel() 
            {  
                User = user, 
                Bills = bills, 
                MeterReadings = meterReadings, 
                FromDate = fromDate, 
                ToDate = toDate 
            };

            string viewName = "MeterReadingsReport";
            string viewHtml = this.RenderView(viewName, model);
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/";
            byte[] pdf = _documentCreatorService.CreateDocument(viewHtml, baseUrl);

            return File(pdf, 
                "application/pdf", 
                $"GasServiceUA_MeterReadings_Report_{fromDate.ToString("dd.MM.yyyy")}-{toDate.ToString("dd.MM.yyyy")}.pdf");
        }


        public ActionResult GeneratePaymentHistoryReport(DateTime fromDate, DateTime toDate)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = _userRepository.Get(
                x => x.Id.ToString().Equals(userId),
                null,
                "Tariffs"
            ).FirstOrDefault();

            IEnumerable<Payment> payments = _paymentRepository.Get(
                x => x.UsersId.ToString().Equals(userId) &&
                x.Date.Date >= fromDate.Date && 
                x.Date.Date <= toDate.Date,
                x => x.OrderByDescending(x => x.Date)
            );

            PaymentHistoryReportViewModel model = new PaymentHistoryReportViewModel()
            {
                User = user,
                Payments= payments,
                FromDate = fromDate,
                ToDate = toDate
            };

            string viewName = "PaymentHistoryReport";
            string viewHtml = this.RenderView(viewName, model);
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/";
            byte[] pdf = _documentCreatorService.CreateDocument(viewHtml, baseUrl);

            return File(pdf,
                "application/pdf",
                $"GasServiceUA_Payment_History_{fromDate.ToString("dd.MM.yyyy")}-{toDate.ToString("dd.MM.yyyy")}.pdf");
        }
    }
}
