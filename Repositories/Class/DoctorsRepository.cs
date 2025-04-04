using WebsiteTuVan.Data;
using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public class DoctorsRepository : Repository<Doctor>, IDoctorsRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Implement any additional methods specific to Doctors here
    }
}
