using BookApi.Data;
using BookApi.Models;
using BookApi.Services;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Tests.Services
{
    /// <summary>
    /// Unit tests for the <see cref="BookService"/> class.
    /// These tests verify the behavior of service methods such as retrieving, creating,
    /// updating, deleting, and searching books using an in-memory database.
    /// </summary>
    public class BookServiceTests : IDisposable
    {
        private readonly BookContext _context;
        private readonly BookService _service;

        public BookServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BookContext(options);
            _service = new BookService(_context);
        }

        public void Dispose() => _context.Dispose();

        /// <summary>
        /// Verifies that GetAllAsync returns the correct number of items per page.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsPagedResults()
        {
            for (int i = 1; i <= 15; i++)
            {
                _context.Books.Add(new Book
                {
                    Title = $"Book {i}",
                    Author = "Author",
                    PublicationDate = DateTime.Today,
                    Price = i
                });
            }
            await _context.SaveChangesAsync();

            var page1 = await _service.GetAllAsync(1, 10);
            var page2 = await _service.GetAllAsync(2, 10);

            Assert.Equal(10, page1.Count);
            Assert.Equal(5, page2.Count);
        }

        /// <summary>
        /// Verifies that GetAllAsync returns an empty list when requesting a page beyond the data range.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenPageOutOfRange()
        {
            // Arrange: only 3 books
            _context.Books.AddRange(
                new Book { Title = "B1", Author = "A", PublicationDate = DateTime.Today, Price = 1 },
                new Book { Title = "B2", Author = "A", PublicationDate = DateTime.Today, Price = 2 },
                new Book { Title = "B3", Author = "A", PublicationDate = DateTime.Today, Price = 3 }
            );
            await _context.SaveChangesAsync();

            // Act: Page 2 with 10 items, no books there
            var result = await _service.GetAllAsync(page: 2, pageSize: 10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that GetByIdAsync returns the correct book when it exists.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ReturnsBook_WhenExists()
        {
            var book = new Book { Title = "A", Author = "B", PublicationDate = DateTime.Today, Price = 20 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(book.Id);

            Assert.NotNull(result);
            Assert.Equal(book.Title, result!.Title);
        }

        /// <summary>
        /// Verifies that GetByIdAsync returns null when the book does not exist.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            var result = await _service.GetByIdAsync(999);
            Assert.Null(result);
        }

        /// <summary>
        /// Verifies that AddAsync successfully adds a new book to the database.
        /// </summary>
        [Fact]
        public async Task AddAsync_AddsBook()
        {
            var book = new Book
            {
                Title = "Test Book",
                Author = "Author",
                PublicationDate = DateTime.Today.AddYears(-1),
                Price = 10
            };

            var added = await _service.AddAsync(book);

            Assert.NotEqual(0, added.Id);
            Assert.Single(_context.Books);
        }

        /// <summary>
        /// Verifies that UpdateAsync updates an existing book correctly.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_UpdatesBook_WhenExists()
        {
            var book = new Book { Title = "Old", Author = "A", PublicationDate = DateTime.Today, Price = 5 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var updated = new Book
            {
                Id = book.Id,
                Title = "Updated",
                Author = "B",
                PublicationDate = DateTime.Today,
                Price = 99
            };

            var result = await _service.UpdateAsync(book.Id, updated);

            Assert.True(result);
            var saved = await _context.Books.FindAsync(book.Id);
            Assert.Equal("Updated", saved!.Title);
        }

        /// <summary>
        /// Verifies that UpdateAsync returns false when trying to update a nonexistent book.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenBookNotFound()
        {
            var result = await _service.UpdateAsync(999, new Book());
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that DeleteAsync deletes a book successfully when it exists.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_DeletesBook_WhenExists()
        {
            var book = new Book { Title = "Delete Me", Author = "A", PublicationDate = DateTime.Today, Price = 10 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(book.Id);

            Assert.True(result);
            Assert.Null(await _context.Books.FindAsync(book.Id));
        }

        /// <summary>
        /// Verifies that DeleteAsync returns false when trying to delete a nonexistent book.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenBookNotFound()
        {
            var result = await _service.DeleteAsync(404);
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that SearchAsync returns books that match the query in a case-insensitive way.
        /// </summary>
        [Fact]
        public async Task SearchAsync_ReturnsMatchingBooks_CaseInsensitive()
        {
            _context.Books.AddRange(
                new Book { Title = "C# Programming", Author = "John", PublicationDate = DateTime.Today, Price = 100 },
                new Book { Title = "Learn Java", Author = "john", PublicationDate = DateTime.Today, Price = 80 },
                new Book { Title = "Python", Author = "Mary", PublicationDate = DateTime.Today, Price = 90 }
            );
            await _context.SaveChangesAsync();

            var results = await _service.SearchAsync("john");

            Assert.Equal(2, results.Count);
        }

        /// <summary>
        /// Verifies that SearchAsync returns an empty list when no books match the query.
        /// </summary>
        [Fact]
        public async Task SearchAsync_ReturnsEmptyList_WhenNoMatch()
        {
            // Arrange
            _context.Books.Add(new Book
            {
                Title = "C# Basics",
                Author = "John Doe",
                PublicationDate = DateTime.Today,
                Price = 100
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.SearchAsync("NotMatchingQuery");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
