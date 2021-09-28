using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YGeometry.Maths
{
    [Serializable]
    public struct IndexN<T> : IEnumerable<T> where T : struct
    {
        public IndexN(IEnumerable<T> indice)
        {
            _indice = indice.ToArray();
        }

        public IndexN(params T[] indice)
        {
            _indice = indice.ToArray();
        }

        public T this[int index] 
        {
            get { return _indice[index]; }
            set { _indice[index] = value; }
        }

        private T[] _indice;

        public int Length { get { return _indice.Length; } }

        public IEnumerator<T> GetEnumerator()
        {
            return _indice.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _indice.GetEnumerator();
        }
    }
}