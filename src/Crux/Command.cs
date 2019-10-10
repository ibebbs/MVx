using System;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Crux
{
    public class Command : ICommand, IObservable<object>, IObserver<bool>
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

        IDisposable IObservable<object>.Subscribe(IObserver<object> observer)
        {
            return _invocations.Subscribe(observer);
        }

        private void OnCanExecuteChanged(bool canExecute)
        {
            _canExecute = canExecute;

            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        void IObserver<bool>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<bool>.OnError(Exception error)
        {
            throw new NotImplementedException();
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
