using System;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Crux
{
    public class Command : ICommand, IObservable<object>, IObserver<bool>, IDisposable
    {
        private Subject<object> _invocations;
        private bool _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(bool canExecute)
        {
            _invocations = new Subject<object>();
            _canExecute = canExecute;
        }

        public Command() : this(false) { }

        private void Dispose(bool isDisposing)
        {
            if (_invocations != null)
            {
                _invocations.Dispose();
                _invocations = null;
            }

            if (isDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~Command() => Dispose(false);

        IDisposable IObservable<object>.Subscribe(IObserver<object> observer)
        {
            return _invocations.Subscribe(observer);
        }

        private void OnCanExecuteChanged(bool canExecute)
        {
            _canExecute = canExecute;

            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        void IObserver<bool>.OnCompleted()
        {
            // Do nothing
        }

        void IObserver<bool>.OnError(Exception error)
        {
            // Do nothing
        }

        void IObserver<bool>.OnNext(bool value)
        {
            OnCanExecuteChanged(value);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _invocations.OnNext(parameter);
        }
    }
}
