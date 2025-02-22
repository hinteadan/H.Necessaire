using System;
using System.Linq;

namespace H.Necessaire.Runtime.UI
{
    public class HHeightCategory : IStringIdentity
    {
        public HHeightCategory(string id, NumberInterval interval)
        {
            ID = id;
            Interval = interval;
        }

        public string ID { get; }
        public NumberInterval Interval { get; }

        public static readonly HHeightCategory XSmall = new HHeightCategory(nameof(XSmall), new NumberInterval(null, 576, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HHeightCategory Small = new HHeightCategory(nameof(Small), new NumberInterval(576, 768, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HHeightCategory Medium = new HHeightCategory(nameof(Medium), new NumberInterval(768, 992, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HHeightCategory Large = new HHeightCategory(nameof(Large), new NumberInterval(992, 1200, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HHeightCategory XLarge = new HHeightCategory(nameof(XLarge), new NumberInterval(1200, 1400, isMinIncluded: true, isMaxIncluded: false));
        public static readonly HHeightCategory XXLarge = new HHeightCategory(nameof(XXLarge), new NumberInterval(1400, null, isMinIncluded: true, isMaxIncluded: true));

        public static readonly HHeightCategory[] All = new HHeightCategory[] { XSmall, Small, Medium, Large, XLarge, XXLarge };
    }

    public class HHeightCategoryAction
    {
        public HHeightCategoryAction(HHeightCategory[] categories, Action<HHeightCategory[]> action)
        {
            Categories = categories;
            Action = action;
        }
        public HHeightCategory[] Categories { get; }
        public Action<HHeightCategory[]> Action { get; }
    }

    public static class HHeightCategoryExtensions
    {
        public static HHeightCategoryAction WithAction(this HHeightCategory[] categories, Action<HHeightCategory[]> action)
            => new HHeightCategoryAction(categories, action);

        public static HHeightCategoryAction WithAction(this HHeightCategory category, Action<HHeightCategory[]> action)
            => new HHeightCategoryAction(category.AsArray(), action);

        public static HHeightCategory GetHeightCategory(this decimal value)
            => HHeightCategory.All.Single(x => value.In(x.Interval));

        public static HHeightCategory GetHeightCategory(this double value)
            => ((decimal)value).GetHeightCategory();

        public static void OnHeightCategory(this decimal value, Action defaultAction = null, params HHeightCategoryAction[] actions)
        {
            if (actions.IsEmpty())
            {
                if (!(defaultAction is null))
                    defaultAction.Invoke();

                return;
            }

            foreach (HHeightCategoryAction categoryAction in actions)
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

        public static void OnHeightCategory(this decimal value, params HHeightCategoryAction[] actions)
            => value.OnHeightCategory(defaultAction: null, actions);

        public static void OnHeightCategory(this double value, Action defaultAction = null, params HHeightCategoryAction[] actions)
            => ((decimal)value).OnHeightCategory(defaultAction, actions);

        public static void OnHeightCategory(this double value, params HHeightCategoryAction[] actions)
            => ((decimal)value).OnHeightCategory(actions);
    }
}
