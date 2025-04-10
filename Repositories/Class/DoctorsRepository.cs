using Microsoft.EntityFrameworkCore;
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
        public async Task<Doctor?> GetDoctorByUserIdAsync(int userId)
        {
            // Giả định bạn có trường UserId trong model Doctor liên kết đến User.Id
            return await _dbSet.FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}
