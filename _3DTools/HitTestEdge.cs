// Decompiled with JetBrains decompiler
// Type: _3DTools.HitTestEdge
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System.Windows;
using System.Windows.Media.Media3D;

namespace _3DTools
{
    public class HitTestEdge
    {
        public Point3D _p1;
        public Point3D _p2;
        public Point _uv1;
        public Point _uv2;
        public Point _p1Transformed;
        public Point _p2Transformed;

        public HitTestEdge(Point3D p1, Point3D p2, Point uv1, Point uv2)
        {
            this._p1 = p1;
            this._p2 = p2;
            this._uv1 = uv1;
            this._uv2 = uv2;
        }

        public void Project(Matrix3D objectToViewportTransform)
        {
            Point3D point3D1 = objectToViewportTransform.Transform(this._p1);
            Point3D point3D2 = objectToViewportTransform.Transform(this._p2);
            this._p1Transformed = new Point(point3D1.X, point3D1.Y);
            this._p2Transformed = new Point(point3D2.X, point3D2.Y);
        }
    }
}