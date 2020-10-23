﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class LazyArray<T> where T : struct
    {
        public LazyArray(IEnumerable<T> source, Func<T, T> init)
        {
            _source = source.ToArray();
            _init = init;
            _values = new T?[_source.Length];
        }

        private T[] _source;
        private Func<T, T> _init;
        private T?[] _values;

        public T this[int index]
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