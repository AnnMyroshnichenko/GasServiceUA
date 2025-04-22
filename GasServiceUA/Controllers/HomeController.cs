using GasServiceUA.Data;
using GasServiceUA.Helpers;
using GasServiceUA.Models;
using GasServiceUA.Repositories;
using GasServiceUA.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GasServiceUA.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IRepository<Tariff> _tariffRepository;

        public HomeController( 
            IUnitOfWork unitOfWork,
            IRepository<Tariff> tariffRepository)
        {
            _unitOfWork = unitOfWork;
            _tariffRepository = tariffRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Tariffs()
        {
            var tariffs = _tariffRepository.Get();
            return View(tariffs);
        }

        [Route("/Home/Error/{code:int}")]
        public IActionResult Error(int code)
        {
            if (code == 404)
            {
                return View("/Views/Shared/PageNotFound.cshtml");

            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult ThrowEx()
        {
            throw new Exception();
        }
    }
}