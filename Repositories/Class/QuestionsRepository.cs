using Microsoft.EntityFrameworkCore;
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
        public async Task<IEnumerable<Question>> GetByPatientIdAsync(int patientId)
        {
            // _dbSet được kế thừa từ lớp Repository<T>
            return await _dbSet
                .Where(q => q.PatientId == patientId)
                .OrderByDescending(q => q.CreatedAt) // Sắp xếp mới nhất lên đầu
                .ToListAsync(); // Lấy danh sách
        }
    }
}
