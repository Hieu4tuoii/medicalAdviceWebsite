
using WebsiteTuVan.Data;
using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public class AnswersRepository : Repository<Answer>, IAnswersRepository
    {
        private readonly ApplicationDbContext _context;

        public AnswersRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Implement any additional methods specific to Answers here
    }
}