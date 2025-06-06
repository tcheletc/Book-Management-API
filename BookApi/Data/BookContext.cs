using Microsoft.EntityFrameworkCore;
using BookApi.Models;

namespace BookApi.Data
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();
    }
}
