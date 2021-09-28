using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YRenderingSystem.GLFunc;
using static YRenderingSystem.GLConst;
using static YRenderingSystem.GL;
using System.ComponentModel;

namespace YRenderingSystem._3D
{
    public class GLModel3D_Old : IDisposable
    {
        public GLModel3D_Old() : this(null, null, null, null)
        {
        }

        public GLModel3D_Old(IEnumerable<Point3F> points, IEnumerable<Vector3F> normals, IEnumerable<PointF> textureCoordinates, IEnumerable<uint> indices)
        {
            _points = points?.ToList();
            _normals = normals?.ToList();
            _textureCoordinates = textureCoordinates?.ToList();
            _indices = indices?.ToList();
            _materials = new List<Material>();
            _backMaterials = new List<Material>();

            _pointSize = 1f;
            _lineWidth = 1f;
            _isVisible = true;
            _isHitTestVisible = true;
            //_isVolumeObject = true;
            _mode = GLPrimitiveMode.GL_TRIANGLES;
        }

        public GLPanel3D Viewport
        {
            get { return _viewport; }
            internal set { _viewport = value; }
        }
        private GLPanel3D _viewport;

        public GLPrimitiveMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    _viewport?.Refresh();
                }
            }
        }
        private GLPrimitiveMode _mode;

        public Rect3F Bounds { get { return _bounds; } }
        private Rect3F _bounds;

        public float PointSize
        {
            get { return _pointSize; }
            set
            {
                var newValue = value;
                if (PointSizeRange != null)
                    MathUtil.Clamp(ref newValue, PointSizeRange[0], PointSizeRange[1]);
                if (_pointSize != newValue)
                {
                    _pointSize = newValue;
                    _viewport?.Refresh();
                }
            }
        }
        private float _pointSize;

        public float LineWidth
        {
            get { return _lineWidth; }
            set
            {
                var newValue = value;
                if (LineWidthRange != null)
                    MathUtil.Clamp(ref newValue, LineWidthRange[0], LineWidthRange[1]);
                if (_lineWidth != newValue)
                {
                    _lineWidth = newValue;
                    _viewport?.Refresh();
                }
            }
        }
        private float _lineWidth;

        public byte[] Dashes
        {
            internal get { return _dashes; }
            set
            {
                var newDash = _GenData(value);
                if (newDash == null && _dashes == null) return;
                if (newDash == null || _dashes == null || !newDash.SequenceEqual(_dashes))
                {
                    _dashes = newDash;
                    if (_distances == null && _viewport != null)
                        UpdateDistance();
                    _viewport?.Refresh();
                }
            }
        }
        private byte[] _dashes;
        private List<float> _distances;

        public bool HasDash { get { return (_mode == GLPrimitiveMode.GL_LINES || _mode == GLPrimitiveMode.GL_LINE_STRIP) && _dashes != null && _indices == null; } }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    _viewport?.Refresh();
                }
            }
        }
        private bool _isVisible;

        //public bool IsVolumeObject { get { return _isVolumeObject; } set { _isVolumeObject = value; } }
        //private bool _isVolumeObject;

        //public Shader CustomShader
        //{
        //    get { return _customShader; }
        //    set { _customShader = value; }
        //}
        //private Shader _customShader;

        public bool IsHitTestVisible { get { return _isHitTestVisible; } set { _isHitTestVisible = value; } }
        private bool _isHitTestVisible;

        internal bool HasInit { get { return _vao != null; } }

        private uint[] _vao;
        private uint[] _vbo;
        private uint[] _ebo;

        public IEnumerable<Point3F> Points { get { return _points; } }
        private List<Point3F> _points;

        public IEnumerable<Vector3F> Normals { get { return _normals; } }
        private List<Vector3F> _normals;

        public IEnumerable<PointF> TextureCoordinates { get { return _textureCoordinates; } }
        private List<PointF> _textureCoordinates;

        public IEnumerable<uint> Indices { get { return _indices; } }
        private List<uint> _indices;

        private List<Material> _materials;
        private List<Material> _backMaterials;

        internal void Init()
        {
            if (HasInit) return;

            _vao = new uint[1];
            _vbo = new uint[1];
            _ebo = new uint[1];
            GenVertexArrays(1, _vao);
            GenBuffers(1, _vbo);
            GenBuffers(1, _ebo);

            UpdateDistance();
            _IndicesBinding();

            MathUtil.Clamp(ref _pointSize, PointSizeRange[0], PointSizeRange[1]);
            MathUtil.Clamp(ref _lineWidth, LineWidthRange[0], LineWidthRange[1]);
        }

        internal void Clean()
        {
            if (!HasInit) return;

            if (_vao != null)
                DeleteVertexArrays(1, _vao);
            if (_vbo != null)
                DeleteBuffers(1, _vbo);
            if (_ebo != null)
                DeleteBuffers(1, _ebo);
            _vao = null;
            _vbo = null;
            _ebo = null;
        }

        public void SetPoints(IEnumerable<Point3F> points)
        {
            _points = points?.ToList();
            _UpdateBounds();
            if (HasInit)
            {
                _viewport.MakeSureCurrentContext();
                UpdateDistance();
            }
        }

        public void SetNormals(IEnumerable<Vector3F> normals)
        {
            _normals = normals?.ToList();
            if (HasInit)
            {
                _viewport.MakeSureCurrentContext();
                _DataBinding();
            }
        }

        public void SetTextureCoordinates(IEnumerable<PointF> textureCoordinates)
        {
            _textureCoordinates = textureCoordinates?.ToList();
            if (HasInit)
            {
                _viewport.MakeSureCurrentContext();
                _DataBinding();
            }
        }

        public void SetIndices(IEnumerable<uint> indices)
        {
            _indices = indices?.ToList();
            _UpdateBounds();
            if (HasInit)
            {
                _viewport.MakeSureCurrentContext();
                _IndicesBinding();
            }
        }

        internal void InvalidateDistance()
        {
            _distances = null;
        }

        internal void UpdateDistance()
        {
            if (HasDash && _points != null)
            {
                var points = _points.ToArray();
                var distances = new float[points.Length];
                switch (_mode)
                {
                    case GLPrimitiveMode.GL_LINES:
                        {
                            for (int i = 0; i < points.Length - 1; i += 2)
                            {
                                var p1 = _viewport.Point3DToPointInWpf(points[i]);
                                var p2 = _viewport.Point3DToPointInWpf(points[i + 1]);
                                distances[i] = 0;
                                distances[i + 1] = (p2 - p1).Length;
                            }
                        }
                        break;
                    case GLPrimitiveMode.GL_LINE_STRIP:
                        {
                            var dis = 0f;
                            var pointsTransformed = new LazyArray<Point3F, PointF>(points, p => _viewport.Point3DToPointInWpf(p));

                            for (int i = 1; i < pointsTransformed.Length; i++)
                            {
                                var p1 = pointsTransformed[i - 1];
                                var p2 = pointsTransformed[i];
                                dis += (p2 - p1).Length;
                                distances[i] = dis;
                            }
                        }
                        break;
                }
                _distances = distances.ToList();
            }
            else _distances = null;

            _DataBinding();
        }

        private void _DataBinding()
        {
            BindVertexArray(_vao[0]);
            BindBuffer(GL_ARRAY_BUFFER, _vbo[0]);
            var size = _CalcDataSize();
            BufferData(GL_ARRAY_BUFFER, size * sizeof(float), default(float[]), GL_STATIC_DRAW);
            var offset = 0;

            if (_points != null)
            {
                size = _points.Count * 3 * sizeof(float);
                BufferSubData(GL_ARRAY_BUFFER, offset, size, _points.GetData());

                EnableVertexAttribArray(0);
                VertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), offset);

                offset += size;
            }
            else DisableVertexAttribArray(0);

            if (_normals != null)
            {
                size = _normals.Count * 3 * sizeof(float);
                BufferSubData(GL_ARRAY_BUFFER, offset, size, _normals.GetData());

                EnableVertexAttribArray(1);
                VertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), offset);

                offset += size;
            }
            else DisableVertexAttribArray(1);

            if (_textureCoordinates != null)
            {
                size = _textureCoordinates.Count * 2 * sizeof(float);
                BufferSubData(GL_ARRAY_BUFFER, offset, size, _textureCoordinates.GetData());

                EnableVertexAttribArray(2);
                VertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), offset);

                offset += size;
            }
            else DisableVertexAttribArray(2);

            if (_distances != null)
            {
                size = _distances.Count * sizeof(float);
                BufferSubData(GL_ARRAY_BUFFER, offset, size, _distances.ToArray());

                EnableVertexAttribArray(3);
                VertexAttribPointer(3, 1, GL_FLOAT, GL_FALSE, sizeof(float), offset);

                offset += size;
            }
            else DisableVertexAttribArray(3);
        }

        private void _IndicesBinding()
        {
            BindVertexArray(_vao[0]);
            
            if (_indices == null)
                BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
            else
            {
                BindBuffer(GL_ELEMENT_ARRAY_BUFFER, _ebo[0]);
                BufferData(GL_ELEMENT_ARRAY_BUFFER, _indices.Count * sizeof(uint), _indices.ToArray(), GL_STATIC_DRAW);
            }
        }

        private int _CalcDataSize()
        {
            return (_points == null ? 0 : _points.Count * 3) + (_normals == null ? 0 : _normals.Count * 3) + (_textureCoordinates == null ? 0 : _textureCoordinates.Count * 2) + (_distances == null ? 0 : _distances.Count);
        }

        private void _UpdateBounds()
        {
            _bounds = Rect3F.Empty;
            if (!_isVisible) return;
            var points = GetDrawPoints();
            if (points != null)
                foreach (var point in points)
                    _bounds.Union(point);
        }

        #region Materia
        /// <summary>
        /// 每个种类的材质只能添加一次
        /// </summary>
        public void AddMaterial(Material material, MaterialOption option)
        {
            if ((option & MaterialOption.Front) == MaterialOption.Front)
                _AddMaterial(material);
            if ((option & MaterialOption.Back) == MaterialOption.Back)
                _AddBackMaterial(material);
            if (option != 0)
                material.PropertyChanged += _OnMaterialChanged;
            _viewport?.Refresh();
        }

        private void _AddMaterial(Material material)
        {
            if (_materials.Contains(material)) throw new InvalidOperationException("material has been added!");
            if (_materials.Exists(mat => mat.Type == material.Type)) throw new InvalidOperationException($"{nameof(material.Type)} material has been added!");
            _materials.Add(material);
        }

        private void _AddBackMaterial(Material backMaterial)
        {
            if (_backMaterials.Contains(backMaterial)) throw new InvalidOperationException("material has been added!");
            if (_backMaterials.Exists(mat => mat.Type == backMaterial.Type)) throw new InvalidOperationException($"{nameof(backMaterial.Type)} material has been added!");
            _backMaterials.Add(backMaterial);
        }

        public void RemoveMaterial(Material material, MaterialOption option)
        {
            if ((option & MaterialOption.Front) == MaterialOption.Front)
                _RemoveMaterial(material);
            if ((option & MaterialOption.Back) == MaterialOption.Back)
                _RemoveBackMaterial(material);
            if (option != 0)
                material.PropertyChanged -= _OnMaterialChanged;
            _viewport?.Refresh();
        }

        private void _RemoveMaterial(Material material)
        {
            if (!_materials.Contains(material)) throw new InvalidOperationException("material does not exist!");
            _materials.Remove(material);
        }

        private void _RemoveBackMaterial(Material backMaterial)
        {
            if (!_backMaterials.Contains(backMaterial)) throw new InvalidOperationException("material does not exist!");
            _backMaterials.Remove(backMaterial);
        }

        private void _OnMaterialChanged(object sender, PropertyChangedEventArgs e)
        {
            _viewport?.Refresh();
        }

        private void _SetMaterialData(Shader shader)
        {
            shader.SetBool("material.hasEmissive", false);
            shader.SetBool("material.hasDiffuse", false);
            shader.SetBool("material.hasSpecular", false);
            foreach (var material in _materials)
            {
                var data = material.Color.GetData();
                switch (material.Type)
                {
                    case MaterialType.Emissive:
                        shader.SetVec4("material.emissive", 1, data);
                        shader.SetBool("material.hasEmissive", true);
                        break;
                    case MaterialType.Diffuse:
                        shader.SetVec4("material.diffuse", 1, data);
                        shader.SetBool("material.hasDiffuse", true);
                        break;
                    case MaterialType.Specular:
                        shader.SetVec4("material.specular", 1, data);
                        shader.SetFloat("material.shininess", (material as SpecularMaterial).SpecularPower);
                        shader.SetBool("material.hasSpecular", true);
                        break;
                }
            }

            shader.SetBool("materialBack.hasEmissive", false);
            shader.SetBool("materialBack.hasDiffuse", false);
            shader.SetBool("materialBack.hasSpecular", false);
            foreach (var material in _backMaterials)
            {
                var data = material.Color.GetData();
                switch (material.Type)
                {
                    case MaterialType.Emissive:
                        shader.SetVec4("materialBack.emissive", 1, data);
                        shader.SetBool("materialBack.hasEmissive", true);
                        break;
                    case MaterialType.Diffuse:
                        shader.SetVec4("materialBack.diffuse", 1, data);
                        shader.SetBool("materialBack.hasDiffuse", true);
                        break;
                    case MaterialType.Specular:
                        shader.SetVec4("materialBack.specular", 1, data);
                        shader.SetFloat("materialBack.shininess", (material as SpecularMaterial).SpecularPower);
                        shader.SetBool("materialBack.hasSpecular", true);
                        break;
                }
            }
        }
        #endregion

        #region Draw
        protected internal virtual void OnRender(Shader shader)
        {
            if (!_isVisible) return;

            shader.SetBool("dashed", HasDash);

            if (HasDash)
            {
                shader.SetFloat("dashedFactor", _dashes.Length * 2);
                TexImage1D(GL_TEXTURE_1D, 0, GL_RED, _dashes.Length, 0, GL_RED, GL_UNSIGNED_BYTE, _dashes);
            }

            GLFunc.PointSize(_pointSize);
            GLFunc.LineWidth(_lineWidth);

            _SetMaterialData(shader);
            BindVertexArray(_vao[0]);
            var hasPoints = _points != null;
            var hasIndices = _indices != null;
            var mode = (uint)_mode;
            if (hasIndices)
                DrawElements(mode, hasPoints ? _indices.Count : 0, GL_UNSIGNED_INT, 0);
            else DrawArrays(mode, 0, hasPoints ? _points.Count : 0);
        }
        #endregion

        internal IEnumerable<Point3F> GetDrawPoints()
        {
            if (_points == null) return null;
            if (_indices == null) return _points;

            var points = new List<Point3F>();
            foreach (var index in _indices)
                if (index < _points.Count)
                    points.Add(_points[(int)index]);
            return points;
        }

        private static byte[] _GenData(byte[] dashes)
        {
            if (dashes == null) return null;

            var data = new List<byte>();

            var flag = true;
            foreach (var value in dashes)
            {
                for (int i = value; i > 0; i--)
                    data.Add(flag ? (byte)255 : (byte)0);
                flag = !flag;
            }

            return data.ToArray();
        }

        public virtual void Dispose()
        {
            Clean();
            _materials = null;
            _viewport = null;
        }
    }
}