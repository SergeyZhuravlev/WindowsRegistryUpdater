using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ReferenceWrapper;

namespace Cached
{
    internal class CacheNode<T> 
    {
        internal CacheNode(T value, Lazy<CacheNode<T>> nodes)
        {
            Value = value;
            Nodes = nodes;
        }

        internal T Value { get; private set; }
        internal Lazy<CacheNode<T>> Nodes { get; private set; }
    }

    public class Cache<T>: IDisposable
    {
        internal Cache(Lazy<CacheNode<T>> nodes, IEnumerable<T> producer, Ref<IEnumerator<T>> subproducer, Object locker)
        {
            Nodes = nodes;
            _subproducer = subproducer;
            _producer = producer;
            _locker = locker;
        }

        internal Lazy<CacheNode<T>> Nodes { get; private set; }
        private IEnumerable<T> _producer;
        private Ref<IEnumerator<T>> _subproducer;
        private object _locker;

        public void Dispose()
        {
            lock(_locker)
                try
                {
                    if (_subproducer.Value != null && _subproducer.Value is IDisposable)
                    {
                        var subproducer = _subproducer.Value;
                        _subproducer.Reset();
                        (subproducer as IDisposable).Dispose();
                    }
                }
                finally
                {
                    if (_producer != null && _producer is IDisposable)
                    {
                        var producer = _producer;
                        _producer = null;
                        (producer as IDisposable).Dispose();
                    }
                }
        }

        public IEnumerable<T> Enumerate
        {
            get
            {
                try
                {
                    var currentNode = this.Nodes.Value;
                    while (currentNode != null)
                    {
                        yield return currentNode.Value;
                        currentNode = currentNode.Nodes.Value;
                    }
                }
                finally
                {
                    this.Dispose();
                }
            }
        }

        internal static CacheNode<T> CacheNodeFactory(IEnumerator<T> subproducer)
        {
            if (subproducer.MoveNext())
            {
                var element = subproducer.Current;
                return new CacheNode<T>(element, new Lazy<CacheNode<T>>(()=> CacheNodeFactory(subproducer), LazyThreadSafetyMode.ExecutionAndPublication));
            }
            return null;
        }
    }

    public static class CachedEnumerable
    {
        public static Cache<T> Make<T>(IEnumerable<T> producer)
        {
            var SoProducer = new Ref<IEnumerator<T>>();
            var locker = new Object();
            return new Cache<T>(new Lazy<CacheNode<T>>(()=>
            {
                lock (locker)
                {
                    var soproducer = producer.GetEnumerator();
                    SoProducer.Assign(soproducer);
                    return Cache<T>.CacheNodeFactory(soproducer);
                }
            }, LazyThreadSafetyMode.ExecutionAndPublication), producer, SoProducer, locker);
        }
    }
}
