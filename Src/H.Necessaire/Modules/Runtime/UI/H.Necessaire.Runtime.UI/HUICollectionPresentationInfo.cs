using System;

namespace H.Necessaire.Runtime.UI
{
    public class HUICollectionPresentationInfo
    {
        public Type ElementType { get; set; } = typeof(object);
        public HUIPresentationInfo ElementPresentationInfo { get; set; } = null;
    }
}