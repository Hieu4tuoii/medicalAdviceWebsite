using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public interface IUsersRepository : IRepository<User>
    {
        //danh sách các phương thức của UsersRepository
        //...
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> AuthenticateAsync(string email, string password);
        Task saveUserAsync(User user);
    }
}
