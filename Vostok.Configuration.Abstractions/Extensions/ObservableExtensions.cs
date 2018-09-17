using System;
using System.Runtime.ExceptionServices;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Extensions
{
    /// <summary>
    /// Provides a set of static methods for subscribing delegates to observables.
    /// </summary>
    [PublicAPI]
    public static class ObservableExtensions
    {
        /// <summary>
        /// Subscribes an element handler to an observable sequence.
        /// </summary>
        [NotNull]
        public static IDisposable Subscribe<T>([NotNull] this IObservable<T> source, [NotNull] Action<T> onNext)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (onNext == null)
                throw new ArgumentNullException(nameof(onNext));

            return source.Subscribe(new AnonymousObserver<T>(onNext));
        }

        /// <summary>
        /// Subscribes an element handler and an exception handler to an observable sequence.
        /// </summary>
        [NotNull]
        public static IDisposable Subscribe<T>([NotNull] this IObservable<T> source, [NotNull] Action<T> onNext, [NotNull] Action<Exception> onError)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (onNext == null)
                throw new ArgumentNullException(nameof(onNext));
            if (onError == null)
                throw new ArgumentNullException(nameof(onError));

            return source.Subscribe(new AnonymousObserver<T>(onNext, onError));
        }

        private class AnonymousObserver<T> : IObserver<T>
        {
            private readonly Action<T> onNext;
            private readonly Action<Exception> onError;

            public AnonymousObserver(Action<T> onNext, Action<Exception> onError = null)
            {
                this.onNext = onNext;
                this.onError = onError;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
                if (onError == null)
                    ExceptionDispatchInfo.Capture(error).Throw();
                else
                    onError(error);
            }

            public void OnNext(T value) => onNext(value);
        }
    }
}