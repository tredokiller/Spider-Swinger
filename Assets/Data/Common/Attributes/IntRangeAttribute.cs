using UnityEngine;

namespace Data.Common.Attributes
{
    public class IntRangeAttribute : PropertyAttribute
    {
        public readonly int min;
        public readonly int max;

        public IntRangeAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
}