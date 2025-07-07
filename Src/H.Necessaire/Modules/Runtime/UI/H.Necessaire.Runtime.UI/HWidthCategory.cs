using System;
using System.Linq;

namespace H.Necessaire.Runtime.UI
{
    public class HWidthCategory : IStringIdentity
    {
        public HWidthCategory(string id, NumberInterval interval)
        {
            ID = id;
            Interval = interval;
        }

        public string ID { get; }
        public NumberInterval Interval { get; }

        public static readonly HWidthCategory XSmall = new HWidthCategory(nameof(XSmall), new NumberInterval(null, 576, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HWidthCategory Small = new HWidthCategory(nameof(Small), new NumberInterval(576, 768, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HWidthCategory Medium = new HWidthCategory(nameof(Medium), new NumberInterval(768, 992, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HWidthCategory Large = new HWidthCategory(nameof(Large), new NumberInterval(992, 1200, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HWidthCategory XLarge = new HWidthCategory(nameof(XLarge), new NumberInterval(1200, 1400, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HWidthCategory XXLarge = new HWidthCategory(nameof(XXLarge), new NumberInterval(1400, null, isMinIncluded: true, isMaxIncluded: true));

        public static readonly HWidthCategory[] All = new HWidthCategory[] { XSmall, Small, Medium, Large, XLarge, XXLarge };
    }

    public class HWidthCategoryAction
    {
        public HWidthCategoryAction(HWidthCategory[] categories, Action<HWidthCategory[]> action)
        {
            Categories = categories;
            Action = action;
        }
        public HWidthCategory[] Categories { get; }
        public Action<HWidthCategory[]> Action { get; }
    }

    public static class HWidthCategoryExtensions
    {
        public static HWidthCategoryAction WithAction(this HWidthCategory[] categories, Action<HWidthCategory[]> action)
            => new HWidthCategoryAction(categories, action);

        public static HWidthCategoryAction WithAction(this HWidthCategory category, Action<HWidthCategory[]> action)
            => new HWidthCategoryAction(category.AsArray(), action);

        public static HWidthCategory GetWidthCategory(this double value)
            => HWidthCategory.All.Single(x => value.In(x.Interval));

        public static void OnWidthCategory(this double value, Action defaultAction = null, params HWidthCategoryAction[] actions)
        {
            if (actions.IsEmpty())
            {
                if (!(defaultAction is null))
                    defaultAction.Invoke();

                return;
            }

            foreach (HWidthCategoryAction categoryAction in actions)
            {
                if ((categoryAction?.Categories).IsEmpty())
                    continue;

                if (categoryAction.Categories.All(cat => !value.In(cat.Interval)))
                    continue;

                if (!(categoryAction.Action is null))
                    categoryAction.Action.Invoke(categoryAction.Categories);

                return;
            }

            if (!(defaultAction is null))
                defaultAction.Invoke();
        }

        public static void OnWidthCategory(this double value, params HWidthCategoryAction[] actions)
            => value.OnWidthCategory(defaultAction: null, actions);
    }
}
