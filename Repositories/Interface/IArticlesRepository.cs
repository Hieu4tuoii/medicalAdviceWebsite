using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public interface IArticlesRepository : IRepository<Article>
    {
        //danh sách các phương thức của ArticlesRepository
        //...
         Task SaveAsync(Article article);
         Task<List<Article>> FindByDoctorId(int doctorId, string status, string keyword);
         Task<Article> FindById(int id);
         //lấy ds bài viết đã đc đăng(bao gồm cả tìm kiếm theo tiêu đề)
         Task<List<Article>> FindAllPublished(int? categoryId, string? keyword, int? size);
    }
}
