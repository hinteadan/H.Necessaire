using System.Linq;

namespace H.Necessaire.CLI.Commands.NuGetVersioning.Models
{
    class NuSpecNode
    {
        public NuSpecInfo NuSpecInfo { get; set; }

        public NuSpecNode[] UsedBy { get; set; } = new NuSpecNode[0];

        public override string ToString()
        {
            return $"{NuSpecInfo} -> {UsedBy.Length}:({string.Join(" ; ", UsedBy.Select(x => x.ToString()))})";
        }
    }
}
