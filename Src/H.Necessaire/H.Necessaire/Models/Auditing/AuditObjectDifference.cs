namespace H.Necessaire
{
    public class AuditObjectDifference
    {
        public string PropertyNameAndPath { get; set; }

        public object Preceding { get; set; }
        public string PrecedingValue { get; set; }

        public object Following { get; set; }
        public string FollowingValue { get; set; }
    }
}
