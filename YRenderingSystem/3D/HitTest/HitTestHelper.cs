using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YRenderingSystem._3D
{
    public class HitTestHelper
    {
        public static IEnumerable<HitResult> HitTest(GLPanel3D viewport, PointF pointInWpf, IEnumerable<IHitTestSource> models, float sensitive = 5)
        {
            var results = new List<HitResult>();
            if (!viewport.Camera.TotalTransformReverse.HasValue)
                return results;

            foreach (var model in models)
                _HitTest(viewport, pointInWpf, sensitive, results, model);
            return results.OrderBy(ret => ret.ZDepth);
        }

        internal static IEnumerable<HitResult> HitTest(GLPanel3D viewport, PointF pointInWpf, float sensitive)
        {
            var results = new List<HitResult>();
            if (!viewport.Camera.TotalTransformReverse.HasValue)
                return results;

            foreach (var visusl in viewport.Visuals.Where(v => v.IsVisible && v.IsHitTestVisible && v.Model != null))
                _HitTest(viewport, pointInWpf, sensitive, results, visusl.Model);
            return results.OrderBy(ret => ret.ZDepth);
        }

        private static bool _HitTest(GLPanel3D viewport, PointF pointInWpf, float sensitive, List<HitResult> results, IHitTestSource model)
        {
            if (model is GLModel3DGroup)
            {
                var group = model as GLModel3DGroup;
                foreach (var child in group.Children)
                {
                    if (_HitTest(viewport, pointInWpf, sensitive, results, child))
                        return true;
                }
                return false;
            }
            else
            {
                //var meshModel = model as GLMeshModel3D;
                var bounds3D = model.Bounds;
                var bounds2D = Math3DHelper.Transform(bounds3D, viewport);
                var sensity = sensitive;
                switch (model.Mode)
                {
                    case GLPrimitiveMode.GL_POINTS:
                    case GLPrimitiveMode.GL_LINES:
                    case GLPrimitiveMode.GL_LINE_LOOP:
                    case GLPrimitiveMode.GL_LINE_STRIP:
                        if (!bounds2D.IsEmpty)
                        {
                            sensity += model.Mode == GLPrimitiveMode.GL_POINTS ? model.PointSize / 2 : model.LineWidth / 2;
                            bounds2D.Extents(sensity);
                        }
                        break;
                    case GLPrimitiveMode.GL_TRIANGLES:
                    case GLPrimitiveMode.GL_TRIANGLE_STRIP:
                    case GLPrimitiveMode.GL_TRIANGLE_FAN:
                        break;
                }
                if (!bounds2D.Contains(pointInWpf)) return false;

                var points = model.GetHitTestPoints().ToArray();
                var pointsTransformed = new LazyArray<Point3F, Point3F>(points, p => viewport.Point3DToPointInWpfWithZDpeth(p));
                switch (model.Mode)
                {
                    case GLPrimitiveMode.GL_POINTS:
                        {
                            if (model.Pairs == null)
                            {
                                if (_HitTestPointResult(model, new DataPair(0, points.Length), points, pointsTransformed, results, pointInWpf, sensity))
                                    return true;
                            }
                            else
                            {
                                foreach (var pair in model.Pairs)
                                    if (_HitTestPointResult(model, pair, points, pointsTransformed, results, pointInWpf, sensity))
                                        return true;
                            }
                        }
                        break;
                    case GLPrimitiveMode.GL_LINES:
                    case GLPrimitiveMode.GL_LINE_STRIP:
                    case GLPrimitiveMode.GL_LINE_LOOP:
                        {
                            if (model.Pairs == null)
                            {
                                if (_HitTestLinesResult(model, new DataPair(0, points.Length), points, pointsTransformed, results, pointInWpf, sensity))
                                    return true;
                            }
                            else
                            {
                                foreach (var pair in model.Pairs)
                                    if (_HitTestLinesResult(model, pair, points, pointsTransformed, results, pointInWpf, sensity))
                                        return true;
                            }
                        }
                        break;
                    case GLPrimitiveMode.GL_TRIANGLES:
                    case GLPrimitiveMode.GL_TRIANGLE_STRIP:
                        {
                            if (model.Pairs == null)
                            {
                                if (_HitTestTrianglesResult(model, new DataPair(0, points.Length), points, pointsTransformed, results, pointInWpf))
                                    return true;
                            }
                            else
                            {
                                foreach (var pair in model.Pairs)
                                    if (_HitTestTrianglesResult(model, pair, points, pointsTransformed, results, pointInWpf))
                                        return true;
                            }
                        }
                        break;
                    case GLPrimitiveMode.GL_TRIANGLE_FAN:
                        {
                            if (model.Pairs == null)
                            {
                                if (_HitTestTriangleFansResult(model, new DataPair(0, points.Length), points, pointsTransformed, results, pointInWpf))
                                    return true;
                            }
                            else
                            {
                                foreach (var pair in model.Pairs)
                                    if (_HitTestTriangleFansResult(model, pair, points, pointsTransformed, results, pointInWpf))
                                        return true;
                            }
                        }
                        break;
                }
                return false;
            }
        }

        private static bool _HitTestPointResult(IHitTestSource model, DataPair pair, Point3F[] points, LazyArray<Point3F, Point3F> pointsTransformed, List<HitResult> results, PointF pointInWpf, float sensity)
        {
            var index = pair.Start;
            for (int i = 0; i < pair.Count; i++, index++)
            {
                var p = points[index];
                var pt = pointsTransformed[index];
                var dist = (pointInWpf - (PointF)pt).Length;
                if (dist <= sensity)
                {
                    results.Add(new HitResult(new PointMesh(p), model, pt.Z));
                    return true;
                }
            }
            return false;
        }

        private static bool _HitTestLinesResult(IHitTestSource model, DataPair pair, Point3F[] points, LazyArray<Point3F, Point3F> pointsTransformed, List<HitResult> results, PointF pointInWpf, float sensity)
        {
            var cond = pair.Count - 1;
            var stride = model.Mode == GLPrimitiveMode.GL_LINES ? 2 : 1;
            var limit = model.Mode == GLPrimitiveMode.GL_LINE_LOOP ? pair.Count : pair.Count - 1;
            for (int i = 0; i < limit; i += stride)
            {
                var index1 = i + pair.Start;
                var index2 = i + 1 + pair.Start;
                if (i == cond)
                    index2 = 0;

                var p1 = points[index1];
                var p2 = points[index2];
                var pt1 = pointsTransformed[index1];
                var pt2 = pointsTransformed[index2];

                var line = new Line((PointF)pt1, (PointF)pt2);
                var cp = _LineHitTest(pointInWpf, line, sensity);
                if (cp.HasValue)
                {
                    var p = cp.Value;
                    var t = line.CalcT(p);
                    GeometryHelper.Clamp(ref t, 0, 1);
                    var z = t * pt1.Z + (1 - t) * pt2.Z;
                    results.Add(new HitResult(new LineMesh(p1, p2, model.GetIndex(index1), model.GetIndex(index2), t, 1 - t), model, z));
                    return true;
                }
            }
            return false;
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

        private static bool _HitTestTrianglesResult(IHitTestSource model, DataPair pair, Point3F[] points, LazyArray<Point3F, Point3F> pointsTransformed, List<HitResult> results, PointF pointInWpf)
        {
            var stride = model.Mode == GLPrimitiveMode.GL_TRIANGLES ? 3 : 1;
            for (int i = 0; i < pair.Count - 2; i += stride)
            {
                var index1 = i + pair.Start;
                var index2 = i + 1 + pair.Start;
                var index3 = i + 2 + pair.Start;
                var p1 = points[index1];
                var p2 = points[index2];
                var p3 = points[index3];
                var pt1 = pointsTransformed[index1];
                var pt2 = pointsTransformed[index2];
                var pt3 = pointsTransformed[index3];

                var triangle = new Triangle((PointF)pt1, (PointF)pt2, (PointF)pt3);
                float a, b;
                triangle.CalcBarycentric(pointInWpf, out a, out b);
                if (a >= 0 && b >= 0 && a + b <= 1)
                {
                    var c = 1 - a - b;
                    var z = pt1.Z * a + pt2.Z * b + pt3.Z * c;
                    results.Add(new HitResult(new TriangleMesh(p1, p2, p3, model.GetIndex(index1), model.GetIndex(index2), model.GetIndex(index3), a, b, c), model, z));
                    return true;
                }
            }
            return false;
        }

        private static bool _HitTestTriangleFansResult(IHitTestSource model, DataPair pair, Point3F[] points, LazyArray<Point3F, Point3F> pointsTransformed, List<HitResult> results, PointF pointInWpf)
        {
            var index1 = pair.Start;
            var p1 = points[index1];
            var pt1 = pointsTransformed[index1];
            for (int i = 2; i < pair.Count; i++)
            {
                var index2 = i - 1 + pair.Start;
                var index3 = i + pair.Start;
                var p2 = points[index2];
                var p3 = points[index3];
                var pt2 = pointsTransformed[index2];
                var pt3 = pointsTransformed[index3];

                var triangle = new Triangle((PointF)pt1, (PointF)pt2, (PointF)pt3);
                float a, b;
                triangle.CalcBarycentric(pointInWpf, out a, out b);
                if (a >= 0 && b >= 0 && a + b <= 1)
                {
                    var c = 1 - a - b;
                    var z = pt1.Z * a + pt2.Z * b + pt3.Z * c;
                    results.Add(new HitResult(new TriangleMesh(p1, p2, p3, model.GetIndex(index1), model.GetIndex(index2), model.GetIndex(index3), a, b, c), model, z));
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<RectHitResult> HitTest(GLPanel3D viewport, IEnumerable<IHitTestSource> models, RectF rectInWpf, RectHitTestMode hitTestMode)
        {
            var results = new List<RectHitResult>();
            if (!viewport.Camera.TotalTransformReverse.HasValue)
                return results;

            var isFullContain = hitTestMode == RectHitTestMode.FullContain;
            foreach (var model in models)
                _HitTest(viewport, rectInWpf, model, results, isFullContain);

            return results;
        }

        internal static IEnumerable<RectHitResult> HitTest(GLPanel3D viewport, RectF rectInWpf, RectHitTestMode hitTestMode)
        {
            var results = new List<RectHitResult>();
            if (!viewport.Camera.TotalTransformReverse.HasValue)
                return results;

            var isFullContain = hitTestMode == RectHitTestMode.FullContain;
            foreach (var visusl in viewport.Visuals.Where(v => v.IsVisible && v.IsHitTestVisible && v.Model != null))
                _HitTest(viewport, rectInWpf, visusl.Model, results, isFullContain);

            return results;
        }

        private static bool _HitTest(GLPanel3D viewport, RectF rectInWpf, IHitTestSource model, List<RectHitResult> results, bool isFullContain)
        {
            if (model is GLModel3DGroup)
            {
                var group = model as GLModel3DGroup;
                if (isFullContain)
                {
                    foreach (var child in group.Children)
                    {
                        if (!_HitTest(viewport, rectInWpf, child, results, isFullContain))
                            return false;
                    }
                    if (model.Parent == null)
                        results.Add(new RectHitResult(model));
                    return true;
                }
                else
                {
                    foreach (var child in group.Children)
                    {
                        if (_HitTest(viewport, rectInWpf, child, results, isFullContain))
                            return true;
                    }
                    return false;
                }
            }
            else
            {
                //var meshModel = model as GLMeshModel3D;
                var bounds3D = model.Bounds;
                var bounds2D = Math3DHelper.Transform(bounds3D, viewport);
                var sensity = model.Mode == GLPrimitiveMode.GL_POINTS ? model.PointSize / 2 : model.LineWidth;
                switch (model.Mode)
                {
                    case GLPrimitiveMode.GL_POINTS:
                    case GLPrimitiveMode.GL_LINES:
                    case GLPrimitiveMode.GL_LINE_LOOP:
                    case GLPrimitiveMode.GL_LINE_STRIP:
                        if (!bounds2D.IsEmpty)
                            bounds2D.Extents(sensity);
                        break;
                }
                if ((isFullContain && bounds2D.IsEmpty) || rectInWpf.Contains(bounds2D))
                {
                    if (!isFullContain || model.Parent == null)
                        results.Add(new RectHitResult(model));
                    return true;
                }
                else
                {
                    if (!rectInWpf.IntersectsWith(bounds2D)) return false;
                    var flag1 = false;
                    var flag2 = true;
                    var pointsTransformed = new LazyArray<Point3F, PointF>(model.GetHitTestPoints(), p => viewport.Point3DToPointInWpf(p));
                    switch (model.Mode)
                    {
                        case GLPrimitiveMode.GL_POINTS:
                            {
                                if (model.Pairs == null)
                                    _HitTestPointResult(new DataPair(0, pointsTransformed.Length), pointsTransformed, rectInWpf, isFullContain, sensity, ref flag1, ref flag2);
                                else
                                {
                                    foreach (var pair in model.Pairs)
                                        if (_HitTestPointResult(pair, pointsTransformed, rectInWpf, isFullContain, sensity, ref flag1, ref flag2))
                                            break;
                                }
                            }
                            break;
                        case GLPrimitiveMode.GL_LINES:
                        case GLPrimitiveMode.GL_LINE_LOOP:
                        case GLPrimitiveMode.GL_LINE_STRIP:
                            {
                                if (model.Pairs == null)
                                    _HitTestLinesResult(model, new DataPair(0, pointsTransformed.Length), pointsTransformed, rectInWpf, isFullContain, sensity, ref flag1, ref flag2);
                                else
                                {
                                    foreach (var pair in model.Pairs)
                                        if (_HitTestLinesResult(model, pair, pointsTransformed, rectInWpf, isFullContain, sensity, ref flag1, ref flag2))
                                            break;
                                }
                            }
                            break;
                        case GLPrimitiveMode.GL_TRIANGLES:
                        case GLPrimitiveMode.GL_TRIANGLE_STRIP:
                            {
                                if (model.Pairs == null)
                                    _HitTestTrianglesResult(model, new DataPair(0, pointsTransformed.Length), pointsTransformed, rectInWpf, isFullContain, ref flag1, ref flag2);
                                else
                                {
                                    foreach (var pair in model.Pairs)
                                        if (_HitTestTrianglesResult(model, pair, pointsTransformed, rectInWpf, isFullContain, ref flag1, ref flag2))
                                            break;
                                }
                            }
                            break;
                        case GLPrimitiveMode.GL_TRIANGLE_FAN:
                            {
                                if (model.Pairs == null)
                                    _HitTestTriangleFansResult(new DataPair(0, pointsTransformed.Length), pointsTransformed, rectInWpf, isFullContain, ref flag1, ref flag2);
                                else
                                {
                                    foreach (var pair in model.Pairs)
                                        if (_HitTestTriangleFansResult(pair, pointsTransformed, rectInWpf, isFullContain, ref flag1, ref flag2))
                                            break;
                                }
                            }
                            break;
                    }
                    if (flag1 && !isFullContain)
                    {
                        results.Add(new RectHitResult(model));
                        return true;
                    }
                    else if (flag2 && isFullContain)
                    {
                        if (model.Parent == null)
                            results.Add(new RectHitResult(model));
                        return true;
                    }
                    return false;
                }
            }
        }

        private static bool _HitTestPointResult(DataPair pair, LazyArray<Point3F, PointF> pointsTransformed, RectF rectInWpf, bool isFullContain, float sensity, ref bool flag1, ref bool flag2)
        {
            var stopCond = false;
            for (int i = 0; i < pair.Count; i++)
            {
                var pt = pointsTransformed[i + pair.Start];
                if (!isFullContain)
                {
                    if (rectInWpf.Contains(pt, sensity))
                    {
                        flag1 = true;
                        stopCond = true;
                        break;
                    }
                }
                else
                {
                    var rect = new RectF(pt, pt);
                    rect.Extents(sensity);
                    if (!rectInWpf.Contains(rect))
                    {
                        flag2 = false;
                        stopCond = true;
                        break;
                    }
                }
            }
            return stopCond;
        }

        private static bool _HitTestLinesResult(IHitTestSource model, DataPair pair, LazyArray<Point3F, PointF> pointsTransformed, RectF rectInWpf, bool isFullContain, float sensity, ref bool flag1, ref bool flag2)
        {
            var stopCond = false;
            var cond = pair.Count - 1;
            var stride = model.Mode == GLPrimitiveMode.GL_LINES ? 2 : 1;
            var limit = model.Mode == GLPrimitiveMode.GL_LINE_LOOP ? pair.Count : pair.Count - 1;
            for (int i = 0; i < limit; i += stride)
            {
                var index1 = i + pair.Start;
                var index2 = i + 1 + pair.Start;
                if (i == cond)
                    index2 = 0;
                var pt1 = pointsTransformed[index1];
                var pt2 = pointsTransformed[index2];

                var line = new Line(pt1, pt2);
                var bounds = line.GetBounds();
                bounds.Extents(sensity);
                if (!isFullContain)
                {
                    if (bounds.IntersectsWith(rectInWpf) && line.HitTest(rectInWpf, sensity))
                    {
                        flag1 = true;
                        stopCond = true;
                        break;
                    }
                }
                else
                {
                    if (!rectInWpf.Contains(bounds))
                    {
                        flag2 = false;
                        stopCond = true;
                        break;
                    }
                }
            }
            return stopCond;
        }

        private static bool _HitTestTrianglesResult(IHitTestSource model, DataPair pair, LazyArray<Point3F, PointF> pointsTransformed, RectF rectInWpf, bool isFullContain, ref bool flag1, ref bool flag2)
        {
            var stopCond = false;
            var stride = model.Mode == GLPrimitiveMode.GL_TRIANGLES ? 3 : 1;
            for (int i = 0; i < pair.Count - 2; i += stride)
            {
                var pt1 = pointsTransformed[i + pair.Start];
                var pt2 = pointsTransformed[i + 1 + pair.Start];
                var pt3 = pointsTransformed[i + 2 + pair.Start];

                var triangle = new Triangle(pt1, pt2, pt3);
                if (!isFullContain)
                {
                    if (rectInWpf.Contains(triangle.P1) || rectInWpf.Contains(triangle.P2) || rectInWpf.Contains(triangle.P3)
                    || triangle.IsPointInside(rectInWpf.TopLeft) || triangle.IsPointInside(rectInWpf.TopRight) || triangle.IsPointInside(rectInWpf.BottomLeft) || triangle.IsPointInside(rectInWpf.BottomRight))
                    {
                        flag1 = true;
                        stopCond = true;
                        break;
                    }
                }
                else
                {
                    if (!triangle.IsCompletelyInside(rectInWpf))
                    {
                        flag2 = false;
                        stopCond = true;
                        break;
                    }
                }
            }
            return stopCond;
        }

        private static bool _HitTestTriangleFansResult(DataPair pair, LazyArray<Point3F, PointF> pointsTransformed, RectF rectInWpf, bool isFullContain, ref bool flag1, ref bool flag2)
        {
            var stopCond = false;
            var pt1 = pointsTransformed[pair.Start];
            for (int i = 2; i < pair.Count; i++)
            {
                var pt2 = pointsTransformed[i - 1 + pair.Start];
                var pt3 = pointsTransformed[i + pair.Start];

                var triangle = new Triangle(pt1, pt2, pt3);
                if (!isFullContain)
                {
                    if (rectInWpf.Contains(triangle.P1) || rectInWpf.Contains(triangle.P2) || rectInWpf.Contains(triangle.P3)
                    || triangle.IsPointInside(rectInWpf.TopLeft) || triangle.IsPointInside(rectInWpf.TopRight) || triangle.IsPointInside(rectInWpf.BottomLeft) || triangle.IsPointInside(rectInWpf.BottomRight))
                    {
                        flag1 = true;
                        stopCond = true;
                        break;
                    }
                }
                else
                {
                    if (!triangle.IsCompletelyInside(rectInWpf))
                    {
                        flag2 = false;
                        stopCond = true;
                        break;
                    }
                }
            }
            return stopCond;
        }
    }
}