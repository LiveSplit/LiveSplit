using LiveSplit.Model;
using Xunit;

namespace LiveSplit.Tests.AutoSplitterTests;

public class GitHubUrlConverterMust
{
    [Theory]
    [InlineData(
        "https://raw.githubusercontent.com/owner/repository/main/autosplitter.asl",
        "https://cdn.jsdelivr.net/gh/owner/repository@main/autosplitter.asl")]
    [InlineData(
        "https://raw.githubusercontent.com/owner/repository/refs/heads/main/directory/autosplitter.asl",
        "https://cdn.jsdelivr.net/gh/owner/repository@main/directory/autosplitter.asl")]
    [InlineData(
        "https://github.com/owner/repository/raw/0123456789abcdef/autosplitter.asl",
        "https://cdn.jsdelivr.net/gh/owner/repository@0123456789abcdef/autosplitter.asl")]
    [InlineData(
        "https://github.com/owner/repository/blob/main/directory/autosplitter.asl?raw=1",
        "https://cdn.jsdelivr.net/gh/owner/repository@main/directory/autosplitter.asl?raw=1")]
    public void ConvertGitHubDownloadUrlsToJsDelivr(string input, string expected)
    {
        Assert.Equal(expected, GitHubUrlConverter.ToJsDelivr(input));
    }

    [Fact]
    public void ConvertJsDelivrUrlsBackToRawGitHubUrlsForUpdateFallback()
    {
        const string jsDelivrUrl = "https://cdn.jsdelivr.net/gh/gudqs7/Black-Myth-Wukong-Autosplitter@main/Components/BlackMythWukong.asl";

        Assert.Equal(
            "https://raw.githubusercontent.com/gudqs7/Black-Myth-Wukong-Autosplitter/main/Components/BlackMythWukong.asl",
            GitHubUrlConverter.ToRawGitHubUrl(jsDelivrUrl));
    }

    [Fact]
    public void PreserveQueryStringsWhenConvertingBackToRawGitHubUrls()
    {
        const string jsDelivrUrl = "https://cdn.jsdelivr.net/gh/owner/repository@main/directory/autosplitter.asl?raw=1";

        Assert.Equal(
            "https://raw.githubusercontent.com/owner/repository/main/directory/autosplitter.asl?raw=1",
            GitHubUrlConverter.ToRawGitHubUrl(jsDelivrUrl));
    }

    [Theory]
    [InlineData("https://github.com/owner/repository/releases/download/v1/autosplitter.asl")]
    [InlineData("https://gist.githubusercontent.com/owner/12345/raw/autosplitter.asl")]
    [InlineData("https://example.com/autosplitter.asl")]
    [InlineData("not a URL")]
    [InlineData(null)]
    public void LeaveUnsupportedUrlsUnchanged(string input)
    {
        Assert.Equal(input, GitHubUrlConverter.ToJsDelivr(input));
    }
}