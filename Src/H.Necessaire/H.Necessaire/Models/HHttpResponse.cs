namespace H.Necessaire
{
    public class HHttpResponse
    {
        public Note[] Headers { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;
    }
}
