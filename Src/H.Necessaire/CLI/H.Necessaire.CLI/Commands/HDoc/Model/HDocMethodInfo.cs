namespace H.Necessaire.CLI.Commands.HDoc.Model
{
    public class HDocMethodInfo
    {
        public string Name { get; set; }
        public bool IsStatic { get; set; }
        public bool IsVirtual { get; set; }
        public string ReturnType { get; set; }
        public HDocParameterInfo[] Parameters { get; set; }
    }
}
