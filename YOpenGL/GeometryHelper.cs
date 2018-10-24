using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public static class GeometryHelper
    {
        internal static IEnumerable<PointF> GetDashPoints(Line line, PointF _start, PointF _end)
        {
            int curValIndex = 0;
            var flag = true;
            var curP = _start;
            var vec = _end - _start;
            vec.Normalize();
            while (true)
            {
                var end = curP + vec * line.Pen.Data[curValIndex] * 2;
                if (flag)
                {
                    if ((end - _end) * vec > 0)
                    {
                        end = _end;
                        flag = false;
                    }
                    yield return curP;
                    yield return end;
                    if (!flag)
                        break;
                }
                curP = end;
                flag = !flag;
                curValIndex = (curValIndex + 1) % line.Pen.Data.Length;
            }
        }
    }
}