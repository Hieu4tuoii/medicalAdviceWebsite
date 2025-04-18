using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public interface IDoctorsRepository : IRepository<Doctor>
    {
        //danh sách các phương thức của DoctorsRepository
        //...
        Task<Doctor?> GetDoctorByUserIdAsync(int userId);
        //get doctor limit size
        Task<List<Doctor>> FindTopDoctor(int? size);
    }
}
