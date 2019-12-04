using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Crux
{

    public interface IBus : IDisposable
    {
        IObservable<TEvent> GetEvent<TEvent>();
        void Publish<TEvent>(TEvent sampleEvent);
    }

    public class Bus : IBus
    {
        private Subject<object> _subject;

        public Bus()
        {
            _subject = new Subject<object>();
        }

        private void Dispose(bool isDisposing)
        {
            if (_subject != null)
            {
                _subject.Dispose();
                _subject = null;
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

        ~Bus() => Dispose(false);

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return _subject.OfType<TEvent>().AsObservable();
        }

        public void Publish<TEvent>(TEvent sampleEvent)
        {
            _subject.OnNext(sampleEvent);
        }
    }
}
