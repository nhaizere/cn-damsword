using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DamSword.Common
{
    public class GenericComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _comparer;

        public GenericComparer(Func<T, T, int> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }
    }

    public static class EnumerableExtensions
    {
        public static string Join<T>(this IEnumerable<T> self)
        {
            return self.Join(string.Empty);
        }

        public static string Join<T>(this IEnumerable<T> self, string separator)
        {
            return string.Join(separator, self);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> self)
        {
            return !self.Any();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> self)
        {
            return self == null || !self.Any();
        }

        public static bool NonNullOrEmpty<T>(this IEnumerable<T> self)
        {
            return !self.IsNullOrEmpty();
        }

        public static bool ContainsAll<T>(this IEnumerable<T> self, IEnumerable<T> other)
        {
            return self.Intersect(other).Count() == self.Count();
        }

        public static bool ContainsAll<T1, T2>(this IEnumerable<T1> self, IEnumerable<T2> other, Func<T1, T2, bool> equalityComparer)
        {
            return self.Count(t1 => other.Any(t2 => equalityComparer(t1, t2))) == self.Count();
        }

        public static bool ContainsOnly<T>(this IEnumerable<T> self, IEnumerable<T> other)
        {
            return self.Count() == other.Count() && self.ContainsAll(other);
        }

        public static bool ContainsOnly<T1, T2>(this IEnumerable<T1> self, IEnumerable<T2> other, Func<T1, T2, bool> equalityComparer)
        {
            return self.Count() == other.Count() && self.ContainsAll(other, equalityComparer);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> self, params T[] items)
        {
            return self.Concat(items);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, params T[] items)
        {
            return items.Concat(self);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> self, params T[] items)
        {
            return self.Except((IEnumerable<T>)items);
        }

        public static IEnumerable<T> Assert<T>(this IEnumerable<T> self, Func<T, bool> that, Func<Exception> exceptionFactoryMethod)
        {
            foreach (var item in self)
            {
                var assertResult = that(item);
                if (!assertResult)
                {
                    var exception = exceptionFactoryMethod();
                    throw exception;
                }
            }

            return self;
        }

        public static IEnumerable<T> DistinctBy<T, TProperty>(this IEnumerable<T> self, Func<T, TProperty> propertySelector)
        {
            var values = new HashSet<TProperty>();
            foreach (var item in self)
            {
                var value = propertySelector(item);
                var isNew = values.Add(value);
                if (isNew)
                    yield return item;
            }
        }

        public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> self)
        {
            return self.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> self)
        {
            return new ReadOnlyCollection<T>(self.ToList());
        }

        public static IReadOnlyDictionary<T1, T2> ToReadOnlyDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> self)
        {
            return new ReadOnlyDictionary<T1, T2>(self.ToDictionary());
        }

        public static IEnumerable<TSource> Order<TSource>(this IEnumerable<TSource> self, Func<TSource, TSource, int> comparer)
        {
            var genericComparer = new GenericComparer<TSource>(comparer);
            return self.OrderBy(t => t, genericComparer);
        }

        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            var genericComparer = new GenericComparer<TKey>(comparer);
            return self.OrderBy(keySelector, genericComparer);
        }

        public static int FindIndex<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var index = 0;
            foreach (var item in self)
            {
                if (predicate(item))
                    return index;

                index++;
            }

            return -1;
        }

        public static int Count(this IEnumerable self)
        {
            var collection = self as ICollection;
            if (collection != null)
                return collection.Count;

            var count = 0;
            var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext())
                count++;

            return count;
        }
    }
}
