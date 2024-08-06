namespace OrleansExample;

public class UrlShortenerGrain(
    [PersistentState(stateName: "url", storageName: "urls")]
    IPersistentState<UrlDetails> state
) : Grain, IUrlShortenerGrain
{
    public async Task SetUrl(CustomUrl fullUrl)
    {
        state.State = new()
        {
            ShortenedRouteSegment = this.GetPrimaryKeyString(),
            FullUrl = fullUrl
        };

        await state.WriteStateAsync();
    }

    public Task<CustomUrl> GetUrl()
    {
        return Task.FromResult(state.State.FullUrl);
    }
}