using System;

namespace Gumbo
{
    internal class LazyFactory
    {
        readonly Func<bool> _disposed;
        readonly string _objectName;

        public LazyFactory(Func<bool> disposed, string objectName)
        {
            _disposed = disposed ?? throw new ArgumentNullException(nameof(disposed));
            _objectName = objectName ?? throw new ArgumentNullException(nameof(objectName));
        }

        public Lazy<T> Create<T>(Func<T> factoryMethod) => new Lazy<T>(() =>
        {
            if (_disposed())
                throw new ObjectDisposedException(_objectName);
            return factoryMethod();
        });
    }
}
