namespace H.Necessaire.Runtime
{
    public class UseCaseFailContext
    {
        public int ReasonCode { get; set; } = 500; // InternalServerError;
        public string ReasonPhrase { get; set; }
        public Note[] Notes { get; set; }
    }
}
