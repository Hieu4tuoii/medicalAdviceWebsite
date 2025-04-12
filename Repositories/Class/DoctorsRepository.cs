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

        public async Task<List<Doctor>> FindTopDoctor(int? size)
        {
           return await _context.Doctors.OrderByDescending(d => d.YearOfStartingWork).Take(size.Value).ToListAsync();
        }

        public async Task<Doctor?> GetDoctorByUserIdAsync(int userId)
        {
            return await _context.Doctors.Where(d => d.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
