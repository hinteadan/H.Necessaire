using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.HUI.Components
{
    class HEmptyViewModelComponent : HMauiComponentBase
    {
        HViewModel viewModel;
        public HEmptyViewModelComponent(HViewModel viewModel) : base(viewModel) { }
        protected override void EnsureDependencies(params object[] constructionArgs)
        {
            base.EnsureDependencies(constructionArgs);

            viewModel = constructionArgs[0] as HViewModel;
        }

        protected override View ConstructContent()
        {
            return
                new VerticalStackLayout()
                .And(layout =>
                {
                    layout.Add(new HLabel
                    {
                        FontSize = Branding.Typography.FontSizeLarger,
                        Text = viewModel.Title.IsEmpty() ? viewModel.ID : viewModel.Title,
                    });

                    if (!viewModel.Description.IsEmpty())
                    {
                        layout.Add(new HLabel
                        {
                            Text = viewModel.Description,
                        });
                    }
                });
        }
    }
}
