using WebsiteTuVan.Data;
using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public class CategoriesRepository : Repository<Category>, ICategoriesRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Implement any additional methods specific to Categories here
    }
}
