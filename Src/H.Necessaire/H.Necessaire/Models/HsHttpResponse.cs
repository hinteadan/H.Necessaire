using System.Net;

namespace H.Necessaire
{
    public class HsHttpResponse
    {
        public Note[] Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode < 300;
    }
}
