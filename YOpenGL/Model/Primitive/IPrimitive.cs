﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public interface IPrimitive : IDisposable
    {
        RectF Bounds { get; }
        PenF Pen { get; }
        Color? FillColor { get; }
        bool Filled { get; }
        PrimitiveType Type { get; }
        bool HitTest(PointF p, float sensitive);
        IEnumerable<PointF> this[bool isOutline] { get; }
        MeshModel Model { get; set; }
        MeshModel FillModel { get; set; }
    }

    public enum PrimitiveType
    {
        Line,
        Arc,
        Rect,
        Spline,
        Bezier,
        SimpleGeometry,
        ComplexGeometry
    }
}