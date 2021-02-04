﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class HitResult
    {
        internal HitResult(IMesh mesh, GLModel3D hitModel, float zDepth)
        {
            _mesh = mesh;
            _visual = hitModel.Visual;
            _hitModel = hitModel;
            _zDepth = zDepth;
        }

        public IMesh Mesh { get { return _mesh; } }
        private IMesh _mesh;

        public GLVisual3D Visual { get { return _visual; } }
        private GLVisual3D _visual;

        public GLModel3D HitModel { get { return _hitModel; } }
        private GLModel3D _hitModel;

        public float ZDepth { get { return _zDepth; } }
        private float _zDepth;
    }
}