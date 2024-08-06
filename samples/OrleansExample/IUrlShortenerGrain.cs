namespace OrleansExample;

public interface IUrlShortenerGrain : IGrainWithStringKey
{
    Task SetUrl(CustomUrl fullUrl);
    Task<CustomUrl> GetUrl();
}