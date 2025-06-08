using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _service;

        public BooksController(BookService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page and pageSize must be positive integers");

            var books = await _service.GetAllAsync(page, pageSize);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (book.PublicationDate == default || book.PublicationDate > DateTime.Now)
                return BadRequest("Non-future Publication date is required.");

            var created = await _service.AddAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (book.PublicationDate == default || book.PublicationDate > DateTime.Now)
                return BadRequest("Non-future Publication date is required.");

            var success = await _service.UpdateAsync(id, book);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is empty");

            var results = await _service.SearchAsync(query);
            return Ok(results);
        }
    }
}

