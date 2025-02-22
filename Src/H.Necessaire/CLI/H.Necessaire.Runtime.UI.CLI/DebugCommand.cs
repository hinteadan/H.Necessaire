using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.UI;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.UI.CLI
{
    internal class DebugCommand : CommandBase
    {
        ImAnHUIComponent loginComponent;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);

            loginComponent = dependencyProvider.Build<ImAnHUIComponent>("login");
        }

        public override async Task<OperationResult> Run()
        {
            //var data = loginComponent.ViewModel.As<LoginCommand>();

            //foreach (var property in loginComponent.ViewModel.Properties)
            //{
            //    property.Value = (await property.Label.CliUiAskForAnyInput<string>(isSecret: property.ID.Is("password"))).ThrowOnFailOrReturn();
            //}


            var viewModel = new HViewModel<DateTime>().WithData(DateTime.Today);

            HViewModelProperty prop = viewModel.Property(x => x.DayOfWeek);

            await Task.Delay(1000);

            viewModel.OnViewModelChanged += async (sender, args) => {
                await Task.CompletedTask;
            };

            viewModel.WithData(DateTime.Today.AddSeconds(1));

            Console.ReadLine();

            return OperationResult.Win();
        }
    }
}
