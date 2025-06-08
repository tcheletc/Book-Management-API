using Moq;
using BookApi.Controllers;
using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockService;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockService = new Mock<IBookService>();
            _controller = new BooksController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsBooks_WhenPageHasResults()
        {
            // Arrange: valid page with books
            var expectedBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author A" },
                new Book { Id = 2, Title = "Book 2", Author = "Author B" }
            };

            _mockService.Setup(s => s.GetAllAsync(1, 10))
                .ReturnsAsync(expectedBooks);

            // Act
            var result = await _controller.GetAll(1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(2, books.Count);
            Assert.Equal("Book 1", books[0].Title);
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenPageHasNoResults()
        {
            // Arrange: page is valid, there are no books on this page
            _mockService.Setup(s => s.GetAllAsync(100, 10))
                .ReturnsAsync(new List<Book>());

            // Act
            var result = await _controller.GetAll(100, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Empty(books);
        }

        [Theory]
        [InlineData(-1, 5)] // page < 1
        [InlineData(1, -10)] // pageSize < 1
        public async Task GetAll_ReturnsBadRequest_WhenPaginationInvalid(int page, int pageSize)
        {
            var result = await _controller.GetAll(page, pageSize);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Page and pageSize must be positive integers", badRequest.Value);
        }

        [Fact]
        public async Task GetById_ReturnsBook_WhenExists()
        {
            var expectedBook = new Book { Id = 1, Title = "Test", Author = "Author" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(expectedBook);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var book = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(expectedBook.Id, book.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Book?)null);

            var result = await _controller.GetById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAt_WhenValid()
        {
            var input = new Book { Title = "New", Author = "A", PublicationDate = DateTime.Now, Price = 10 };
            var created = new Book { Id = 42, Title = "New", Author = "A", PublicationDate = DateTime.Now, Price = 10 };

            _mockService.Setup(s => s.AddAsync(input)).ReturnsAsync(created);

            var result = await _controller.Create(input);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var book = Assert.IsType<Book>(createdResult.Value);
            Assert.Equal(42, book.Id);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange – book with invalid values
            var book = new Book
            {
                Title = "", // empty - invalid
                Author = "Author",
                PublicationDate = default, // default date - invalid
                Price = -10 // negative - invalid
            };

            var controller = new BooksController(_mockService.Object);

            // Forces the controller to state that the model is invalid (as if ModelState.IsValid == false)
            controller.ModelState.AddModelError("Title", "Required");
            controller.ModelState.AddModelError("PublicationDate", "The date must be valid (not default or in the future)");
            controller.ModelState.AddModelError("Price", "Must be non-negative");

            // Act
            var result = await controller.Create(book);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequest.Value);
        }


        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var input = new Book { Title = "Updated", Author = "Author", PublicationDate = DateTime.Now, Price = 15 };

            _mockService.Setup(s => s.UpdateAsync(1, input)).ReturnsAsync(true);

            var result = await _controller.Update(1, input);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenNotExists()
        {
            var input = new Book { Title = "Updated", Author = "Author", PublicationDate = DateTime.Now, Price = 15 };

            _mockService.Setup(s => s.UpdateAsync(999, input)).ReturnsAsync(false);

            var result = await _controller.Update(999, input);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange – book with invalid values
            var book = new Book
            {
                Title = "", // empty - invalid
                Author = "Author",
                PublicationDate = DateTime.Now.AddDays(1), // future date - invalid
                Price = -10 // negative - invalid
            };

            var controller = new BooksController(_mockService.Object);

            // Forces the controller to state that the model is invalid (as if ModelState.IsValid == false)
            controller.ModelState.AddModelError("Title", "Required");
            controller.ModelState.AddModelError("PublicationDate", "The date must be valid (not default or in the future)");
            controller.ModelState.AddModelError("Price", "Must be non-negative");

            // Act
            var result = await controller.Update(1, book);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequest.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

            var result = await _controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Search_ReturnsMatchingBooks()
        {
            var results = new List<Book>
            {
                new Book { Id = 1, Title = "Book A", Author = "Alice" },
                new Book { Id = 2, Title = "Book B", Author = "Bob" }
            };

            _mockService.Setup(s => s.SearchAsync("book")).ReturnsAsync(results);

            var result = await _controller.Search("book");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public async Task Search_ReturnsEmptyList_WhenNoResults()
        {
            // Arrange: valid query, no results
            _mockService.Setup(s => s.SearchAsync("notfound"))
                .ReturnsAsync(new List<Book>());

            // Act
            var result = await _controller.Search("notfound");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Empty(books);
        }

        [Fact]
        public async Task Search_ReturnsBadRequest_WhenQueryIsEmpty()
        {
            var result = await _controller.Search(""); // empty query

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Query must not be empty", badRequest.Value);
        }
    }
}
