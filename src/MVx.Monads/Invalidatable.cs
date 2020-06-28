using System;

namespace MVx.Monads
{
    /// <summary>
    /// Represents a lazily evaluated value that can be invalidated to force re-evaluation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Invalidatable<T>
    {
        private Func<T> _create;
        private Action<T> _dispose;
        private Lazy<T> _lazy;

        public Invalidatable(Func<T> create, Action<T> dispose = null)
        {
            _create = create;
            _dispose = dispose;

            RecreateLazy();
        }

        private void RecreateLazy()
        {
            if (_lazy?.IsValueCreated ?? false && _dispose != null)
            {
                _dispose(_lazy.Value);
            }

            _lazy = new Lazy<T>(_create);
        }

        public void Invalidate()
        {
            RecreateLazy();
        }

        public T Value { get; }
    }
}
