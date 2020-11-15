using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Metanous.Model.Core.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the collection.
        /// </summary>
        /// <typeparam name="T">Type of the elements in the collection.</typeparam>
        /// <param name="source">The collection on which elements the action will be executed.</param>
        /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the collection.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null) return;
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static void EachIndex<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null || action == null) return;
            var i = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    action(i++);
                }
            }
        }

        [Pure]
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        [Pure]
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence was empty");
                }

                TSource min = sourceIterator.Current;

                TKey minKey = selector(min);

                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;

                    TKey candidateProjected = selector(candidate);

                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;

                        minKey = candidateProjected;
                    }
                }

                return min;
            }
        }

        [Pure]
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        [Pure]
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence was empty");
                }

                TSource max = sourceIterator.Current;

                TKey maxKey = selector(max);

                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;

                    TKey candidateProjected = selector(candidate);

                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;

                        maxKey = candidateProjected;
                    }
                }

                return max;
            }
        }

        [Pure]
        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        [Pure]
        public static IEnumerable<T> TakeAfter<T>(this IEnumerable<T> target, Func<T, bool> predicate)
        {
            return target.TakeFrom(predicate).SkipWhile(predicate);
        }

        [Pure]
        public static IEnumerable<T> TakeTo<T>(this IEnumerable<T> target, Func<T, bool> predicate)
        {
            var enumerable = target as T[] ?? target.ToArray();

            if (enumerable.Any(predicate))
            {
                var shouldBreak = false;
                foreach (var item in enumerable)
                {
                    if (shouldBreak && !predicate(item))
                    {
                        break;
                    }
                    yield return item;
                    if (predicate(item))
                    {
                        shouldBreak = true;
                    }
                }
            }
        }

        [Pure]
        public static IEnumerable<T> TakeFrom<T>(this IEnumerable<T> target, Func<T, bool> predicate)
        {
            return target.SkipWhile(Reverse(predicate));
        }

        [Pure]
        public static IEnumerable<T> TakeBetween<T>(this IEnumerable<T> target, Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            var enumerable = target as T[] ?? target.ToArray();
            return enumerable.TakeTo(predicate1).TakeFrom(predicate2).Union(enumerable.TakeTo(predicate2).TakeFrom(predicate1));
        }

        [Pure]
        public static IEnumerable<T> TakeTo<T>(this IEnumerable<T> target, T item)
        {
            return target.TakeTo(i => Equals(i, item));
        }

        [Pure]
        public static IEnumerable<T> TakeFrom<T>(this IEnumerable<T> target, T item)
        {
            return target.TakeFrom(i => Equals(i, item));
        }

        [Pure]
        public static IEnumerable<T> TakeBetween<T>(this IEnumerable<T> target, T item1, T item2)
        {
            return target.TakeBetween(i => Equals(i, item1), i => Equals(i, item2));
        }

        [Pure]
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T other)
        {
            foreach (var item in source)
            {
                yield return item;
            }

            yield return other;
        }

        private static Func<T, bool> Reverse<T>(Func<T, bool> f)
        {
            return a => !f(a);
        }
    }
}
