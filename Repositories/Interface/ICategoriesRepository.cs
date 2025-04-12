using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public interface ICategoriesRepository : IRepository<Category>
    {
        //danh sách các phương thức của CategoriesRepository
        //...
        //get all categories
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
