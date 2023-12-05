using System;
using UnityEngine;

namespace RangePrimitive
{
    [Serializable]
    public struct Range<T>
    {
        [SerializeField]
        private T min;
        
        [SerializeField]
        private T max;
        
        public Range(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public T Min
        {
            get => min;
            set => min = value;
        }

        public T Max
        {
            get => max;
            set => max = value;
        }
    }
}