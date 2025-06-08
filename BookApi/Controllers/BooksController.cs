using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _service;

        public BooksController(IBookService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of books.
        /// </summary>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>A paginated list of books.</returns>
        /// <response code="200">Returns the list of books</response>
        /// <response code="400">If page or pageSize are invalid</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page and pageSize must be positive integers");

            var books = await _service.GetAllAsync(page, pageSize);
            return Ok(books);
        }

        /// <summary>
        /// Retrieves a book by its ID.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>The book if found.</returns>
        /// <response code="200">Returns the book</response>
        /// <response code="404">If the book is not found</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="book">The book to create.</param>
        /// <returns>The created book.</returns>
        /// <response code="201">Returns the newly created book</response>
        /// <response code="400">If the model is invalid</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.AddAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="book">The updated book data.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the update was successful</response>
        /// <response code="400">If the model is invalid</response>
        /// <response code="404">If the book is not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _service.UpdateAsync(id, book);
            return success ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deletes a book by its ID.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>No content if deleted.</returns>
        /// <response code="204">If the deletion was successful</response>
        /// <response code="404">If the book is not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        /// <summary>
        /// Searches for books matching a query string.
        /// </summary>
        /// <param name="query">The search term.</param>
        /// <returns>A list of matching books.</returns>
        /// <response code="200">Returns the search results</response>
        /// <response code="400">If the query is empty</response>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query must not be empty");

            var results = await _service.SearchAsync(query);
            return Ok(results);
        }
    }
}

