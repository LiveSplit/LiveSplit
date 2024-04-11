using System.Collections.Generic;
using Xunit;
using LiveSplit.Model;

namespace LiveSplit.Tests.Model
{
    public class AbbreviationExtensionsMust
    {
        [Theory]
        [MemberData(nameof(AbbreviationsFeeder))]
        public void ReturnAbbreviationsCorrectly(string anyPhrase, string[] expectedAbbreviations)
        {
            var abbreviations = anyPhrase.GetAbbreviations();
            Assert.Equal(expectedAbbreviations, abbreviations);
        }

        public static IEnumerable<object[]> AbbreviationsFeeder()
        {
            yield return new object[] { "", new[] { string.Empty } };
            yield return new object[] { null, new[] { string.Empty } };

            yield return new object[] { "Elephant", new[] { "Elephant" } };

            yield return new object[] { "Elephant (animal)", new[] { "Elephant (animal)", "Elephant" }  };
            yield return new object[] { "  Elephant (animal)  ", new[] { "Elephant (animal)", "Elephant" }  };
            yield return new object[] { "  Elephant  (animal)  ", new[] { "Elephant  (animal)", "Elephant" }  };
            yield return new object[] { "  Elephant  (  animal  )  ", new[] { "Elephant  (  animal  )", "Elephant" }  };

            yield return new object[] { "Elephant: animal", new[] { "Elephant: animal", "animal" } };
            yield return new object[] { "  Elephant: animal  ", new[] { "Elephant: animal", "animal" } };
            yield return new object[] { "  Elephant:  animal  ", new[] { "Elephant:  animal", "Elephant: animal", "animal" } };
            yield return new object[] { "Elephant: animal 1", new[] { "Elephant: animal 1", "Elephant: a1", "animal 1", "a1" } };
            yield return new object[] { "Elephant: animal IX", new[] { "Elephant: animal IX", "Elephant: a IX", "animal IX", "a IX" } };
            yield return new object[] { "Elephant1: animal", new[] { "Elephant1: animal", "Elephant1", "animal" } };
            yield return new object[] { "XV: animal", new[] { "XV: animal", "animal", "XV" } };
            yield return new object[] { "Elephant XI: animal", new[] { "Elephant XI: animal", "E XI: animal", "Elephant XI", "animal", "E XI" } };

            yield return new object[] { "Elephant - animal", new[] { "Elephant - animal", "animal" } };
            yield return new object[] { "  Elephant - animal  ", new[] { "Elephant - animal", "animal" } };
            yield return new object[] { "  Elephant  -  animal  ", new[] { "Elephant  -  animal", "Elephant - animal", "animal" } };
            yield return new object[] { "Elephant -the animal- here", new[] { "Elephant -the animal- here", "Etah" } };
            yield return new object[] { "Elephant - animal 1", new[] { "Elephant - animal 1", "Elephant - a1", "animal 1", "a1" } };
            yield return new object[] { "Elephant - animal IX", new[] { "Elephant - animal IX", "Elephant - a IX", "animal IX", "a IX" } };

            yield return new object[] { "Elephant | animal", new[] { "Elephant | animal" } };
            yield return new object[] { "  Elephant | animal  ", new[] { "Elephant | animal" } };
            yield return new object[] { "  Elephant  |  animal  ", new[] { "Elephant  |  animal", "Elephant | animal" } };

            yield return new object[] { "Elephant the animal", new string[] { "Elephant the animal", "Eta" } };
            yield return new object[] { "Elephant animal", new string[] { "Elephant animal", "Ea" } };
            yield return new object[] { "The elephant is an animal", new string[] { "The elephant is an animal", "elephant is an animal", "Teiaa" } };

            yield return new object[] { "A elephant", new[] { "A elephant", "elephant", "Ae" } };
            yield return new object[] { "  A elephant  ", new[] { "A elephant", "elephant", "Ae" } };
            yield return new object[] { "An elephant", new[] { "An elephant", "Ae" } };
            yield return new object[] { "  An elephant  ", new[] { "An elephant", "Ae" } };

            yield return new object[] { "Elephant and animal", new[] { "Elephant and animal", "Elephant & animal", "Eaa" } };
            yield return new object[] { "Elephant & animal", new[] { "Elephant & animal", "Eaa" } };
            yield return new object[] { "  Elephant and animal  ", new[] { "Elephant and animal", "Elephant & animal", "Eaa" } };

            yield return new object[] { "Elephant, animal 34567 here", new[] { "Elephant, animal 34567 here", "Ea34567h" } };
            yield return new object[] { "Elephant, animal 345 here", new[] { "Elephant, animal 345 here", "Ea345h" } };
            yield return new object[] { "Elephant 13h animal", new[] { "Elephant 13h animal", "E13ha" } };
            yield return new object[] { "Elephant1", new[] { "Elephant1" } };

            yield return new object[] { "Elephant animal XVI", new[] { "Elephant animal XVI", "Ea XVI" } };
            yield return new object[] { "Elephant the animal XVI", new[] { "Elephant the animal XVI", "Eta XVI" } };
            yield return new object[] { "Elephant the animalXVI", new[] { "Elephant the animalXVI", "Eta" } };
        }
    }
}
