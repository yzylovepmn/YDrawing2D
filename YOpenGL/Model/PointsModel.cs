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
    public struct PointPair
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
            BindVertexArray(_vao[0]);
            DrawArrays(GL_POINTS, 0, _pointCount);
        }
    }
}