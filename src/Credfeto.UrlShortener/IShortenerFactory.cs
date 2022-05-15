namespace Credfeto.UrlShortener;

public interface IShortenerFactory
{
    IUrlShortener Create(string type);
}