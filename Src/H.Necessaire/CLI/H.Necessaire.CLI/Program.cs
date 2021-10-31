// See https://aka.ms/new-console-template for more information

using H.Necessaire.CLI;
using H.Necessaire.Runtime.CLI;

await
    new CliApp()
    .WithEverything()
    .Run(askForCommandIfEmpty: true)
    ;
