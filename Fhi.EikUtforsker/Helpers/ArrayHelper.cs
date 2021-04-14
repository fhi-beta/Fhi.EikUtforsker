using System;
using System.Linq;

namespace Fhi.EikUtforsker.Helpers
{
    public static class ArrayHelper
    {
        public static (T[] first, T[] second) Split<T>(T[] array, int index)
        {
            var first = array.Take(index).ToArray();
            var second = array.Skip(index).ToArray();
            return (first, second);
        }

        public static T[] Concat<T>(T[] a, T[] b)
        {
            T[] output = new T[a.Length + b.Length];

            for (int i = 0; i < a.Length; i++)
            {
                output[i] = a[i];
            }

            for (int j = 0; j < b.Length; j++)
            {
                output[a.Length + j] = b[j];
            }

            return output;
        }

        public static T[] Sub<T>(T[] data, int start, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, start, result, 0, length);
            return result;
        }
    }
}
