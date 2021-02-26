using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;
using static YOpenGL.GL;
using System.ComponentModel;

namespace YOpenGL._3D
{
    [Serializable]
    public struct DataPair
    {
        public DataPair(int start, int count)
        {
            _start = start;
            _count = count;
        }

        public int Start { get { return _start; } }
        private int _start;

        public int Count { get { return _count; } }
        private int _count;
    }

    public class GLMeshModel3D : GLModel3D
    {
        public GLMeshModel3D() : this(null, null, null, null)
        {

        }

        public GLMeshModel3D(IEnumerable<Point3F> points, IEnumerable<Vector3F> normals, IEnumerable<PointF> textureCoordinates, IEnumerable<uint> indices)
        {
            _points = points?.ToList();
            _normals = normals?.ToList();
            _textureCoordinates = textureCoordinates?.ToList();
            _indices = indices?.ToList();
            _materials = new List<Material>();
            _backMaterials = new List<Material>();

            _pointSize = 1f;
            _lineWidth = 1f;
            _mode = GLPrimitiveMode.GL_TRIANGLES;
        }

        internal int DataIndex { get { return _dataIndex; } set { _dataIndex = value; } }
        private int _dataIndex;

        public IEnumerable<Point3F> Points { get { return _points; } }
        private List<Point3F> _points;

        internal int PointCount { get { return _points == null ? 0 : _points.Count; } }

        public IEnumerable<Vector3F> Normals { get { return _normals; } }
        private List<Vector3F> _normals;

        public IEnumerable<PointF> TextureCoordinates { get { return _textureCoordinates; } }
        private List<PointF> _textureCoordinates;

        public IEnumerable<uint> Indices { get { return _indices; } }
        private List<uint> _indices;

        public override IEnumerable<DataPair> Pairs { get { return _pairs; } }
        private List<DataPair> _pairs;

        private List<Material> _materials;
        private List<Material> _backMaterials;

        public override GLPrimitiveMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    Visual?.Viewport?.Refresh();
                }
            }
        }
        private GLPrimitiveMode _mode;

        public override float PointSize
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
                    if (_mode == GLPrimitiveMode.GL_POINTS)
                        Visual?.Viewport?.Refresh();
                }
            }
        }
        private float _pointSize;

        public override float LineWidth
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
                    if (_mode == GLPrimitiveMode.GL_LINES
                        || _mode == GLPrimitiveMode.GL_LINE_LOOP
                        || _mode == GLPrimitiveMode.GL_LINE_STRIP)
                        Visual?.Viewport?.Refresh();
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
                    if (_distances == null && Visual?.Viewport != null)
                        UpdateDistance();
                    Visual?.Viewport?.Refresh();
                }
            }
        }
        private byte[] _dashes;
        private List<float> _distances;

        public bool HasDash { get { return (_mode == GLPrimitiveMode.GL_LINES || _mode == GLPrimitiveMode.GL_LINE_STRIP) && _dashes != null && _indices == null; } }

        public void SetPoints(IEnumerable<Point3F> points)
        {
            _points = points?.ToList();
            UpdateBounds();
            _UpdateDistance();
            Visual?.InvalidateData();
        }

        private void _BindingPoints()
        {
            if (Visual == null) return;
            var offset = _dataIndex * 3 * sizeof(float);
            var size = PointCount * 3 * sizeof(float);
            BufferSubData(GL_ARRAY_BUFFER, offset, size, _points?.GetData());
        }

        public void SetNormals(IEnumerable<Vector3F> normals)
        {
            _normals = normals?.ToList();
            _BindingNormals(true);
            //Visual?.Viewport?.Refresh();
        }

        private void _BindingNormals(bool bindBuffer = false)
        {
            if (Visual == null) return;
            if (bindBuffer)
                Visual.BufferBinding();
            var offset = _dataIndex * 3 * sizeof(float) + Visual.PointCount * 3 * sizeof(float);
            var count = Math.Min(PointCount, _normals == null ? 0 : _normals.Count);
            var size = count * 3 * sizeof(float);
            BufferSubData(GL_ARRAY_BUFFER, offset, size, _normals?.TakeOrDefault(count).GetData());
        }

        public void SetTextureCoordinates(IEnumerable<PointF> textureCoordinates)
        {
            _textureCoordinates = textureCoordinates?.ToList();
            _BindingTextureCoordinates(true);
            //Visual?.Viewport?.Refresh();
        }

        private void _BindingTextureCoordinates(bool bindBuffer = false)
        {
            if (Visual == null) return;
            if (bindBuffer)
                Visual.BufferBinding();
            var offset = _dataIndex * 2 * sizeof(float) + Visual.PointCount * 6 * sizeof(float);
            var count = Math.Min(PointCount, _textureCoordinates == null ? 0 : _textureCoordinates.Count);
            var size = count * 2 * sizeof(float);
            BufferSubData(GL_ARRAY_BUFFER, offset, size, _textureCoordinates?.TakeOrDefault(count).GetData());
        }

        public void SetIndices(IEnumerable<uint> indices)
        {
            _indices = indices?.ToList();
            UpdateBounds();
            //Visual?.Viewport?.Refresh();
        }

        internal override void UpdateDistance()
        {
            _UpdateDistance();
            _BindingDistance(true);
        }

        private void _UpdateDistance()
        {
            if (Visual?.Viewport == null) return;
            var viewport = Visual.Viewport;

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
                                var p1 = viewport.Point3DToPointInWpf(points[i]);
                                var p2 = viewport.Point3DToPointInWpf(points[i + 1]);
                                distances[i] = 0;
                                distances[i + 1] = (p2 - p1).Length;
                            }
                        }
                        break;
                    case GLPrimitiveMode.GL_LINE_STRIP:
                        {
                            var dis = 0f;
                            var pointsTransformed = new LazyArray<Point3F, PointF>(points, p => viewport.Point3DToPointInWpf(p));

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
        }

        private void _BindingDistance(bool bindBuffer = false)
        {
            if (Visual == null) return;
            if (bindBuffer)
                Visual.BufferBinding();
            var offset = _dataIndex * sizeof(float) + Visual.PointCount * 8 * sizeof(float);
            var size = _distances == null ? 0 : _distances.Count * sizeof(float);
            BufferSubData(GL_ARRAY_BUFFER, offset, size, _distances?.ToArray());
        }

        public void SetPairs(IEnumerable<DataPair> pairs = null)
        {
            _pairs = pairs?.ToList();
            UpdateBounds();
            //Visual?.Viewport?.Refresh();
        }

        #region Material
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
            //Visual?.Viewport?.Refresh();
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
            //Visual?.Viewport?.Refresh();
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
            Visual?.Viewport?.Refresh();
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

        internal override int UpdateDataIndex(int dataIndex)
        {
            _dataIndex = dataIndex;
            return dataIndex += PointCount;
        }

        internal override void BindingData()
        {
            if (Visual == null) return;
            _BindingPoints();
            _BindingNormals();
            _BindingTextureCoordinates();
            _BindingDistance();
        }

        internal override void OnRender(Shader shader)
        {
            shader.SetBool("dashed", HasDash);
            if (HasDash)
            {
                shader.SetFloat("dashedFactor", _dashes.Length * 2);
                TexImage1D(GL_TEXTURE_1D, 0, GL_RED, _dashes.Length, 0, GL_RED, GL_UNSIGNED_BYTE, _dashes);
            }

            GLFunc.PointSize(_pointSize);
            GLFunc.LineWidth(_lineWidth);

            _SetMaterialData(shader);

            var hasPoints = _points != null;
            var hasIndices = _indices != null;
            var mode = (uint)_mode;
            if (hasIndices)
            {
                BufferData(GL_ELEMENT_ARRAY_BUFFER, _indices.Count * sizeof(uint), _indices.Select(index => index + (uint)_dataIndex).ToArray(), GL_DYNAMIC_DRAW);
                DrawElements(mode, hasPoints ? _indices.Count : 0, GL_UNSIGNED_INT, 0);
            }
            else
            {
                if (_pairs == null)
                    DrawArrays(mode, _dataIndex, hasPoints ? _points.Count : 0);
                else MultiDrawArrays(mode, _pairs.Select(pair => pair.Start + _dataIndex).ToArray(), _pairs.Select(pair => pair.Count).ToArray(), _pairs.Count);
            }
        }

        public override int GetIndex(int index)
        {
            if (_indices != null)
                return (int)_indices[index];
            return index;
        }

        public override IEnumerable<Point3F> GetHitTestPoints()
        {
            if (_points == null) return null;
            if (_indices == null) return _points;

            var points = new List<Point3F>();
            foreach (var index in _indices)
                if (index < _points.Count)
                    points.Add(_points[(int)index]);
            return points;
        }

        private IEnumerable<Point3F> _GetDrawPoints()
        {
            if (_points == null) return null;
            var points = new List<Point3F>();
            if (_indices == null)
            {
                if (_pairs == null)
                    return _points;
                else
                {
                    foreach (var pair in _pairs)
                    {
                        for (int i = 0; i < pair.Count; i++)
                            points.Add(_points[pair.Start + i]);
                    }
                }
            }
            else
            {
                foreach (var index in _indices)
                    if (index < _points.Count)
                        points.Add(_points[(int)index]);
            }
            return points;
        }

        internal override void UpdateBounds()
        {
            _bounds = Rect3F.Empty;
            var points = _GetDrawPoints();
            if (points != null)
                foreach (var point in points)
                    _bounds.Union(point);
            _parent?.UpdateBounds();
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
    }
}