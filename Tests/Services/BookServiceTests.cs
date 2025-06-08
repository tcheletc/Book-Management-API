using BookApi.Data;
using BookApi.Models;
using BookApi.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace BookApi.Tests.Services
{
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

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            var result = await _service.GetByIdAsync(999);
            Assert.Null(result);
        }

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

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenBookNotFound()
        {
            var result = await _service.UpdateAsync(999, new Book());
            Assert.False(result);
        }

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

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenBookNotFound()
        {
            var result = await _service.DeleteAsync(404);
            Assert.False(result);
        }

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
