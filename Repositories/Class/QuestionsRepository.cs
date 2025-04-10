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

        public async Task<Question?> GetDetailsByIdAsync(int id)
        {
            // Include thêm các bảng liên quan nếu cần hiển thị thông tin từ chúng (ví dụ: Patient, Category)
            return await _dbSet
                .Include(q => q.Category) // Ví dụ: Lấy thông tin Category
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<List<Question>> GetPublicAnsweredQuestionsAsync(int? categoryId, int excludeQuestionId, int count)
        {
            var query = _dbSet.AsQueryable(); // Đảm bảo bắt đầu với IQueryable

            // --- BƯỚC 1: ÁP DỤNG TẤT CẢ CÁC BỘ LỌC (WHERE) TRƯỚC ---
            query = query.Where(q => q.Status == "answered" && q.Id != excludeQuestionId);

            if (categoryId.HasValue && categoryId > 0)
            {
                // Áp dụng lọc CategoryId ở đây, TRƯỚC Include
                query = query.Where(q => q.CategoryId == categoryId.Value);
            }

            // --- BƯỚC 2: ÁP DỤNG INCLUDE VÀ THENINCLUDE SAU KHI ĐÃ LỌC ---
            // Bây giờ biến query vẫn là IQueryable<Question>, Include sẽ hoạt động đúng
            query = query
                .Include(q => q.Answers)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User);

            // --- BƯỚC 3: ÁP DỤNG SẮP XẾP VÀ PHÂN TRANG/GIỚI HẠN ---
            return await query
                .OrderByDescending(q => q.CreatedAt) // Hoặc logic sắp xếp khác
                .Take(count)
                .ToListAsync();
        }
        public async Task<IEnumerable<Question>> GetAllIncludingPatientAsync()
        {
            return await _dbSet
                .Include(q => q.Patient) // Include thông tin người hỏi
                .OrderByDescending(q => q.CreatedAt) // Sắp xếp mặc định
                .ToListAsync();
        }
        public async Task<Question?> GetDetailsByIdIncludingPatientAndAttachmentsAsync(int id)
        {
            return await _dbSet
                .Include(q => q.Patient)        // Lấy thông tin người hỏi
                .Include(q => q.Attachments)    // Lấy file đính kèm của câu hỏi
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
