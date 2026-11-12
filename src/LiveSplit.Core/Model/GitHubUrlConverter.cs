using System;

namespace LiveSplit.Model;

public static class GitHubUrlConverter
{
    private const string JsDelivrPrefix = "https://cdn.jsdelivr.net/gh/";

    public static string ToJsDelivr(string url)
    {
        if (string.IsNullOrWhiteSpace(url)
            || !Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return url;
        }

        if (uri.Host.Equals("raw.githubusercontent.com", StringComparison.OrdinalIgnoreCase))
        {
            return ConvertRawGitHubUrl(uri) ?? url;
        }

        if (uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("www.github.com", StringComparison.OrdinalIgnoreCase))
        {
            return ConvertGitHubUrl(uri) ?? url;
        }

        return url;
    }

    private static string ConvertRawGitHubUrl(Uri uri)
    {
        string[] segments = GetPathSegments(uri);
        if (segments.Length < 4)
        {
            return null;
        }

        return CreateJsDelivrUrl(uri, segments, 0, 1, 2, 3);
    }

    private static string ConvertGitHubUrl(Uri uri)
    {
        string[] segments = GetPathSegments(uri);
        if (segments.Length < 5)
        {
            return null;
        }

        string action = segments[2];
        if (!action.Equals("raw", StringComparison.OrdinalIgnoreCase)
            && !action.Equals("blob", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return CreateJsDelivrUrl(uri, segments, 0, 1, 3, 4);
    }

    private static string CreateJsDelivrUrl(
        Uri uri,
        string[] segments,
        int ownerIndex,
        int repositoryIndex,
        int referenceIndex,
        int pathIndex)
    {
        (string reference, int contentIndex) = GetReference(segments, referenceIndex);
        if (string.IsNullOrEmpty(reference) || contentIndex >= segments.Length)
        {
            return null;
        }

        string owner = segments[ownerIndex];
        string repository = segments[repositoryIndex];
        string contentPath = string.Join("/", segments, contentIndex, segments.Length - contentIndex);
        return string.Concat(
            JsDelivrPrefix,
            owner,
            "/",
            repository,
            "@",
            reference,
            "/",
            contentPath,
            uri.Query);
    }

    private static (string Reference, int ContentIndex) GetReference(string[] segments, int referenceIndex)
    {
        if (segments[referenceIndex].Equals("refs", StringComparison.OrdinalIgnoreCase)
            && referenceIndex + 2 < segments.Length
            && (segments[referenceIndex + 1].Equals("heads", StringComparison.OrdinalIgnoreCase)
                || segments[referenceIndex + 1].Equals("tags", StringComparison.OrdinalIgnoreCase)))
        {
            return (segments[referenceIndex + 2], referenceIndex + 3);
        }

        return (segments[referenceIndex], referenceIndex + 1);
    }

    private static string[] GetPathSegments(Uri uri)
    {
        return uri.AbsolutePath.Trim('/').Split('/');
    }
}