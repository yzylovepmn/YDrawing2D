using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class RectHitResult
    {
        public RectHitResult(GLModel3D model)
        {
            _model = model;
        }

        public GLModel3D Model { get { return _model; } }
        private GLModel3D _model;
    }
}