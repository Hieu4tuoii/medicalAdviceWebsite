using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Repositories;

namespace WebsiteTuVan.Controllers
{
    [Route("Category")]
    public class CategoryController : Controller
    {
        private readonly ICategoriesRepository _repository;
        public CategoryController(ICategoriesRepository repository)
        {
            _repository = repository;
        }

        //get ds catgory
        [HttpGet("")]
        public async Task<IActionResult> getAllCategory()
        {
            var categories = await _repository.GetAllAsync();
            return Json(new { data = categories });
        }
    }
}