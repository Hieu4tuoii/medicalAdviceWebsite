using Microsoft.EntityFrameworkCore;

namespace WebsiteTuVan.Helpers
{
    public class PagedResult<T> where T : class
    {
        public List<T> Items { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        private PagedResult(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            // Tính tổng số trang, đảm bảo ít nhất là 1 trang nếu có dữ liệu hoặc 0 nếu không
            TotalPages = (pageSize > 0 && count > 0) ? (int)Math.Ceiling(count / (double)pageSize) : 0;
            Items = items; // Gán danh sách items
        }
        public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // Đảm bảo pageSize và pageNumber hợp lệ
            pageSize = Math.Max(1, pageSize); // Ít nhất 1 mục/trang
            pageNumber = Math.Max(1, pageNumber); // Trang đầu tiên là 1

            // Đếm tổng số lượng item (thực hiện truy vấn COUNT vào DB)
            var count = await source.CountAsync();

            // Lấy các items cho trang hiện tại (thực hiện truy vấn SELECT với OFFSET/FETCH vào DB)
            // Chỉ thực hiện nếu tổng số lượng > 0 và trang yêu cầu hợp lệ
            List<T> items;
            if (count > 0 && pageNumber <= (int)Math.Ceiling(count / (double)pageSize))
            {
                items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else
            {
                // Nếu không có item nào hoặc yêu cầu trang không hợp lệ, trả về danh sách rỗng
                items = new List<T>();
                // Điều chỉnh lại pageNumber nếu nó vượt quá số trang thực tế
                if (count == 0) pageNumber = 1; // Nếu không có item nào thì coi như ở trang 1
                // Không cần điều chỉnh thêm vì constructor sẽ tính TotalPages = 0
            }


            // Tạo và trả về đối tượng PagedResult
            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }
    }
}
