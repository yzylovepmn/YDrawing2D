using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static YRenderingSystem.GLFunc;
using static YRenderingSystem.GLConst;

namespace YRenderingSystem
{
    public class StreamModel : MeshModel
    {
        internal StreamModel() { }

        private Dictionary<int, Tuple<int, Color>> _idx;
        private List<int> _flags;

        internal override void BeginInit()
        {
            base.BeginInit();
            _idx = new Dictionary<int, Tuple<int, Color>>();
            _flags = new List<int>();
        }

        internal override bool TryAttachPrimitive(IPrimitive primitive, bool isOutline = true)
        {
            var cnt = 0;
            var geo = (_ComplexGeometry)primitive;
            foreach (var child in geo.Children.Where(c => c.Filled))
                cnt += child[isOutline].Count();
            if (_pointCount > 0 && cnt < Capacity && _pointCount + cnt > Capacity)
                return false;

            _pointCount += cnt;
            _primitives.Add(primitive, new Tuple<bool, int>(isOutline, cnt));
            _needUpdate = true;
            return true;
        }

        protected override void _BeforeEnd()
        {
            base._BeforeEnd();
            if (_needUpdate)
            {
                _idx.Clear();
                _flags.Clear();
                var cnt = 0;
                foreach (var pair in _primitives)
                {
                    var geo = (_ComplexGeometry)pair.Key;
                    var children = geo.Children.Where(c => c.Filled);
                    foreach (var child in children)
                    {
                        var _tuple = new Tuple<int, Color>(child[pair.Value.Item1].Count(), child.FillColor.Value);
                        _idx.Add(cnt, _tuple);
                        cnt += _tuple.Item1;
                    }
                    _flags.Add(children.Count());
                }
            }
        }

        protected override float[] GenVertice()
        {
            var points = new List<PointF>();

            foreach (var pair in _primitives)
            {
                var geo = (_ComplexGeometry)pair.Key;
                foreach (var child in geo.Children.Where(c => c.Filled))
                    points.AddRange(child[pair.Value.Item1]);
            }

            return points.GetData();
        }

        internal override void Draw(Shader shader)
        {
            if (!_hasInit) return;
            BindVertexArray(_vao[0]);

            var pairs = new List<KeyValuePair<int, Tuple<int, Color>>>();
            var cnt = 0;
            var flag = _flags[cnt++];
            foreach (var index in _idx)
            {
                if (flag > 0)
                {
                    pairs.Add(index);
                    flag--;
                    if (flag == 0)
                    {
                        ColorMask(GL_FALSE, GL_FALSE, GL_FALSE, GL_FALSE);
                        StencilFunc(GL_ALWAYS, 0, 1);
                        StencilOp(GL_ZERO, GL_ZERO, GL_INVERT);
                        foreach (var pair in pairs)
                            DrawArrays(GL_TRIANGLE_FAN, pair.Key, pair.Value.Item1);

                        ColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
                        StencilFunc(GL_EQUAL, 1, 1);
                        StencilOp(GL_ZERO, GL_ZERO, GL_ZERO);
                        foreach (var pair in pairs)
                        {
                            shader.SetVec4("color", 1, pair.Value.Item2.GetData());
                            DrawArrays(GL_TRIANGLE_FAN, pair.Key, pair.Value.Item1);
                        }

                        pairs.Clear();

                        if (cnt < _flags.Count)
                            flag = _flags[cnt++];
                    }
                }
            }
        }

        protected override void _Dispose()
        {
            base._Dispose();
            _idx?.Clear();
            _idx = null;
            _flags?.Clear();
            _flags = null;
        }
    }
}