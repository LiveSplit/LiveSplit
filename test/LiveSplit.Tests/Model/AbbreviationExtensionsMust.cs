using LiveSplit.Model;
using System.Collections.Generic;
using Xunit;

namespace LiveSplit.Tests.Model;

public class AbbreviationExtensionsMust
{
    [Theory]
    [MemberData(nameof(AbbreviationsFeeder))]
    public void ReturnAbbreviationsCorrectly(string anyPhrase, string[] expectedAbbreviations)
    {
        IEnumerable<string> abbreviations = anyPhrase.GetAbbreviations();
        Assert.Equal(expectedAbbreviations, abbreviations);
    }

    public static IEnumerable<object[]> AbbreviationsFeeder()
    {
        yield return ["", new[] { string.Empty }];
        yield return [null, new[] { string.Empty }];

        yield return ["Elephant", new[] { "Elephant" }];

        yield return ["Elephant (animal)", new[] { "Elephant (animal)", "Elephant" }];
        yield return ["  Elephant (animal)  ", new[] { "Elephant (animal)", "Elephant" }];
        yield return ["  Elephant  (animal)  ", new[] { "Elephant  (animal)", "Elephant" }];
        yield return ["  Elephant  (  animal  )  ", new[] { "Elephant  (  animal  )", "Elephant" }];

        yield return ["Elephant: animal", new[] { "Elephant: animal", "animal" }];
        yield return ["  Elephant: animal  ", new[] { "Elephant: animal", "animal" }];
        yield return ["  Elephant:  animal  ", new[] { "Elephant:  animal", "Elephant: animal", "animal" }];
        yield return ["Elephant: animal 1", new[] { "Elephant: animal 1", "Elephant: a1", "animal 1", "a1" }];
        yield return ["Elephant: animal IX", new[] { "Elephant: animal IX", "Elephant: a IX", "animal IX", "a IX" }];
        yield return ["Elephant1: animal", new[] { "Elephant1: animal", "Elephant1", "animal" }];
        yield return ["XV: animal", new[] { "XV: animal", "animal", "XV" }];
        yield return ["Elephant XI: animal", new[] { "Elephant XI: animal", "E XI: animal", "Elephant XI", "animal", "E XI" }];

        yield return ["Elephant - animal", new[] { "Elephant - animal", "animal" }];
        yield return ["  Elephant - animal  ", new[] { "Elephant - animal", "animal" }];
        yield return ["  Elephant  -  animal  ", new[] { "Elephant  -  animal", "Elephant - animal", "animal" }];
        yield return ["Elephant -the animal- here", new[] { "Elephant -the animal- here", "Etah" }];
        yield return ["Elephant - animal 1", new[] { "Elephant - animal 1", "Elephant - a1", "animal 1", "a1" }];
        yield return ["Elephant - animal IX", new[] { "Elephant - animal IX", "Elephant - a IX", "animal IX", "a IX" }];

        yield return ["Elephant | animal", new[] { "Elephant | animal" }];
        yield return ["  Elephant | animal  ", new[] { "Elephant | animal" }];
        yield return ["  Elephant  |  animal  ", new[] { "Elephant  |  animal", "Elephant | animal" }];

        yield return ["Elephant the animal", new[] { "Elephant the animal", "Eta" }];
        yield return ["Elephant animal", new[] { "Elephant animal", "Ea" }];
        yield return ["The elephant is an animal", new[] { "The elephant is an animal", "elephant is an animal", "Teiaa" }];

        yield return ["A elephant", new[] { "A elephant", "elephant", "Ae" }];
        yield return ["  A elephant  ", new[] { "A elephant", "elephant", "Ae" }];
        yield return ["An elephant", new[] { "An elephant", "Ae" }];
        yield return ["  An elephant  ", new[] { "An elephant", "Ae" }];

        yield return ["Elephant and animal", new[] { "Elephant and animal", "Elephant & animal", "Eaa" }];
        yield return ["Elephant & animal", new[] { "Elephant & animal", "Eaa" }];
        yield return ["  Elephant and animal  ", new[] { "Elephant and animal", "Elephant & animal", "Eaa" }];

        yield return ["Elephant, animal 34567 here", new[] { "Elephant, animal 34567 here", "Ea34567h" }];
        yield return ["Elephant, animal 345 here", new[] { "Elephant, animal 345 here", "Ea345h" }];
        yield return ["Elephant 13h animal", new[] { "Elephant 13h animal", "E13ha" }];
        yield return ["Elephant1", new[] { "Elephant1" }];

        yield return ["Elephant animal XVI", new[] { "Elephant animal XVI", "Ea XVI" }];
        yield return ["Elephant the animal XVI", new[] { "Elephant the animal XVI", "Eta XVI" }];
        yield return ["Elephant the animalXVI", new[] { "Elephant the animalXVI", "Eta" }];
    }
}
