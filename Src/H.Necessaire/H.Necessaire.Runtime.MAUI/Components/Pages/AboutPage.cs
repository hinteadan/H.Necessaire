using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Core;

namespace H.Necessaire.Runtime.MAUI.Components.Pages
{
    [Category("Main")]
    class AboutPage : HMauiPageBase
    {
        protected override View ConstructContent()
        {
            View content = null;
            TimeSpan contentBuildDuration = TimeSpan.Zero;
            Label lbl = null;
            using (new PreciseTimeMeasurement(x => contentBuildDuration = x))
            {
                content = new Label().And(x => lbl = x);
            }
            lbl.Text = contentBuildDuration.ToString();

            return content;
        }
    }
}
