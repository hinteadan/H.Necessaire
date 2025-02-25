using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.UI
{
    public class HUIPresentationInfo
    {
        public HUIPresentationType Type { get; set; } = HUIPresentationType.Custom;

        public bool IsRequired { get; set; } = false;

        public HUIBooleanPresentationInfo Boolean { get; set; }
        public HUITextPresentationInfo Text { get; set; }
        public HUINumberPresentationInfo Number { get; set; }
        public HUISelectionPresentationInfo Selection { get; set; }
        public HUIDateTimePresentationInfo DateTime { get; set; }
        public HUICollectionPresentationInfo Collection { get; set; }
    }

    public static class HViewModelPropertyPresentationInfoExtensions
    {
        public static HUIPresentationType GetPresentationType(this Type dataType)
        {
            if (dataType.In(typeof(string)))
                return HUIPresentationType.Text;

            if (dataType.In(typeof(Note), typeof(Note?)))
                return HUIPresentationType.Note;

            if (dataType.In(typeof(bool), typeof(bool?)))
                return HUIPresentationType.Boolean;

            if (dataType.In(
                typeof(sbyte), typeof(sbyte?),
                typeof(byte), typeof(byte?),

                typeof(short), typeof(short?),
                typeof(ushort), typeof(ushort?),

                typeof(int), typeof(int?),
                typeof(uint), typeof(uint?),

                typeof(long), typeof(long?),
                typeof(ulong), typeof(ulong?),

                typeof(float), typeof(float?),

                typeof(double), typeof(double?),

                typeof(decimal), typeof(decimal?)
                )
            )
                return HUIPresentationType.Number;

            if (dataType.In(typeof(NumberInterval), typeof(NumberInterval?)))
                return HUIPresentationType.NumberInterval;

            if (dataType.In(typeof(TimeSpan), typeof(TimeSpan?)))
                return HUIPresentationType.TimeSpan;

            if (dataType.FullName == "System.DateOnly")
                return HUIPresentationType.Date;

            if (dataType.FullName == "System.TimeOnly")
                return HUIPresentationType.Time;

            if (dataType.In(typeof(DateTime), typeof(DateTime?)))
                return HUIPresentationType.DateAndTime;

            if (dataType.In(typeof(PeriodOfTime)))
                return HUIPresentationType.PeriodOfTime;

            if (dataType.In(typeof(PartialDateTime)))
                return HUIPresentationType.PartialDateTime;

            if (dataType.In(typeof(PartialPeriodOfTime)))
                return HUIPresentationType.PartialPeriodOfTime;

            if (dataType.In(typeof(ApproximatePeriodOfTime)))
                return HUIPresentationType.ApproximatePeriodOfTime;

            Type enumerableType
                = dataType.GetInterfaces().SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?? dataType.GetInterfaces().SingleOrDefault(i => i == typeof(IEnumerable))
                ;

            if (!(enumerableType is null))
                return HUIPresentationType.Collection;


            if (!dataType.Assembly.IsCoreAssembly() && !dataType.GetProperties().IsEmpty())
                return HUIPresentationType.SubViewModel;


            return HUIPresentationType.Custom;
        }
    }
}