﻿using System;

namespace H.Necessaire
{
    public interface ImAMultiType
    {
        object ToObject();
    }

    public class MultiType<TFirst, TSecond> : ImAMultiType
    {
        #region Construct
        readonly TFirst valueForFirst;
        readonly bool hasFirstValue = false;

        readonly TSecond valueForSecond;
        readonly bool hasSecondValue = false;

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
        #endregion

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
