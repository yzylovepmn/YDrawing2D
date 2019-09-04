using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;
using System.Windows.Media;

namespace YOpenGL
{
    public struct PointPair : IComparable<PointPair>
    {
        public PointPair(Color color, float pointSize)
        {
            Color = color;
            PointSize = pointSize;
        }

        public Color Color;
        public float PointSize;

        public static bool operator ==(PointPair p1, PointPair p2)
        {
            return p1.Color == p2.Color && p1.PointSize == p2.PointSize;
        }

        public static bool operator !=(PointPair p1, PointPair p2)
        {
            return !(p1 == p2);
        }

        public int CompareTo(PointPair other)
        {
            if (PointSize > other.PointSize)
                return -1;
            if (PointSize < other.PointSize)
                return 1;
            var v1 = Color.GetValue();
            var v2 = other.Color.GetValue();
            if (v1 > v2)
                return 1;
            if (v1 < v2)
                return -1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is PointPair)
                return this == (PointPair)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode() ^ PointSize.GetHashCode();
        }
    }

    public class PointsModel : MeshModel
    {
        internal PointsModel() { }

        internal override void Draw(Shader shader)
        {
            if (!_hasInit) return;
            BindVertexArray(_vao[0]);
            DrawArrays(GL_POINTS, 0, _pointCount);
        }
    }
}