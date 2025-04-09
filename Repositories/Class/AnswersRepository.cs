
using Microsoft.EntityFrameworkCore;
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
        public async Task<Answer?> GetByQuestionIdAsync(int questionId)
        {
            // Include Doctor và User của Doctor để hiển thị thông tin bác sĩ
            return await _dbSet // _dbSet ở đây là DbSet<Answer>
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.QuestionId == questionId);
        }
    }
}