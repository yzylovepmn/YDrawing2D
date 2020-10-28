using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class HitTestHelper
    {
        internal static IEnumerable<HitResult> HitTest(GLPanel3D viewport, PointF pointInWpf, float sensitive, bool findTop)
        {
            var results = new List<HitResult>();
            if (!viewport.Camera.TotalTransformReverse.HasValue)
                return results;

            foreach (var model in viewport.Models.Where(m => m.IsVisible && m.IsHitTestVisible && m.Points != null))
            {
                var bounds3D = model.Bounds;
                var bounds2D = Math3DHelper.Transform(bounds3D, viewport);
                var sensity = sensitive;
                switch (model.Mode)
                {
                    case GLPrimitiveMode.GL_POINTS:
                    case GLPrimitiveMode.GL_LINES:
                    case GLPrimitiveMode.GL_LINE_LOOP:
                    case GLPrimitiveMode.GL_LINE_STRIP:
                        sensity += model.Mode == GLPrimitiveMode.GL_POINTS ? model.PointSize / 2 : model.LineWidth / 2;
                        bounds2D.Extents(sensity);
                        break;
                    case GLPrimitiveMode.GL_TRIANGLES:
                    case GLPrimitiveMode.GL_TRIANGLE_STRIP:
                    case GLPrimitiveMode.GL_TRIANGLE_FAN:
                        break;
                }
                if (!bounds2D.Contains(pointInWpf)) continue;

                var points = model.GetDrawPoints().ToArray();
                var pointsTransformed = new LazyArray<Point3F, Point3F>(points, p => viewport.Point3DToPointInWpfWithZDpeth(p));// points.Select(p => p * transform).ToArray();
                switch (model.Mode)
                {
                    case GLPrimitiveMode.GL_POINTS:
                        {
                            for (int i = 0; i < points.Length; i++)
                            {
                                var p = points[i];
                                var pt = pointsTransformed[i];
                                var dist = (pointInWpf - (PointF)pt).Length;
                                if (dist <= sensity)
                                {
                                    results.Add(new HitResult(new PointMesh(p), model, p, pt.Z));
                                    if (findTop) return results;
                                    break;
                                }
                            }
                        }
                        break;
                    case GLPrimitiveMode.GL_LINES:
                    case GLPrimitiveMode.GL_LINE_STRIP:
                    case GLPrimitiveMode.GL_LINE_LOOP:
                        {
                            var cond = points.Length - 1;
                            var stride = model.Mode == GLPrimitiveMode.GL_LINES ? 2 : 1;
                            var limit = model.Mode == GLPrimitiveMode.GL_LINE_LOOP ? points.Length : points.Length - 1;
                            for (int i = 0; i < limit; i += stride)
                            {
                                var p1 = new Point3F();
                                var p2 = new Point3F();
                                var pt1 = new Point3F();
                                var pt2 = new Point3F();
                                if (i != cond)
                                {
                                    p1 = points[i];
                                    p2 = points[i + 1];
                                    pt1 = pointsTransformed[i];
                                    pt2 = pointsTransformed[i + 1];
                                }
                                else
                                {
                                    p1 = points[i];
                                    p2 = points[0];
                                    pt1 = pointsTransformed[i];
                                    pt2 = pointsTransformed[0];
                                }

                                var line = new Line((PointF)pt1, (PointF)pt2);
                                var cp = _LineHitTest(pointInWpf, line, sensity);
                                if (cp.HasValue)
                                {
                                    var p = cp.Value;
                                    var t = line.CalcT(p);
                                    var z = t * pt1.Z + (1 - t) * pt2.Z;
                                    var ht = viewport.PointInWpfToPoint3D(new PointF(p.X, p.Y), z);
                                    if (ht.HasValue)
                                    {
                                        results.Add(new HitResult(new LineMesh(p1, p2), model, ht.Value, z));
                                        if (findTop) return results;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    case GLPrimitiveMode.GL_TRIANGLES:
                    case GLPrimitiveMode.GL_TRIANGLE_STRIP:
                        {
                            var stride = model.Mode == GLPrimitiveMode.GL_TRIANGLES ? 3 : 1;
                            for (int i = 0; i < points.Length - 2; i += stride)
                            {
                                var p1 = points[i];
                                var p2 = points[i + 1];
                                var p3 = points[i + 2];
                                var pt1 = pointsTransformed[i];
                                var pt2 = pointsTransformed[i + 1];
                                var pt3 = pointsTransformed[i + 2];

                                var triangle = new Triangle((PointF)pt1, (PointF)pt2, (PointF)pt3);
                                float a, b;
                                triangle.CalcBarycentric(pointInWpf, out a, out b);
                                if (a >= 0 && b >= 0 && a + b <= 1)
                                {
                                    var c = 1 - a - b;
                                    var z = pt1.Z * a + pt2.Z * b + pt3.Z * c;
                                    var ht = viewport.PointInWpfToPoint3D(new PointF(pointInWpf.X, pointInWpf.Y), z);
                                    if (ht.HasValue)
                                    {
                                        results.Add(new HitResult(new TriangleMesh(p1, p2, p3), model, ht.Value, z));
                                        if (findTop) return results;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    case GLPrimitiveMode.GL_TRIANGLE_FAN:
                        {
                            var p1 = points[0];
                            var pt1 = pointsTransformed[0];
                            for (int i = 2; i < points.Length; i++)
                            {
                                var p2 = points[i - 1];
                                var p3 = points[i];
                                var pt2 = pointsTransformed[i - 1];
                                var pt3 = pointsTransformed[i];

                                var triangle = new Triangle((PointF)pt1, (PointF)pt2, (PointF)pt3);
                                float a, b;
                                triangle.CalcBarycentric(pointInWpf, out a, out b);
                                if (a >= 0 && b >= 0 && a + b <= 1)
                                {
                                    var c = 1 - a - b;
                                    var z = pt1.Z * a + pt2.Z * b + pt3.Z * c;
                                    var ht = viewport.PointInWpfToPoint3D(new PointF(pointInWpf.X, pointInWpf.Y), z);
                                    if (ht.HasValue)
                                    {
                                        results.Add(new HitResult(new TriangleMesh(p1, p2, p3), model, ht.Value, z));
                                        if (findTop) return results;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return results.OrderBy(ret => ret.ZDpeth);
        }

        private static PointF? _LineHitTest(PointF pointInWpf, Line line, float sensitive)
        {
            if (!line.GetBounds().Contains(pointInWpf, sensitive)) return null;

            var cp = line.Projection(pointInWpf);
            if (cp.HasValue)
            {
                var dist = (pointInWpf - cp.Value).Length;
                if (dist <= sensitive)
                    return cp;
            }
            return null;
        }

        internal static IEnumerable<RectHitResult> HitTest(GLPanel3D viewport, RectF rectInWpf, RectHitTestMode hitTestMode)
        {
            var results = new List<RectHitResult>();
            if (!viewport.Camera.TotalTransformReverse.HasValue)
                return results;

            foreach (var model in viewport.Models.Where(m => m.IsVisible && m.IsHitTestVisible && m.Points != null))
            {
                var bounds3D = model.Bounds;
                var bounds2D = Math3DHelper.Transform(bounds3D, viewport);
                var sensity = model.Mode == GLPrimitiveMode.GL_POINTS ? model.PointSize / 2 : model.LineWidth;
                switch (model.Mode)
                {
                    case GLPrimitiveMode.GL_POINTS:
                    case GLPrimitiveMode.GL_LINES:
                    case GLPrimitiveMode.GL_LINE_LOOP:
                    case GLPrimitiveMode.GL_LINE_STRIP:
                        bounds2D.Extents(sensity);
                        break;
                }
                if (rectInWpf.Contains(bounds2D))
                {
                    results.Add(new RectHitResult(model));
                    continue;
                }
                else if (hitTestMode == RectHitTestMode.Intersect)
                {
                    if (!rectInWpf.IntersectsWith(bounds2D)) continue;
                    var pointsTransformed = new LazyArray<Point3F, PointF>(model.GetDrawPoints(), p => viewport.Point3DToPointInWpf(p));//model.GetDrawPoints().Select(p => p * transform).ToArray();
                    switch (model.Mode)
                    {
                        case GLPrimitiveMode.GL_POINTS:
                            {
                                for (int i = 0; i < pointsTransformed.Length; i++)
                                {
                                    var pt = pointsTransformed[i];
                                    if (rectInWpf.Contains(pt) 
                                        || (rectInWpf.BottomLeft - pt).Length <= sensity
                                        || (rectInWpf.BottomRight - pt).Length <= sensity
                                        || (rectInWpf.TopLeft - pt).Length <= sensity
                                        || (rectInWpf.TopRight - pt).Length <= sensity)
                                    {
                                        results.Add(new RectHitResult(model));
                                        break;
                                    }
                                }
                            }
                            break;
                        case GLPrimitiveMode.GL_LINES:
                        case GLPrimitiveMode.GL_LINE_LOOP:
                        case GLPrimitiveMode.GL_LINE_STRIP:
                            {
                                var cond = pointsTransformed.Length - 1;
                                var stride = model.Mode == GLPrimitiveMode.GL_LINES ? 2 : 1;
                                var limit = model.Mode == GLPrimitiveMode.GL_LINE_LOOP ? pointsTransformed.Length : pointsTransformed.Length - 1;
                                for (int i = 0; i < limit; i += stride)
                                {
                                    var pt1 = new PointF();
                                    var pt2 = new PointF();
                                    if (i != cond)
                                    {
                                        pt1 = pointsTransformed[i];
                                        pt2 = pointsTransformed[i + 1];
                                    }
                                    else
                                    {
                                        pt1 = pointsTransformed[i];
                                        pt2 = pointsTransformed[0];
                                    }

                                    var line = new Line(pt1, pt2);
                                    var bounds = line.GetBounds();
                                    bounds.Extents(sensity);
                                    if (bounds.IntersectsWith(rectInWpf) && line.HitTest(rectInWpf, sensity))
                                    {
                                        results.Add(new RectHitResult(model));
                                        break;
                                    }
                                }
                            }
                            break;
                        case GLPrimitiveMode.GL_TRIANGLES:
                        case GLPrimitiveMode.GL_TRIANGLE_STRIP:
                            {
                                var stride = model.Mode == GLPrimitiveMode.GL_TRIANGLES ? 3 : 1;
                                for (int i = 0; i < pointsTransformed.Length - 2; i += stride)
                                {
                                    var pt1 = pointsTransformed[i];
                                    var pt2 = pointsTransformed[i + 1];
                                    var pt3 = pointsTransformed[i + 2];

                                    var triangle = new Triangle(pt1, pt2, pt3);
                                    if (rectInWpf.Contains(triangle.P1) || rectInWpf.Contains(triangle.P2) || rectInWpf.Contains(triangle.P3)
                                        || triangle.IsPointInside(rectInWpf.TopLeft) || triangle.IsPointInside(rectInWpf.TopRight) || triangle.IsPointInside(rectInWpf.BottomLeft) || triangle.IsPointInside(rectInWpf.BottomRight))
                                    {
                                        results.Add(new RectHitResult(model));
                                        break;
                                    }
                                }
                            }
                            break;
                        case GLPrimitiveMode.GL_TRIANGLE_FAN:
                            {
                                var pt1 = pointsTransformed[0];
                                for (int i = 2; i < pointsTransformed.Length; i++)
                                {
                                    var pt2 = pointsTransformed[i - 1];
                                    var pt3 = pointsTransformed[i];

                                    var triangle = new Triangle(pt1, pt2, pt3);
                                    if (rectInWpf.Contains(triangle.P1) || rectInWpf.Contains(triangle.P2) || rectInWpf.Contains(triangle.P3)
                                        || triangle.IsPointInside(rectInWpf.TopLeft) || triangle.IsPointInside(rectInWpf.TopRight) || triangle.IsPointInside(rectInWpf.BottomLeft) || triangle.IsPointInside(rectInWpf.BottomRight))
                                    {
                                        results.Add(new RectHitResult(model));
                                        break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            return results;
        }
    }
}