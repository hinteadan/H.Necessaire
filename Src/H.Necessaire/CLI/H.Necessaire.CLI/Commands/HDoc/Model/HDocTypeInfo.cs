namespace H.Necessaire.CLI.Commands.HDoc.Model
{
    public class HDocTypeInfo : IStringIdentity
    {
        public string ID { get; set; }
        public string Module { get; set; }
        public string[] FilePath { get; set; }
        public string[] FolderPath { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Namespace { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsInterface { get; set; }
        public bool IsSealed { get; set; }

        public HDocConstructorInfo[] Constructors { get; set; }
        public HDocFieldInfo[] Fields { get; set; }
        public HDocPropertyInfo[] Properties { get; set; }
        public HDocMethodInfo[] Methods { get; set; }


        public string[] InheritedTypeNames { get; set; }
    }
}
