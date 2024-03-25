namespace H.Necessaire.CLI.Commands.HDoc.Model
{
    public class HDocMethodInfo
    {
        public string Name { get; set; }
        public bool IsStatic { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsProtected { get; set; }
        public string ReturnType { get; set; }
        public HDocParameterInfo[] Parameters { get; set; }
    }
}
