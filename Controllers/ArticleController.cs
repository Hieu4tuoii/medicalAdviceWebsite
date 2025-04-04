using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Repositories;

namespace WebsiteTuVan.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticlesRepository _repository;

        public ArticleController(IArticlesRepository repository)
        {
            _repository = repository;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}