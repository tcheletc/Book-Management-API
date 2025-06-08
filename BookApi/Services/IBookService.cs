using BookApi.Models;

namespace BookApi.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllAsync(int page, int pageSize);
        Task<Book?> GetByIdAsync(int id);
        Task<Book> AddAsync(Book book);
        Task<bool> UpdateAsync(int id, Book updatedBook);
        Task<bool> DeleteAsync(int id);
        Task<List<Book>> SearchAsync(string query);
    }
}
