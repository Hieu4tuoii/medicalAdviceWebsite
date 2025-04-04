using WebsiteTuVan.Data;
using WebsiteTuVan.Models;
namespace WebsiteTuVan.Repositories
{
    public class QuestionsRepository : Repository<Question>, IQuestionsRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Implement any additional methods specific to Questions here
    }
}
