using BookApi.Data;
using BookApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Services
{
    public class BookService
    {
        private readonly BookContext _context;

        public BookService(BookContext context)
        {
            _context = context;
        }

        // The function returns all books in the database (paginated)
        public async Task<List<Book>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _context.Books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(b => b.Author)
                .ThenBy(b => b.Title)
                .ToListAsync();
        }

        // The function returns a book from the database by ID
        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        //The function adds the received book as a parameter to the database, and returns it
        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        // The function updates the book with the ID received as a parameter
        // with the details of the book received as a parameter
        // and returns whether the update was successful.
        public async Task<bool> UpdateAsync(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return false;

            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.PublicationDate = updatedBook.PublicationDate;
            book.Price = updatedBook.Price;

            await _context.SaveChangesAsync();
            return true;
        }

        // The function deletes the book with the ID number received as a parameter from the database
        // and returns whether the deletion was successful.
        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        // The function searches (case insensitive) the database for books by title or author name
        // and returns the list of books that match the search.
        public async Task<List<Book>> SearchAsync(string query)
        {
            query = query.ToLower();
            return await _context.Books
                .Where(b => b.Title.ToLower().Contains(query) || b.Author.ToLower().Contains(query))
                .ToListAsync();
        }
    }
}
