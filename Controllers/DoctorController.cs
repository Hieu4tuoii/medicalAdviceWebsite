using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Repositories;

namespace WebsiteTuVan.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorsRepository _repository;

        public DoctorController(IDoctorsRepository repository)
        {
            _repository = repository;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}