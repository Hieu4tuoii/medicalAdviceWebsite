

using Microsoft.EntityFrameworkCore;
using WebsiteTuVan.Data;
using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public class UsersRepository : Repository<User>, IUsersRepository
    {
        private readonly ApplicationDbContext _context;

        public UsersRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        //kiem tra ton tai email trong csdl
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            if ((await _context.Users.FirstOrDefaultAsync(u => u.Email == email)) == null)
            {
                return false;
            }
            return true;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            //get user by email
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            //xác thực mật khẩu
            return user.VerifyPassword(password);
        }

        public async Task saveUserAsync(User user)
        {
            //thêm người dùng vào cơ sở dữ liệu
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
