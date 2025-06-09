using System.ComponentModel.DataAnnotations;

namespace BookApi.Validation
{
    /// <summary>
    /// Validation attribute that checks if a DateTime value is neither the default value nor in the future.
    /// </summary>
    public class NotDefaultOrFutureDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes the validation attribute with a default error message.
        /// </summary>
        public NotDefaultOrFutureDateAttribute()
        {
            ErrorMessage = "The date must be valid (not empty or in the future)";
        }

        /// <summary>
        /// Determines whether the specified value is a valid date (i.e., not default and not in the future).
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>True if the value is a valid date; otherwise, false.</returns>
        public override bool IsValid(object? value)
        {
            if (value is not DateTime date)
                return false;

            if (date == default || date > DateTime.Now)
                return false;

            return true;
        }
    }
}
