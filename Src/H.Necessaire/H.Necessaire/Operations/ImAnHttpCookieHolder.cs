using System.Net;

namespace H.Necessaire
{
    public interface ImAnHttpCookieHolder
    {
        CookieContainer CookieContainer { get; }
    }
}
