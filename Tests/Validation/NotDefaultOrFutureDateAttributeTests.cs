using BookApi.Validation;

namespace BookApi.Tests.Validation
{
    public class NotDefaultOrFutureDateAttributeTests
    {
        private readonly NotDefaultOrFutureDateAttribute _attribute;

        public NotDefaultOrFutureDateAttributeTests()
        {
            _attribute = new NotDefaultOrFutureDateAttribute();
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenDateIsDefault()
        {
            var result = _attribute.IsValid(default(DateTime));
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenDateIsInFuture()
        {
            var futureDate = DateTime.Now.AddDays(1);
            var result = _attribute.IsValid(futureDate);
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsTrue_WhenDateIsTodayOrPast()
        {
            var validDate = DateTime.Now.AddDays(-1);
            var result = _attribute.IsValid(validDate);
            Assert.True(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenValueIsNotDateTime()
        {
            var result = _attribute.IsValid("not a date");
            Assert.False(result);
        }
    }
}