using BookApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookApi.Models
{
    /// <summary>
    /// Represents a book with basic bibliographic information.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// The unique identifier of the book.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The title of the book. Minimum length is 2 characters.
        /// </summary>
        [Required, MinLength(2)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The author's full name. Minimum length is 2 characters.
        /// </summary>
        [Required, MinLength(2)]
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// The date the book was published. Must not be a future date.
        /// </summary>
        [NotDefaultOrFutureDate]
        public DateTime PublicationDate { get; set; }

        /// <summary>
        /// The price of the book. Must be zero or a positive value.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Price must be zero or positive.")]
        public decimal Price { get; set; }
    }
}
