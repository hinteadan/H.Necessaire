using System;

namespace H.Necessaire.UI
{
    public class UserOption
    {
        public object Payload { get; set; }
        public string Icon { get; set; } = null;
        public string Label { get; set; } = null;
        public object Tag { get; set; }
        public Action<object> OnClick { get; set; } = null;
    }
}
