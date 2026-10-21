
using Xunit;

namespace RaceDay.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("Run")]
        [InlineData("Walk")]
        [InlineData("Cycle")]
        public void SupportedEventTypes_AreRecognised(string eventType)
        {
            bool isSupported =
                eventType == "Run" ||
                eventType == "Walk" ||
                eventType == "Cycle";

            Assert.True(isSupported);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void EmptyEventNames_AreInvalid(string eventName)
        {
            Assert.True(string.IsNullOrWhiteSpace(eventName));
        }

        [Theory]
        [InlineData("Swim")]
        [InlineData("Football")]
        [InlineData("Unknown")]
        public void UnsupportedEventTypes_AreRejected(string eventType)
        {
            bool isSupported =
                eventType == "Run" ||
                eventType == "Walk" ||
                eventType == "Cycle";

            Assert.False(isSupported);
        }
    }
}
