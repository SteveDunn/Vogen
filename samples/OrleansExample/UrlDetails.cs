namespace OrleansExample;

[GenerateSerializer, Alias(nameof(UrlDetails))]
public sealed record UrlDetails
{
    [Id(0)]
    public required CustomUrl FullUrl { get; init; }

    [Id(1)]
    public required string ShortenedRouteSegment { get; init; }
}