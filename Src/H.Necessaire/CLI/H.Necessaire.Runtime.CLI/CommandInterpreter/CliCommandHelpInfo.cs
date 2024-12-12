using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.CLI.CommandInterpreter
{
    internal class CliCommandHelpInfo
    {
        public Type ConcreteType { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public string[] Aliases { get; set; }
        public string[] Categories { get; set; }
        public string[] UsageSyntaxes { get; set; }

        public string GetPreferredCommandSyntax()
        {
            return GetAllCommandSyntaxes(isFullTypeNameIncluded: true).First();
        }

        public string[] GetAllCommandSyntaxes(bool isFullTypeNameIncluded = false)
        {
            List<string> allSyntaxes = new List<string>();

            if (!ID.IsEmpty())
                allSyntaxes.Add(ID);

            if (Aliases != null)
                allSyntaxes.AddRange(Aliases.Where(a => !a.IsEmpty()));

            if (!Name.IsEmpty())
                allSyntaxes.Add(Name);

            if (isFullTypeNameIncluded && !Name.Is(ConcreteType.Name))
                allSyntaxes.Add(ConcreteType.Name);

            return allSyntaxes.ToArray();
        }

        public override string ToString()
        {
            return $"{string.Join(" | ", GetAllCommandSyntaxes())} [{ConcreteType.TypeName()}]";
        }
    }
}
