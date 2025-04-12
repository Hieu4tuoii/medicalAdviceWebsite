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

        public async Task<Doctor?> GetDoctorByUserIdAsync(int userId)
        {
            return await _context.Doctors.Where(d => d.UserId == userId).FirstOrDefaultAsync();
            //return await _dbSet.FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}
