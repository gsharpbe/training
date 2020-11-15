using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Metanous.Model.Core.Extensions
{
    /// <summary>
    /// Holds extension methods for <see cref="ICollection{T}"/>.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the elements from the specified collection - <paramref name="items"/> to the end of the target <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The collection that will be extended.</param>
        /// <param name="items">The items that will be added.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null</exception>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            items.ForEach(collection.Add);
        }

        /// <summary>
        /// Adds the elements from the specified collection - <paramref name="items"/> to the end of the target <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list that will be extended.</param>
        /// <param name="items">The items that will be added.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null</exception>
        public static void AddRange(this IList list, IEnumerable items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (object item in items)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Removes all elements from the given collection.
        /// </summary>
        /// <typeparam name="T">Elements type.</typeparam>
        /// <param name="collection">The collection, which will be emptied.</param>
        public static void RemoveAll<T>(this IList<T> collection)
        {
            while (collection.Count != 0)
            {
                collection.RemoveAt(0);
            }
        }

        /// <summary>
        /// Removes all elements from a collection that match the condition defined by the specified predicate.
        /// </summary>
        /// <typeparam name="T">Elements type.</typeparam>
        /// <param name="collection">The collection, which elements will be removed.</param>
        /// <param name="match">The predicate delegate that defines the condition for the removed elements.</param>
        /// <returns>Number of removed elements.</returns>
        public static int RemoveAll<T>(this ICollection<T> collection, Predicate<T> match)
        {
            var removed = collection.Where(item => match(item));

            int count = 0;

            foreach (var item in removed)
            {
                collection.Remove(item);
                count = count + 1;
            }

            return count;
        }

        /// <summary>
        /// Removes all elements from a list that match the condition defined by the specified predicate.
        /// </summary>
        /// <param name="list">The list, which elements will be removed.</param>
        /// <param name="match">The predicate delegate that defines the condition for the removed elements.</param>
        /// <returns>Number of removed elements.</returns>
        public static int RemoveAll(this IList list, Predicate<object> match)
        {
            var removed = new List<object>();
            foreach (object item in list)
            {
                if (match(item))
                {
                    removed.Add(item);
                }
            }

            foreach (object item in removed)
            {
                list.Remove(item);
            }

            return removed.Count;
        }
    }
}