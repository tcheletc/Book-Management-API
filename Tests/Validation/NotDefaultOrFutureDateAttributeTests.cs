using BookApi.Validation;

namespace BookApi.Tests.Validation
{
    /// <summary>
    /// Unit tests for the <see cref="NotDefaultOrFutureDateAttribute"/> validation attribute.
    /// These tests verify that the attribute correctly validates DateTime values,
    /// rejecting default or future dates and accepting valid past or present dates.
    /// </summary>
    public class NotDefaultOrFutureDateAttributeTests
    {
        private readonly NotDefaultOrFutureDateAttribute _attribute;

        public NotDefaultOrFutureDateAttributeTests()
        {
            _attribute = new NotDefaultOrFutureDateAttribute();
        }

        /// <summary>
        /// Verifies that IsValid returns false when the date is the default value (DateTime.MinValue).
        /// </summary>
        [Fact]
        public void IsValid_ReturnsFalse_WhenDateIsDefault()
        {
            var result = _attribute.IsValid(default(DateTime));
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that IsValid returns false when the date is in the future.
        /// </summary>
        [Fact]
        public void IsValid_ReturnsFalse_WhenDateIsInFuture()
        {
            var futureDate = DateTime.Now.AddDays(1);
            var result = _attribute.IsValid(futureDate);
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that IsValid returns true when the date is today or in the past.
        /// </summary>
        [Fact]
        public void IsValid_ReturnsTrue_WhenDateIsTodayOrPast()
        {
            var validDate = DateTime.Now.AddDays(-1);
            var result = _attribute.IsValid(validDate);
            Assert.True(result);
        }

        /// <summary>
        /// Verifies that IsValid returns false when the input value is not a DateTime instance.
        /// </summary>
        [Fact]
        public void IsValid_ReturnsFalse_WhenValueIsNotDateTime()
        {
            var result = _attribute.IsValid("not a date");
            Assert.False(result);
        }
    }
}