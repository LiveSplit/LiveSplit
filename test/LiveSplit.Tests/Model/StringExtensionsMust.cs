using Xunit;
using LiveSplit.Model;
using System.Net.NetworkInformation;

namespace LiveSplit.Tests.Model
{
    public class StringExtensionsMust
    {
        [Fact]
        public void EscapeAmpersandCorrectly()
        {
            const string fileMenu = "&File";
            var sut = fileMenu.EscapeMenuItemText();
            Assert.Equal("&&File", sut);
        }

        [Theory]
        [InlineData(null, null, 0)]
        [InlineData(null, "", 0)]
        [InlineData("", null, 0)]
        [InlineData("", "", 0)]
        [InlineData(null, "elephant", 8)]
        [InlineData("", "elephant", 8)]
        [InlineData("elephant", null, 8)]
        [InlineData("elephant", "", 8)]
        [InlineData("Elephant", "Elephant", 0)]
        [InlineData("Elephant", "elephant", 0)]
        [InlineData("elephant", "Elephant", 0)]
        [InlineData("Lion", "Elephant", 6)]
        [InlineData("Cat", "Hen", 3)]
        public void CalculateSimilarityCorrectly(string word, string other, int expectedSimilarity) =>
            Assert.Equal(expectedSimilarity, word.Similarity(other));

        [Fact]
        public void OrderWordsBySimilarityCorrectly()
        {
            string[] words = { "rabbit", "elephant", "hen", "Elephant", "cat", "Lion" };
            var result = words.OrderBySimilarityTo("dog");
            Assert.Collection(result,
                p1 => Assert.Equal("hen", p1),
                p2 => Assert.Equal("cat", p2),
                p3 => Assert.Equal("Lion", p3),
                p4 => Assert.Equal("rabbit", p4),
                p5 => Assert.Equal("elephant", p5),
                p6 => Assert.Equal("Elephant", p6));
        }

        [Fact]
        public void FindMostSimilarValueToWordCorrectly()
        {
            string[] words = { "rabbit", "elephant", "hen", "Elephant", "cat", "Lion" };
            var result = words.FindMostSimilarValueTo("dog");
            Assert.Equal("hen", result);
        }
    }
}
