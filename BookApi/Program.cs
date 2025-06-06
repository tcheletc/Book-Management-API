using BookApi.Data;
using BookApi.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);

// add DbContext with SQLite
builder.Services.AddDbContext<BookContext>(options =>
    options.UseSqlite("Data Source=books.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Minimal APIs
app.MapGet("/api/books", async (BookContext db, int page = 1, int pageSize = 10) =>
{
    if (page <= 0 || pageSize <= 0)
        return Results.BadRequest("Page and pageSize must be positive integers.");
    
    var books = await db.Books
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    return Results.Ok(books);
});

app.MapGet("/api/books/{id}", async (BookContext db, int id) =>
    await db.Books.FindAsync(id) is Book book
        ? Results.Ok(book)
        : Results.NotFound());

app.MapPost("/api/books", async (BookContext db, Book book) =>
{
    // Validity check – are all fields filled in?
    if (string.IsNullOrWhiteSpace(book.Title) ||
        string.IsNullOrWhiteSpace(book.Author) ||
        book.PublicationDate == default ||
        book.Price <= 0)
    {
        return Results.BadRequest("All fields must be filled in with valid values.");
    }

    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/api/books/{book.Id}", book);
});

app.MapPut("/api/books/{id}", async (BookContext db, int id, Book input) =>
{
    // Validity check – are all fields filled in?
    if (string.IsNullOrWhiteSpace(input.Title) ||
        string.IsNullOrWhiteSpace(input.Author) ||
        input.PublicationDate == default ||
        input.Price <= 0)
    {
        return Results.BadRequest("All fields must be filled in with valid values.");
    }

    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    book.Title = input.Title;
    book.Author = input.Author;
    book.PublicationDate = input.PublicationDate;
    book.Price = input.Price;

    await db.SaveChangesAsync();
    return Results.Ok(book);
});

app.MapDelete("/api/books/{id}", async (BookContext db, int id) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/api/books/search", async (BookContext db, string? q) =>
{
    if (string.IsNullOrWhiteSpace(q))
        return Results.BadRequest("Query is empty");

    //Case-insensitive search
    var loweredQuery = q.ToLower();

    var results = await db.Books
        .Where(b => 
            b.Title.ToLower().Contains(loweredQuery) || 
            b.Author.ToLower().Contains(loweredQuery))
        .ToListAsync();

    return Results.Ok(results);
});

app.Run();
