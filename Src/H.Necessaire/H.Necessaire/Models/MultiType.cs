using System;

namespace H.Necessaire
{
    public interface ImAMultiType
    {
        object ToObject();
    }

    public class MultiType<TFirst, TSecond> : ImAMultiType
    {
        #region Construct
        TFirst valueForFirst;
        bool hasFirstValue = false;

        TSecond valueForSecond;
        bool hasSecondValue = false;

        public MultiType() { }

        private MultiType(TFirst valueForFirst)
        {
            hasFirstValue = true;
            this.valueForFirst = valueForFirst;
        }

        private MultiType(TSecond valueForSecond)
        {
            hasSecondValue = true;
            this.valueForSecond = valueForSecond;
        }

        public static implicit operator MultiType<TFirst, TSecond>(TFirst value) => new MultiType<TFirst, TSecond>(value);

        public static implicit operator MultiType<TFirst, TSecond>(TSecond value) => new MultiType<TFirst, TSecond>(value);

        public static explicit operator TFirst(MultiType<TFirst, TSecond> value)
        {
            TFirst result = default(TFirst);
            value.Read(a => result = a, null);
            return result;
        }

        public static explicit operator TSecond(MultiType<TFirst, TSecond> value)
        {
            TSecond result = default(TSecond);
            value.Read(null, b => result = b);
            return result;
        }
        #endregion

        public TFirst A
        {
            get => valueForFirst;
            set
            {
                valueForFirst = value;
                hasFirstValue = true;
                hasSecondValue = false;
            }
        }

        public TSecond B
        {
            get => valueForSecond;
            set
            {
                valueForSecond = value;
                hasFirstValue = false;
                hasSecondValue = true;
            }
        }

        public bool HasA
        {
            get => hasFirstValue;
            set
            {
                hasFirstValue = value;
                hasSecondValue = !hasFirstValue;
            }
        }
        public bool HasB
        {
            get => hasSecondValue;
            set
            {
                hasSecondValue = value;
                hasFirstValue = !hasSecondValue;
            }
        }

        public void Read(Action<TFirst> readFirstType = null, Action<TSecond> readSecondType = null)
        {
            if (hasFirstValue)
            {
                readFirstType?.Invoke(valueForFirst);
                return;
            }

            if (hasSecondValue)
            {
                readSecondType?.Invoke(valueForSecond);
                return;
            }
        }

        public override string ToString()
        {
            string result = null;
            Read(
                readFirstType: x => result = x?.ToString(),
                readSecondType: x => result = x?.ToString()
            );
            return result;
        }

        public object ToObject()
        {
            object result = null;
            Read(
                readFirstType: x => result = (x is ImAMultiType) ? (x as ImAMultiType)?.ToObject() : x,
                readSecondType: x => result = (x is ImAMultiType) ? (x as ImAMultiType)?.ToObject() : x
            );
            return result;
        }
    }
}
