using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    internal class StreamModel : MeshModel
    {
        private Dictionary<int, Tuple<int, Color>> _indices;
        private Queue<int> _flags;

        internal override void BeginInit()
        {
            base.BeginInit();
            _indices = new Dictionary<int, Tuple<int, Color>>();
            _flags = new Queue<int>();
        }

        internal override bool TryAttachPrimitive(IPrimitive primitive, bool isOutline = true)
        {
            var cnt = 0;
            var geo = (_ComplexGeometry)primitive;
            var subgeos = new List<Tuple<Color, List<PointF>>>();
            foreach (var child in geo.Children.Where(c => c.Filled))
            {
                var subpoints = new List<PointF>();
                subpoints.AddRange(child[isOutline]);
                subgeos.Add(new Tuple<Color, List<PointF>>(child.FillColor.Value, subpoints));
                cnt += subpoints.Count;
            }
            if (_points.Count > 0 && cnt < Capacity && _points.Count + cnt > Capacity)
                return false;

            foreach (var tuple in subgeos)
            {
                _indices.Add(_points.Count, new Tuple<int, Color>(tuple.Item2.Count + 1, tuple.Item1));
                _points.Add(new PointF());
                _points.AddRange(tuple.Item2);
            }
            _flags.Enqueue(subgeos.Count);

            return true;
        }

        internal override void Draw(Shader shader)
        {
            GLFunc.BindVertexArray(_vao[0]);

            var pairs = new List<KeyValuePair<int, Tuple<int, Color>>>();
            var flag = _flags.Dequeue();
            _flags.Enqueue(flag);
            foreach (var index in _indices)
            {
                if (flag > 0)
                {
                    pairs.Add(index);
                    flag--;
                    if (flag == 0)
                    {
                        GLFunc.Clear(GLConst.GL_STENCIL_BUFFER_BIT);

                        GLFunc.ColorMask(GLConst.GL_FALSE, GLConst.GL_FALSE, GLConst.GL_FALSE, GLConst.GL_FALSE);
                        GLFunc.StencilFunc(GLConst.GL_ALWAYS, 0, 1);
                        GLFunc.StencilOp(GLConst.GL_KEEP, GLConst.GL_KEEP, GLConst.GL_INVERT);
                        foreach (var pair in pairs)
                            GLFunc.DrawArrays(GLConst.GL_TRIANGLE_FAN, pair.Key, pair.Value.Item1);

                        GLFunc.ColorMask(GLConst.GL_TRUE, GLConst.GL_TRUE, GLConst.GL_TRUE, GLConst.GL_TRUE);
                        GLFunc.StencilFunc(GLConst.GL_EQUAL, 1, 1);
                        GLFunc.StencilOp(GLConst.GL_KEEP, GLConst.GL_KEEP, GLConst.GL_KEEP);
                        foreach (var pair in pairs)
                        {
                            shader.SetVec4("color", 1, pair.Value.Item2.GetData());
                            GLFunc.DrawArrays(GLConst.GL_TRIANGLE_FAN, pair.Key, pair.Value.Item1);
                        }

                        pairs.Clear();
                        flag = _flags.Dequeue();
                        _flags.Enqueue(flag);
                    }
                }
            }
        }

        protected override void _Dispose()
        {
            base._Dispose();
            _indices?.Clear();
            _indices = null;
            _flags?.Clear();
            _flags = null;
        }
    }
}