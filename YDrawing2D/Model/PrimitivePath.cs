using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct PrimitivePath
    {
        public PrimitivePath(IPrimitive primitive, IEnumerable<Int32Point> path, bool isVirtual = false)
        {
            _primitive = primitive;
            _path = path;
            _isVirtual = isVirtual;
        }

        public IPrimitive Primitive { get { return _primitive; } }
        private IPrimitive _primitive;

        public IEnumerable<Int32Point> Path { get { return _path; } }
        private IEnumerable<Int32Point> _path;

        public bool IsVirtual { get { return _isVirtual; } }
        private bool _isVirtual;
    }
}