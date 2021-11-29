using H.Necessaire.CLI.Commands.NuGetVersioning.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    class NuSpecDependencyTreeProcessor
    {
        public async Task<OperationResult<NuSpecTree>> ProcessDependencyTreeFor(NuSpecInfo nuSpec, IEnumerable<NuSpecInfo> nuSpecsPool)
        {
            NuSpecTree result = new NuSpecTree
            {
                Root = new NuSpecNode
                {
                    NuSpecInfo = nuSpec,
                    UsedBy = await Task.Run(() => ProcessNodeChildrenFor(nuSpec, nuSpecsPool)),
                },
            };

            return OperationResult.Win().WithPayload(result);
        }

        private NuSpecNode[] ProcessNodeChildrenFor(NuSpecInfo nuSpec, IEnumerable<NuSpecInfo> nuSpecsPool)
        {
            NuSpecInfo[] nuSpecsThatDependOnIt = nuSpecsPool?.Where(dep => nuSpec.ID.In(dep.Dependencies?.Select(x => x.ID))).ToArray() ?? new NuSpecInfo[0];

            if (!nuSpecsThatDependOnIt.Any())
                return new NuSpecNode[0];

            return
                nuSpecsThatDependOnIt
                .Select(x => new NuSpecNode
                {
                    NuSpecInfo = x,
                    UsedBy = ProcessNodeChildrenFor(x, nuSpecsPool ?? new NuSpecInfo[0]),
                })
                .ToArray()
                ;
        }
    }
}
