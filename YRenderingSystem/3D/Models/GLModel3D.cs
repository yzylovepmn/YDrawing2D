using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YRenderingSystem._3D
{
    [Flags]
    public enum MaterialOption
    {
        Front = 1,
        Back = 2,
        Both = Front | Back
    }

    public abstract class GLModel3D : IHitTestSource, IDisposable
    {
        internal GLModel3D()
        {
        }

        public Rect3F Bounds { get { return _bounds; } }
        protected Rect3F _bounds;

        public GLVisual3D Visual
        {
            get { return _parent != null ? _parent.Visual : _visual; }
            internal set { _visual = value; }
        }
        private GLVisual3D _visual;

        public GLModel3D Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }
        protected GLModel3D _parent;

        internal abstract int UpdateDataIndex(int dataIndex);

        internal abstract void BindingData();

        internal abstract void UpdateBounds();

        internal abstract void UpdateDistance();

        internal abstract void OnRender(Shader shader);

        #region HitTest
        IHitTestSource IHitTestSource.Parent { get { return _parent; } }

        public abstract GLPrimitiveMode Mode { get; set; }

        public abstract float PointSize { get; set; }

        public abstract float LineWidth { get; set; }

        public abstract IEnumerable<DataPair> Pairs { get; }

        public abstract IEnumerable<Point3F> GetHitTestPoints();

        public abstract int GetIndex(int index);
        #endregion

        protected bool _isDisposed;
        public virtual void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
        }
    }
}