using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVx.Monads
{
    public static class Fallible
    {
        public static Fallible<T> Operation<T>(Func<T> operation)
        {
            try
            {
                return Fallible<T>.Success(operation());
            }
            catch (Exception exception)
            {
                return Fallible<T>.Fail(exception);
            }
        }

        public static async Task<Fallible<T>> OperationAsync<T>(Func<Task<T>> operation)
        {
            try
            {
                return Fallible<T>.Success(await operation().ConfigureAwait(false));
            }
            catch (Exception exception)
            {
                return Fallible<T>.Fail(exception);
            }
        }

        public static Fallible<TResult> Select<TSource, TResult>(this Fallible<TSource> source, Func<TSource, TResult> projection)
        {
            return source.IsSuccess ? Operation(() => projection(source.Value)) : Fallible<TResult>.Fail(source.Exception);
        }

        public static Fallible<TResult> Select<TSource, TResult>(this Fallible<TSource> source, Func<TSource, Fallible<TResult>> projection)
        {
            return source.IsSuccess ? projection(source.Value) : Fallible<TResult>.Fail(source.Exception);
        }

        public static async Task<Fallible<TResult>> SelectAsync<TSource, TResult>(this Fallible<TSource> source, Func<TSource, Task<TResult>> projection)
        {
            return source.IsSuccess ? await OperationAsync(() => projection(source.Value)).ConfigureAwait(false) : Fallible<TResult>.Fail(source.Exception);
        }

        public static async Task<Fallible<TResult>> SelectAsync<TSource, TResult>(this Fallible<TSource> source, Func<TSource, Task<Fallible<TResult>>> projection)
        {
            return source.IsSuccess ? await projection(source.Value).ConfigureAwait(false) : Fallible<TResult>.Fail(source.Exception);
        }

        public static async Task<Fallible<TResult>> SelectAsync<TSource, TResult>(this Task<Fallible<TSource>> sourceTask, Func<TSource, TResult> projection)
        {
            var source = await sourceTask.ConfigureAwait(false);

            return source.IsSuccess ? Fallible<TResult>.Success(projection(source.Value)) : Fallible<TResult>.Fail(source.Exception);
        }

        public static async Task<Fallible<TResult>> SelectAsync<TSource, TResult>(this Task<Fallible<TSource>> sourceTask, Func<TSource, Fallible<TResult>> projection)
        {
            var source = await sourceTask.ConfigureAwait(false);

            return source.IsSuccess ? projection(source.Value) : Fallible<TResult>.Fail(source.Exception);
        }

        public static async Task<Fallible<TResult>> SelectAsync<TSource, TResult>(this Task<Fallible<TSource>> sourceTask, Func<TSource, Task<TResult>> projection)
        {
            var source = await sourceTask.ConfigureAwait(false);

            return source.IsSuccess ? Fallible<TResult>.Success(await projection(source.Value).ConfigureAwait(false)) : Fallible<TResult>.Fail(source.Exception);
        }

        public static async Task<Fallible<TResult>> SelectAsync<TSource, TResult>(this Task<Fallible<TSource>> sourceTask, Func<TSource, Task<Fallible<TResult>>> projection)
        {
            var source = await sourceTask.ConfigureAwait(false);

            return source.IsSuccess ? await projection(source.Value).ConfigureAwait(false) : Fallible<TResult>.Fail(source.Exception);
        }

        public static void Do<T>(this Fallible<T> source, Action<T> onSuccess, Action<Exception> onFail)
        {
            if (source.IsSuccess)
            {
                onSuccess(source.Value);
            }
            else
            {
                onFail(source.Exception);
            }
        }

        public static Fallible<T> OnSuccess<T>(this Fallible<T> source, Action<T> action)
        {
            if (source.IsSuccess)
            {
                action(source.Value);
            }

            return source;
        }

        public static async Task<Fallible<T>> OnSuccessAsync<T>(this Fallible<T> source, Func<T, Task> action)
        {
            if (source.IsSuccess)
            {
                await action(source.Value).ConfigureAwait(false);
            }

            return source;
        }

        public static async Task<Fallible<T>> OnSuccessAsync<T>(this Task<Fallible<T>> task, Func<T, Task> action)
        {
            var source = await task.ConfigureAwait(false);

            if (source.IsSuccess)
            {
                await action(source.Value).ConfigureAwait(false);
            }

            return source;
        }

        public static Fallible<T> OnFailure<T>(this Fallible<T> source, Action<Exception> action)
        {
            if (source.IsFailure)
            {
                action(source.Exception);
            }

            return source;
        }

        public static async Task<Fallible<T>> OnFailureAsync<T>(this Fallible<T> source, Func<Exception, Task> action)
        {
            if (source.IsFailure)
            {
                await action(source.Exception).ConfigureAwait(false);
            }

            return source;
        }

        public static async Task<Fallible<T>> OnFailureAsync<T>(this Task<Fallible<T>> task, Func<Exception, Task> action)
        {
            var source = await task.ConfigureAwait(false);

            if (source.IsFailure)
            {
                await action(source.Exception).ConfigureAwait(false);
            }

            return source;
        }

        public static T ValueOrThrow<T>(this Fallible<T> source)
        {
            if (source.IsSuccess)
            {
                return source.Value;
            }
            else
            {
                throw source.Exception;
            }
        }

        public static T Coalesce<T>(this Fallible<T> source, Func<Exception, T> projection)
        {
            return source.IsSuccess ? source.Value : projection(source.Exception);
        }

        public static Fallible<T> Coalesce<T>(this Fallible<T> source, Func<Exception, Fallible<T>> projection)
        {
            return source.IsSuccess ? source : projection(source.Exception);
        }

        public async static Task<T> CoalesceAsync<T>(this Task<Fallible<T>> sourceTask, Func<Exception, T> projection)
        {
            var source = await sourceTask.ConfigureAwait(false);

            return source.IsSuccess ? source.Value : projection(source.Exception);
        }

        public async static Task<Fallible<T>> CoalesceAsync<T>(this Task<Fallible<T>> sourceTask, Func<Exception, Fallible<T>> projection)
        {
            var source = await sourceTask.ConfigureAwait(false);

            return source.IsSuccess ? source : projection(source.Exception);
        }
        public async static Task<T> CoalesceAsync<T>(this Task<Fallible<T>> sourceTask, Func<Exception, Task<T>> projection)
        {
            var source = await sourceTask.ConfigureAwait(false);

            return source.IsSuccess ? source.Value : await projection(source.Exception);
        }

        public async static Task<Fallible<T>> CoalesceAsync<T>(this Task<Fallible<T>> sourceTask, Func<Exception, Task<Fallible<T>>> projection)
        {
            var source = await sourceTask.ConfigureAwait(false);

            return source.IsSuccess ? source : await projection(source.Exception);
        }

        public static IEnumerable<T> Successful<T>(this IEnumerable<Fallible<T>> source)
        {
            return source.Where(fallible => fallible.IsSuccess).Select(fallible => fallible.Value);
        }

        public static IEnumerable<Exception> Failed<T>(this IEnumerable<Fallible<T>> source)
        {
            return source.Where(fallible => fallible.IsFailure).Select(fallible => fallible.Exception);
        }

        public static Fallible<TDest> Cast<TSource,TDest>(this Fallible<TSource> source) where TDest : TSource
        {
            return source.Select(value => Operation(() => (TDest)value));
        }

        public static IEnumerable<Fallible<TDest>> Cast<TSource,TDest>(this IEnumerable<Fallible<TSource>> source) where TDest : TSource
        {
            return source.Select(fallible => fallible.Cast<TSource, TDest>());
        }
    }

    public interface IFallible
    {
        bool IsSuccess { get; }

        bool IsFailure { get; }

        Exception Exception { get; }
    }

    public struct Fallible<T> : IFallible
    {
        public static Fallible<T> Success(T result)
        {
            return new Fallible<T>(result);
        }

        public static Fallible<T> Fail(Exception exception)
        {
            return new Fallible<T>(exception);
        }

        public Fallible(T value)
        {
            IsSuccess = true;
            Value = value;
            Exception = null;
        }

        public Fallible(Exception exception)
        {
            IsSuccess = false;
            Value = default;
            Exception = exception;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public T Value { get; }

        public Exception Exception { get; }
    }
}
