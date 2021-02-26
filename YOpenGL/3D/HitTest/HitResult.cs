using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class HitResult
    {
        internal HitResult(IMesh mesh, IHitTestSource hitModel, float zDepth)
        {
            _mesh = mesh;
            _hitModel = hitModel;
            _zDepth = zDepth;
        }

        public IMesh Mesh { get { return _mesh; } }
        private IMesh _mesh;

        public IHitTestSource HitModel { get { return _hitModel; } }
        private IHitTestSource _hitModel;

        public float ZDepth { get { return _zDepth; } }
        private float _zDepth;
    }
}