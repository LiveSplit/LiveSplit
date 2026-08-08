using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Model.RunFactories;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace LiveSplit.Tests.Model.RunFactories;

public class StandardFormatsRunFactoryMust
{
    private static IRun Parse(string splits)
    {
        LiveSplitCoreFactory.LoadLiveSplitCore();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(splits));
        var factory = new StandardFormatsRunFactory(stream);
        return factory.Create(new StandardComparisonGeneratorsFactory());
    }

    [Fact]
    public void DowngradeNativeSegmentGroupsToLegacySubsplitNames()
    {
        const string splits = """
            <?xml version="1.0" encoding="UTF-8"?>
            <Run version="1.8.1">
              <GameIcon />
              <GameName>Game</GameName>
              <CategoryName>Any%</CategoryName>
              <Offset>00:00:00</Offset>
              <AttemptCount>0</AttemptCount>
              <Segments>
                <Segment><Name>Intro</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
                <Segment><Name>-Literal Dash</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
                <Segment><Name>Opening End</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
                <Segment><Name>Part</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
                <Segment><Name>Finish</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
              </Segments>
              <SegmentGroups>
                <SegmentGroup start="0" end="3"><Name>Opening</Name></SegmentGroup>
                <SegmentGroup start="3" end="5" />
              </SegmentGroups>
              <AutoSplitterSettings />
            </Run>
            """;

        IRun run = Parse(splits);

        Assert.Equal(
            ["-Intro", "--Literal Dash", "{Opening} Opening End", "-Part", "Finish"],
            run.Select(segment => segment.Name));
    }

    [Fact]
    public void PreserveLegacySubsplitNamesAfterCoreUpgradesThem()
    {
        const string splits = """
            <?xml version="1.0" encoding="UTF-8"?>
            <Run version="1.8.0">
              <GameIcon />
              <GameName>Game</GameName>
              <CategoryName>Any%</CategoryName>
              <Offset>00:00:00</Offset>
              <AttemptCount>0</AttemptCount>
              <Segments>
                <Segment><Name>Intro</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
                <Segment><Name>-Part 1</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
                <Segment><Name>-Part 2</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
                <Segment><Name>{Chapter} Finish</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
                <Segment><Name>Outro</Name><Icon /><SplitTimes /><BestSegmentTime /><SegmentHistory /></Segment>
              </Segments>
              <AutoSplitterSettings />
            </Run>
            """;

        IRun run = Parse(splits);

        Assert.Equal(
            ["Intro", "-Part 1", "-Part 2", "{Chapter} Finish", "Outro"],
            run.Select(segment => segment.Name));
    }
}
