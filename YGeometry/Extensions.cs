using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YGeometry
{
    public static class Extensions
    {
        public static void Dispose<T>(this IEnumerable<T> elements) where T : IDisposable
        {
            if (elements == null) return;
            foreach (var ele in elements)
                ele.Dispose();
        }
    }
}