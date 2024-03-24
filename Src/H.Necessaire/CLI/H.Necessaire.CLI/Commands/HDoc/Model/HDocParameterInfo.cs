namespace H.Necessaire.CLI.Commands.HDoc.Model
{
    public class HDocParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsTheExtensionMethodValue { get; set; }
        public bool IsParamsArray { get; set; }
        public bool IsOutput { get; set; }
        public bool IsOptional { get; set; }
        public string DefaultsTo { get; set; }
    }
}
