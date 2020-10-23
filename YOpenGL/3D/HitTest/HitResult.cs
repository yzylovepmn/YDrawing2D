using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class HitResult
    {
        internal HitResult(IMesh mesh, GLModel3D model, Point3F hitPoint, float zDepth)
        {
            _mesh = mesh;
            _model = model;
            _hitPoint = hitPoint;
            _zDepth = zDepth;
        }

        public IMesh Mesh { get { return _mesh; } }
        private IMesh _mesh;

        public GLModel3D Model { get { return _model; } }
        private GLModel3D _model;

        public Point3F HitPoint { get { return _hitPoint; } }
        private Point3F _hitPoint;

        public float ZDpeth { get { return _zDepth; } }
        private float _zDepth;
    }
}