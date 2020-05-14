using Loadify.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Loadify.Extension
{
    public static class Extensions
    {
        public static IEnumerable<(int, TGeneralized)> Enumerate<TGeneralized>
        (this IEnumerable<TGeneralized> array) => array.Select((item, index) => (index, item));

        public static bool IsIn<T>(this T keyObject, IEnumerable<T> collection) => collection.Contains(keyObject);


        public static void DispatchedAdd<T>(this ObservableCollection<T> collection, T item)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                collection.Add(item);
            });
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f) => e.SelectMany(c => f(c).Flatten(f)).Concat(e);

        public static void DispatchedInsert<T>(this ObservableCollection<T> collection, int index, T item)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                collection.Insert(index, item);
            });
        }


        public static void DispatchedRemove<T>(this ObservableCollection<T> collection, T item)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                collection.Remove(item);
            });
        }

        public static void DispatchedAddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> enumerable)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                collection.AddRange(enumerable);
            });
        }



        public static Task ChainTask<TGeneralized>(this IEnumerable<TGeneralized> enumerable, Action<TGeneralized> action)
        {
            Task the_task = new Task(() => { });
            Task next = the_task;
            enumerable.All((x) => { next = NextTask(next, new Task(() => { action(x); })); return true; });
            return the_task;
        }

        private static Task NextTask(Task task, Task newTask)
        {
            task.ContinueWith(ascendant => newTask.Start());
            return newTask;
        }

        public static void Swap<T>(this ObservableCollection<T> collection, T first, T second, Func<T, T, Tuple< int,int>> indexFinder)
        {
            var indexes = indexFinder(first, second);
            collection.Remove(first);
            collection.Insert(indexes.Item1, first);
            collection.Remove(second);
            collection.Insert(indexes.Item2, second);
        }

        public static void AddRange<T>(this ICollection<T> destination,
                               IEnumerable<T> source)
        {
            if (destination is List<T> list)
            {
                list.AddRange(source);
            }
            else
            {
                foreach (T item in source)
                {
                    destination.Add(item);
                }
            }
        }
    }
}
