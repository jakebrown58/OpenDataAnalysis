using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataElements.Model.Analytics
{
    public class PivotFunctions
    {
        public static Dictionary<K, IEnumerable<T>> GeneratePivotData<K, T>(IEnumerable<T> collection, Func<T, K> pivot)
        {
            Dictionary<K, IEnumerable<T>> values = ProducePivotAnalytic<T, K, IEnumerable<T>>(collection,
                (x) => pivot(x),
                (z) => (z));

            return values;
        }

        public static Dictionary<K, int> GenerateCategoricalCounts<K, T>(IEnumerable<T> collection, Func<T, K> pivot)
        {
            Dictionary<K, int> histogram = ProducePivotAnalytic<T, K, int>(
                collection,
                (x) => pivot(x),
                (z) => (z.Count()));

            return histogram;
        }

        public static Dictionary<K, int> ProduceHistogram<T, K>(IEnumerable<T> colletion, Func<T, K> f)
        {
            var grouped =
                from n in colletion
                group n by f(n) into g
                select new { theKey = g.Key, theValue = g };

            var dictionary = new Dictionary<K, int>();

            foreach (var g in grouped)
            {
                dictionary.Add(g.theKey, g.theValue.Count());
            }

            return dictionary;
        }

        public static Dictionary<K, V> ProducePivotAnalytic<T, K, V>(IEnumerable<T> colletion, Func<T, K> pivotOn, Func<IEnumerable<T>, V> analytic)
        {
            var grouped =
                from n in colletion
                group n by pivotOn(n) into g
                select new { theKey = g.Key, theValue = g };

            Dictionary<K, V> dictionary = new Dictionary<K, V>();

            foreach (var g in grouped)
            {
                V theValue = analytic(g.theValue);
                dictionary.Add(g.theKey, theValue);
            }

            return dictionary;
        }
    }
}
