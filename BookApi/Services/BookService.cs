using BookApi.Data;
using BookApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Services
{
    /// <summary>
    /// Service that handles operations related to books in the database.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly BookContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public BookService(BookContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all books from the database ordered by author and title, with pagination.
        /// </summary>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A list of books.</returns>
        public async Task<List<Book>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _context.Books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(b => b.Author)
                .ThenBy(b => b.Title)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a book by its ID.
        /// </summary>
        /// <param name="id">The book ID.</param>
        /// <returns>The book if found; otherwise, null.</returns>
        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        /// <summary>
        /// Adds a new book to the database.
        /// </summary>
        /// <param name="book">The book to add.</param>
        /// <returns>The added book.</returns>
        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        /// <summary>
        /// Updates an existing book with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="updatedBook">The updated book details.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
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

        /// <summary>
        /// Deletes a book by its ID.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Searches for books by title or author (case-insensitive).
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <returns>A list of books that match the search criteria.</returns>
        public async Task<List<Book>> SearchAsync(string query)
        {
            query = query.ToLower();
            return await _context.Books
                .Where(b => b.Title.ToLower().Contains(query) || b.Author.ToLower().Contains(query))
                .ToListAsync();
        }
    }
}
