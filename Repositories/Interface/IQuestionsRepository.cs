


using WebsiteTuVan.Models;

namespace WebsiteTuVan.Repositories
{
    public interface IQuestionsRepository : IRepository<Question>
    {
        //danh sách các phương thức của QuestionsRepository
        //...
        // Thêm phương thức này để lấy câu hỏi theo ID bệnh nhân
        Task<IEnumerable<Question>> GetByPatientIdAsync(int patientId);
    }
}
