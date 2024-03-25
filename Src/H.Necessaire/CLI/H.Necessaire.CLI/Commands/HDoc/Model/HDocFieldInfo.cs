namespace H.Necessaire.CLI.Commands.HDoc.Model
{
    public class HDocFieldInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsStatic { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsConst { get; set; }
        public bool HasDefaultValue { get; set; }
        public string DefaultsTo { get; set; }
    }
}
