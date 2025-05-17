using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Builders;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Components.Layouts;
using H.Necessaire.Runtime.UI;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Components.HUI.Components
{
    class HUIDefaultComponent : HMauiComponentBase
    {
        ImAnHUIComponent hUIComponent;
        ImAHMauiHUIPropertyComponentBuilder propertyComponentBuilder;
        public HUIDefaultComponent(ImAnHUIComponent hUIComponent) : base(hUIComponent) { }
        protected override void EnsureDependencies(params object[] constructionArgs)
        {
            base.EnsureDependencies(constructionArgs);

            hUIComponent = constructionArgs[0] as ImAnHUIComponent;
            propertyComponentBuilder = Get<ImAHMauiHUIPropertyComponentBuilder>();
        }

        HViewModel ViewModel => hUIComponent.ViewModel;

        protected override View ConstructContent()
        {
            return
                new HResponsiveFormLayout()
                .And(layout =>
                {
                    layout.Add(new HLabel
                    {
                        FontSize = Branding.Typography.FontSizeLarger,
                        Text = ViewModel.Title.IsEmpty() ? ViewModel.ID : ViewModel.Title,
                    });

                    if (!ViewModel.Description.IsEmpty())
                    {
                        layout.Add(new HLabel
                        {
                            Text = ViewModel.Description,
                        });
                    }

                    foreach (HViewModelProperty prop in ViewModel.Properties.OrderBy(p => p.Priority))
                    {
                        layout.Add(propertyComponentBuilder.BuildComponentForProperty(prop));
                    }
                });
        }
    }
}
