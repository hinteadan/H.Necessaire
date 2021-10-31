using System;

namespace H.Necessaire.Runtime
{
    public class UseCaseContext : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public DateTime AsOf { get; set; } = DateTime.Now;
        public SecurityContext SecurityContext { get; set; }
        public UseCaseFailContext FailContext { get; set; }
        public Note[] Notes { get; set; } = new Note[0];
    }
}
