using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YRenderingSystem._3D
{
    public class RectHitResult
    {
        internal RectHitResult(IHitTestSource hitModel)
        {
            _hitModel = hitModel;
        }
        public IHitTestSource HitModel { get { return _hitModel; } }
        private IHitTestSource _hitModel;
    }
}