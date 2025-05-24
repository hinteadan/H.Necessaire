using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.UI.Concrete
{
    [ID("login")]
    public class HUILoginComponent : HUIComponentBase
    {
        public HUILoginComponent() : base(new LoginCommand().ToHViewModel())
        {
        }

        public HUILoginComponent(string title) : base(new LoginCommand().ToHViewModel(title))
        {
        }
        public HUILoginComponent(string title, string description) : base(new LoginCommand().ToHViewModel(title, description))
        {
        }

        public HUILoginComponent(string title, string description, params Note[] notes) : base(new LoginCommand().ToHViewModel(title, description, notes))
        {
        }
    }
}
