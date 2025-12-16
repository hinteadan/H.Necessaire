using H.Necessaire.CLI.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    [ID("copy")]
    [Alias("cpy")]
    internal class CopyCommand : CommandBase
    {
        static readonly string[] usageSyntax = new string[] {
            "copy|cpy folder|dir src=Path/To/Source/Folder dst=Path/To/Destintion/Folder",
            //"help|? [searchKey:string]",
            //"help|? [q=searchKey:string]",
        };
        protected override string[] GetUsageSyntaxes() => usageSyntax;
        public override Task<OperationResult> Run() => RunSubCommand();

        [ID("folder")]
        [Alias("dir")]
        class FolderSubCommand : SubCommandBase
        {
            public override async Task<OperationResult> Run(params Note[] args)
            {
                string src = args.Get("src", ignoreCase: true);
                if (src.IsEmpty())
                    return "src arg must be specified, representing the source folder to be copied";

                string dst = args.Get("dst", ignoreCase: true);
                if (src.IsEmpty())
                    return "dst arg must be specified, representing the destination folder to copy to";

                DirectoryInfo srcFolder = new DirectoryInfo(src);
                if (!srcFolder.Exists)
                    return $"Source folder doesn't exist: {src}";

                DirectoryInfo dstFolder = new DirectoryInfo(dst);
                if (!HSafe.Run(dstFolder.Create).Ref(out var dstCreateRes))
                    return dstCreateRes;

                if (!HSafe.Run(() => srcFolder.CopyTo(dstFolder)).Ref(out var copyRes))
                    return copyRes;

                return true;
            }
        }
    }
}
