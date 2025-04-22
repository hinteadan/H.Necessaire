using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Common
{
    internal class CustomizableCliContextProvider : ImAUseCaseContextProvider
    {
        private readonly ImAUseCaseContextProvider baseUseCaseContextProvider;

        public CustomizableCliContextProvider(ImAUseCaseContextProvider baseUseCaseContextProvider)
        {
            this.baseUseCaseContextProvider = baseUseCaseContextProvider;
        }

        public async Task<UseCaseContext> GetCurrentContext()
        {
            UseCaseContext context = await baseUseCaseContextProvider.GetCurrentContext();

            Note[] customArgs = CallContext<Note[]>.GetData(CustomizableCliContextScope.CallContextID)?.Where(n => !n.IsEmpty()).ToArray();
            if (customArgs?.Any() == true)
            {
                context.Notes = customArgs;
            }

            return context;
        }

        public static IDisposable WithArgs(params Note[] args) => new CustomizableCliContextScope(args);




        class CustomizableCliContextScope : ScopedRunner
        {
            public const string CallContextID = "CustomArgsForCustomizableCliContextScope";

            public CustomizableCliContextScope(params Note[] args)
                : base(
                    onStart: () =>
                    {
                        CallContext<Note[]>.SetData(CallContextID, args);
                    },
                    onStop: () =>
                    {
                        CallContext<Note[]>.ZapData(CallContextID);
                    }
                )
            {
            }
        }
    }
}
