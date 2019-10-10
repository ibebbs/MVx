using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Crux
{
    public class Property<TValue> : IObservable<TValue>, IObserver<TValue>
    {
        private TValue _current;
        private BehaviorSubject<TValue> _values;
        private IDisposable _notifySubscription;

        public Property(TValue defaultValue, string propertyName, Action<string> notifyPropertyChanged)
        {
            _current = defaultValue;
            _values = new BehaviorSubject<TValue>(defaultValue);

            _notifySubscription = _values
                .DistinctUntilChanged()
                .Do(value => _current = value)
                .Subscribe(value => notifyPropertyChanged(propertyName));
        }

        public Property(string propertyName, Action<string> notifyPropertyChanged) : this(default, propertyName, notifyPropertyChanged) { }

        IDisposable IObservable<TValue>.Subscribe(IObserver<TValue> observer)
        {
            return _values.Subscribe(observer);
        }

        void IObserver<TValue>.OnCompleted()
        {
            // Do nothing
        }

        void IObserver<TValue>.OnError(Exception error)
        {
            // Do nothing
        }

        void IObserver<TValue>.OnNext(TValue value)
        {
            _values.OnNext(value);
        }

        public TValue Get()
        {
            return _current;
        }

        public void Set(TValue value)
        {
            _values.OnNext(value);
        }
    }
}
