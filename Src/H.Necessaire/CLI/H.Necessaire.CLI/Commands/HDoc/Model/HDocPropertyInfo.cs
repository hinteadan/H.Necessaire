namespace H.Necessaire.CLI.Commands.HDoc.Model
{
    public class HDocPropertyInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsReadable { get; set; }
        public bool IsWriteable { get; set; }
        public bool IsVirtual { get; set; }
        public bool HasDefaultValue { get; set; }
        public string DefaultsTo { get; set; }
        public string Expression { get; set; }
    }
}
