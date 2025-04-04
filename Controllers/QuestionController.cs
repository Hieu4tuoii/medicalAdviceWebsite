using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Repositories;
using WebsiteTuVan.Models; // Add this for Question model

namespace WebsiteTuVan.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionsRepository _repository;

        public QuestionController(IQuestionsRepository repository)
        {
            _repository = repository;
        }
        public async Task<IActionResult> Index()
        {
            var questions = await _repository.GetAllAsync();
            return View(questions);
        }

        // GET: Question/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: Question/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(question);
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }
    }
}