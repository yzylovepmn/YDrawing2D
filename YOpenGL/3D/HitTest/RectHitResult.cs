using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class RectHitResult
    {
        internal RectHitResult(GLModel3D hitModel)
        {
            _visual = hitModel.Visual;
            _hitModel = hitModel;
        }

        public GLVisual3D Visual { get { return _visual; } }
        private GLVisual3D _visual;

        public GLModel3D HitModel { get { return _hitModel; } }
        private GLModel3D _hitModel;
    }
}