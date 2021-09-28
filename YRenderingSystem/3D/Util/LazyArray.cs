using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YRenderingSystem._3D
{
    public class LazyArray<T, TResult> where T : struct where TResult : struct
    {
        public LazyArray(IEnumerable<T> source, Func<T, TResult> init)
        {
            _source = source.ToArray();
            _init = init;
            _values = new TResult?[_source.Length];
        }

        private T[] _source;
        private Func<T, TResult> _init;
        private TResult?[] _values;

        public TResult this[int index]
        {
            get
            {
                var t = _values[index];
                if (!t.HasValue)
                    t = _init(_source[index]);
                return t.Value;
            }
        }

        public int Length { get { return _source.Length; } }
    }
}