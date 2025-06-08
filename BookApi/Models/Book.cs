using System.ComponentModel.DataAnnotations;

namespace BookApi.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MinLength(2)]
        public string Title { get; set; } = string.Empty;

        [Required, MinLength(2)]
        public string Author { get; set; } = string.Empty;

        public DateTime PublicationDate { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
