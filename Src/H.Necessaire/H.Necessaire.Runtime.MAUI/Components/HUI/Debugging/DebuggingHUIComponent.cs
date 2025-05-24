using H.Necessaire.Runtime.UI;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Components.HUI.Debugging
{
    class DebuggingHUIComponent : HUIComponentBase
    {
        public DebuggingHUIComponent()
            : base(
                new DebuggingDataModel()
                .ToHViewModel(title: "HUI Controls Debugger")
                .And(vm =>
                {
                    vm.Property(x => x.Selection).SetPresentation(new HUIPresentationInfo
                    {
                        Type = HUIPresentationType.Selection,
                        Selection = new HUISelectionPresentationInfo
                        {
                            Options = ["Selection Option 1", "Selection Option 2", "Selection Option 3", "Selection Option 4"],
                        },
                    });
                })
            )
        {
        }
    }
}
