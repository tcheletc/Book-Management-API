using System.ComponentModel.DataAnnotations;

namespace BookApi.Validation
{
    public class NotDefaultOrFutureDateAttribute : ValidationAttribute
    {
        public NotDefaultOrFutureDateAttribute()
        {
            ErrorMessage = "The date must be valid (not empty or in the future)";
        }

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
