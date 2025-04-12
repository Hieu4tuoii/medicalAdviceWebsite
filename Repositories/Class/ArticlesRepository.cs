using Microsoft.EntityFrameworkCore;
using WebsiteTuVan.Data;
using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public class ArticlesRepository : Repository<Article>, IArticlesRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticlesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        

        public async Task<List<Article>> FindAllPublished(int? categoryId, string? keyword)
        {
             //nếu có keyword thì mới tìm kiếm
            var query = _context.Articles.Where(a => a.Status == "published" && a.CategoryId == categoryId);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.Title.ToLower().Contains(keyword.ToLower()));
            }
            //nếu categoryId khác null và khác 0 thì thêm điều kiện tìm kiếm theo categoryId
            if(categoryId!=null && categoryId!=0){
                query = query.Where(a => a.CategoryId == categoryId);
            }
            return await query.ToListAsync();
        }

        public async Task<List<Article>> FindByDoctorId(int doctorId, string status, string keyword)
        {
            var query = _context.Articles.Where(a => a.DoctorId == doctorId);

            //nếu status không phải là all thì tìm kiếm theo status
            if (status != "all")
            {
                query = query.Where(a => a.Status == status);
            }
            //nếu keyword không rỗng thì thêm điều kiện tìm kiếm theo tiêu đề
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.Title.ToLower().Contains(keyword.ToLower()));
            }
            return await query.ToListAsync();
        }

        public async Task<Article> FindById(int id)
        {
          return await _context.Articles.Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task SaveAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
        }

        // Implement any additional methods specific to Articles here
    }
}
