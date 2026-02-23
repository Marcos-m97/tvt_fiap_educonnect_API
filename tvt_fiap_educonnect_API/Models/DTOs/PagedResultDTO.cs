namespace EduConnect_API.Models.DTOs
{
    public class PagedResultDTO<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
    }
}