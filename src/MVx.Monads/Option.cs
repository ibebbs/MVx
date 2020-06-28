using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVx.Monads
{
    public static class Option
    {
        public static Option<T> AsOption<T>(this T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default) ? Option<T>.None : Option<T>.Some(value);
        }

        public static Option<TResult> Select<TSource,TResult>(this Option<TSource> source, Func<TSource, TResult> projection)
        {
            return source.IsSome ? Option<TResult>.Some(projection(source.Value)) : Option<TResult>.None;
        }
        public static Option<TResult> Select<TSource, TResult>(this Option<TSource> source, Func<TSource, Option<TResult>> projection)
        {
            return source.IsSome ? projection(source.Value) : Option<TResult>.None;
        }

        public static async Task<Option<TResult>> SelectAsync<TSource, TResult>(this Option<TSource> source, Func<TSource, Task<TResult>> projection)
        {
            return source.IsSome ? Option<TResult>.Some(await projection(source.Value).ConfigureAwait(false)) : Option<TResult>.None;
        }

        public static async Task<Option<TResult>> SelectAsync<TSource, TResult>(this Option<TSource> source, Func<TSource, Task<Option<TResult>>> projection)
        {
            return source.IsSome ? await projection(source.Value).ConfigureAwait(false) : Option<TResult>.None;
        }

        public static IEnumerable<T> Collect<T>(this IEnumerable<Option<T>> source)
        {
            return source.Where(option => option.IsSome).Select(option => option.Value);
        }

        public static T Coalesce<T>(this Option<T> source, Func<T> value)
        {
            return source.IsSome ? source.Value : value();
        }

        public static Option<T> Coalesce<T>(this Option<T> source, Func<Option<T>> value)
        {
            return source.IsSome ? source : value();
        }

        public static async Task<T> CoalesceAsync<T>(this Option<T> source, Func<Task<T>> value)
        {
            return source.IsSome ? source.Value : await value().ConfigureAwait(false);
        }

        public static async Task<T> CoalesceAsync<T>(this Task<Option<T>> task, Func<Task<T>> value)
        {
            var source = await task.ConfigureAwait(false);

            return source.IsSome ? source.Value : await value().ConfigureAwait(false);
        }

        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, TKey key)
        {
            if (source.TryGetValue(key, out TValue value))
            {
                return Option<TValue>.Some(value);
            }
            else
            {
                return Option<TValue>.None;
            }
        }

        public static T ValueOrThrow<T>(this Option<T> source, Func<Exception> exceptionFactory)
        {
            if (source.IsSome)
            {
                return source.Value;
            }
            else
            {
                throw exceptionFactory();
            }
        }

        public static Option<T> OnSome<T>(this Option<T> source, Action<T> action)
        {
            if (source.IsSome)
            {
                action(source.Value);
            }

            return source;
        }

        public static async Task<Option<T>> OnSomeAsync<T>(this Option<T> source, Func<T, Task> action)
        {
            if (source.IsSome)
            {
                await action(source.Value).ConfigureAwait(false);
            }

            return source;
        }

        public static async Task<Option<T>> OnSomeAsync<T>(this Task<Option<T>> task, Func<T, Task> action)
        {
            var source = await task.ConfigureAwait(false);

            if (source.IsSome)
            {
                await action(source.Value).ConfigureAwait(false);
            }

            return source;
        }

        public static Option<T> OnNone<T>(this Option<T> source, Action action)
        {
            if (source.IsNone)
            {
                action();
            }

            return source;
        }

        public static async Task<Option<T>> OnNoneAsync<T>(this Option<T> source, Func<Task> action)
        {
            if (source.IsNone)
            {
                await action().ConfigureAwait(false);
            }

            return source;
        }

        public static async Task<Option<T>> OnNoneAsync<T>(this Task<Option<T>> task, Func<Task> action)
        {
            var source = await task.ConfigureAwait(false);

            if (source.IsNone)
            {
                await action().ConfigureAwait(false);
            }

            return source;
        }

        public static Option<T> FirstOption<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return Option<T>.Some(enumerator.Current);
                }
                else
                {
                    return Option<T>.None;
                }
            }
        }
    }

    public struct Option<T>
    {
        public static readonly Option<T> None = new Option<T>();

        public static Option<T> Some(T value)
        {
            return new Option<T>(true, value);
        }

        public Option(bool isSome, T value)
        {
            IsSome = isSome;
            Value = value;
        }

        public bool IsSome { get; }

        public bool IsNone => !IsSome;

        public T Value { get; }
    }
}
