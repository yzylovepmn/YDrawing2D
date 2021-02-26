using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class GLModel3DGroup : GLModel3D
    {
        public GLModel3DGroup()
        {
            _children = new List<GLModel3D>();
        }

        public IEnumerable<GLModel3D> Children { get { return _children; } }
        private List<GLModel3D> _children;

        public void AddChilds(IEnumerable<GLModel3D> models)
        {
            foreach (var model in models)
                _AddChild(model);
            _OnChildrenChanged();
        }

        public void AddChild(GLModel3D model)
        {
            _AddChild(model);
            _OnChildrenChanged();
        }

        private void _AddChild(GLModel3D model)
        {
            if (model.Parent != null)
                throw new InvalidOperationException("The model has a logical parent");
            if (model.Visual != null)
                throw new InvalidOperationException("The model has associated visual");
            _children.Add(model);
            model.Parent = this;
        }

        public void RemoveAllChilds()
        {
            foreach (var model in _children)
                _RemoveChild(model);
            _OnChildrenChanged();
        }

        public void RemoveChilds(IEnumerable<GLModel3D> models)
        {
            foreach (var model in models)
                _RemoveChild(model);
            _OnChildrenChanged();
        }

        public void RemoveChild(GLModel3D model)
        {
            _RemoveChild(model);
            _OnChildrenChanged();
        }

        private void _RemoveChild(GLModel3D model)
        {
            if (model.Parent != this)
                throw new InvalidOperationException("The logical parent is error");
            _children.Remove(model);
            model.Parent = null;
        }

        private void _OnChildrenChanged()
        {
            UpdateBounds();
            Visual?.InvalidateData();
        }

        internal override int UpdateDataIndex(int dataIndex)
        {
            foreach (var child in _children)
                dataIndex = child.UpdateDataIndex(dataIndex);
            return dataIndex;
        }

        internal override void BindingData()
        {
            _children.ForEach(child => child.BindingData());
        }

        internal override void UpdateDistance()
        {
            _children.ForEach(child => child.UpdateDistance());
        }

        internal override void UpdateBounds()
        {
            _bounds = Rect3F.Empty;
            foreach (var child in _children)
                _bounds.Union(child.Bounds);
            _parent?.UpdateBounds();
        }

        internal override void OnRender(Shader shader)
        {
            _children.ForEach(child => child.OnRender(shader));
        }

        #region HitTest
        public override GLPrimitiveMode Mode { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override float PointSize { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override float LineWidth { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override IEnumerable<DataPair> Pairs => throw new NotImplementedException();

        public override IEnumerable<Point3F> GetHitTestPoints()
        {
            throw new NotImplementedException();
        }

        public override int GetIndex(int index)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override void Dispose()
        {
            if (_isDisposed) return;
            _children.ForEach(child => child.Dispose());
            _children = null;
            base.Dispose();
        }
    }
}