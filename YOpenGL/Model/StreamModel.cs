using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;

namespace YOpenGL
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
            var subgeos = new List<Tuple<int, Color>>();
            foreach (var child in geo.Children.Where(c => c.Filled))
            {
                var tuple = new Tuple<int, Color>(child[isOutline].Count() + 1, child.FillColor.Value);
                subgeos.Add(tuple);
                cnt += tuple.Item1;
            }
            if (_pointCount > 0 && cnt < Capacity && _pointCount + cnt > Capacity)
                return false;

            var _currentCount = _pointCount;
            foreach (var tuple in subgeos)
            {
                _idx.Add(_currentCount, tuple);
                _currentCount += tuple.Item1;
            }
            _flags.Add(subgeos.Count);
            _pointCount = _currentCount;

            _primitives.Add(new Tuple<IPrimitive, bool, int>(primitive, isOutline, cnt));
            _needUpdate = true;
            return true;
        }

        protected override void _DetachBefore(Tuple<IPrimitive, bool, int> tuple)
        {
            var index = _primitives.IndexOf(tuple);
            if (index >= 0)
            {
                int skipCount = 0, removeCount = _flags[index];
                for (int i = 0; i < index; i++)
                    skipCount += _flags[i];
                _flags.RemoveAt(index);

                var ahead = _idx.Take(skipCount);
                var idx = new Dictionary<int, Tuple<int, Color>>();
                foreach (var pair in _idx.ToList())
                {
                    if (skipCount == 0)
                    {
                        if (removeCount > 0)
                            removeCount--;
                        else idx.Add(pair.Key - tuple.Item3, pair.Value);
                    }
                    else
                    {
                        idx.Add(pair.Key, pair.Value);
                        skipCount--;
                    }
                }
                _idx = idx;
            }
            base._DetachBefore(tuple);
        }

        protected override float[] GenVertice(List<uint> indices = null)
        {
            var points = new List<PointF>();

            foreach (var tuple in _primitives)
            {
                var geo = (_ComplexGeometry)tuple.Item1;
                foreach (var child in geo.Children.Where(c => c.Filled))
                {
                    points.Add(new PointF());
                    points.AddRange(child[tuple.Item2]);
                }
            }

            return points.GetData();
        }

        internal override void Draw(Shader shader)
        {
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