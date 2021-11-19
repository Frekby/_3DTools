// Decompiled with JetBrains decompiler
// Type: _3DTools.InteractiveVisual3D
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DTools
{
  public class InteractiveVisual3D : ModelVisual3D
  {
    private static DependencyProperty VisualProperty = DependencyProperty.Register(nameof (Visual), typeof (Visual), typeof (InteractiveVisual3D), new PropertyMetadata((object) null, new PropertyChangedCallback(InteractiveVisual3D.OnVisualChanged)));
    private static readonly DependencyProperty IsBackVisibleProperty = DependencyProperty.Register(nameof (IsBackVisible), typeof (bool), typeof (InteractiveVisual3D), new PropertyMetadata((object) false, new PropertyChangedCallback(InteractiveVisual3D.OnIsBackVisiblePropertyChanged)));
    private static readonly DiffuseMaterial _defaultMaterialPropertyValue;
    public static readonly DependencyProperty MaterialProperty;
    public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register(nameof (Geometry), typeof (Geometry3D), typeof (InteractiveVisual3D), new PropertyMetadata((object) null, new PropertyChangedCallback(InteractiveVisual3D.OnGeometryChanged)));
    public static readonly DependencyProperty IsInteractiveMaterialProperty = DependencyProperty.RegisterAttached("IsInteractiveMaterial", typeof (bool), typeof (InteractiveVisual3D), new PropertyMetadata((object) false));
    internal readonly GeometryModel3D _content;
    private Point[] _lastVisCorners;
    private List<HitTestEdge> _lastEdges;
    private Matrix3D _lastMatrix3D;
    private UIElement _internalVisual;
    private VisualBrush _visualBrush;

    public InteractiveVisual3D()
    {
      this.InternalVisualBrush = this.CreateVisualBrush();
      this._content = new GeometryModel3D();
      this.Content = (Model3D) this._content;
      this.GenerateMaterial();
    }

    static InteractiveVisual3D()
    {
      InteractiveVisual3D._defaultMaterialPropertyValue = new DiffuseMaterial();
      InteractiveVisual3D._defaultMaterialPropertyValue.SetValue(InteractiveVisual3D.IsInteractiveMaterialProperty, (object) true);
      InteractiveVisual3D._defaultMaterialPropertyValue.Freeze();
      InteractiveVisual3D.MaterialProperty = DependencyProperty.Register(nameof (Material), typeof (Material), typeof (InteractiveVisual3D), new PropertyMetadata((object) InteractiveVisual3D._defaultMaterialPropertyValue, new PropertyChangedCallback(InteractiveVisual3D.OnMaterialPropertyChanged)));
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      this._lastVisCorners = (Point[]) null;
    }

    internal List<HitTestEdge> GetVisualEdges(Point[] texCoordsOfInterest)
    {
      this._lastEdges = this.GrabValidEdges(texCoordsOfInterest);
      this._lastVisCorners = texCoordsOfInterest;
      return this._lastEdges;
    }

    private List<HitTestEdge> GrabValidEdges(Point[] tc)
    {
      List<HitTestEdge> edgeList = new List<HitTestEdge>();
      Dictionary<InteractiveVisual3D.Edge, InteractiveVisual3D.EdgeInfo> adjInformation = new Dictionary<InteractiveVisual3D.Edge, InteractiveVisual3D.EdgeInfo>();
      MeshGeometry3D geometry = (MeshGeometry3D) this._content.Geometry;
      Point3DCollection positions = geometry.Positions;
      PointCollection textureCoordinates = geometry.TextureCoordinates;
      Int32Collection triangleIndices = geometry.TriangleIndices;
      Transform3D transform = this._content.Transform;
      Viewport3DVisual viewport;
      bool success;
      Matrix3D cameraSpace = MathUtils.TryTransformToCameraSpace((DependencyObject) this, out viewport, out success);
      if (!success)
        return new List<HitTestEdge>();
      if (transform != null)
        cameraSpace.Prepend(transform.Value);
      Matrix3D objectToViewportTransform = MathUtils.TryTransformTo2DAncestor((DependencyObject) this, out viewport, out success);
      if (!success)
        return new List<HitTestEdge>();
      if (transform != null)
        objectToViewportTransform.Prepend(transform.Value);
      bool flag = this._lastVisCorners != null;
      if (this._lastVisCorners != null)
      {
        for (int index = 0; index < tc.Length; ++index)
        {
          if (tc[index] != this._lastVisCorners[index])
          {
            flag = false;
            break;
          }
        }
        if (this._lastMatrix3D != objectToViewportTransform)
          flag = false;
      }
      if (flag)
        return this._lastEdges;
      this._lastMatrix3D = objectToViewportTransform;
      try
      {
        cameraSpace.Invert();
      }
      catch (InvalidOperationException ex)
      {
        return new List<HitTestEdge>();
      }
      Point3D camPosObjSpace = cameraSpace.Transform(new Point3D(0.0, 0.0, 0.0));
      Rect empty1 = Rect.Empty;
      for (int index = 0; index < tc.Length; ++index)
        empty1.Union(tc[index]);
      int[] numArray = new int[3];
      Point3D[] p = new Point3D[3];
      Point[] uv = new Point[3];
      for (int index1 = 0; index1 < triangleIndices.Count; index1 += 3)
      {
        Rect empty2 = Rect.Empty;
        for (int index2 = 0; index2 < 3; ++index2)
        {
          numArray[index2] = triangleIndices[index1 + index2];
          p[index2] = positions[numArray[index2]];
          uv[index2] = textureCoordinates[numArray[index2]];
          empty2.Union(uv[index2]);
        }
        if (empty1.IntersectsWith(empty2))
          this.ProcessTriangle(p, uv, tc, edgeList, adjInformation, camPosObjSpace);
      }
      foreach (InteractiveVisual3D.Edge key in adjInformation.Keys)
      {
        InteractiveVisual3D.EdgeInfo edgeInfo = adjInformation[key];
        if (edgeInfo.hasFrontFace && edgeInfo.numSharing == 1)
          this.HandleSilhouetteEdge(edgeInfo.uv1, edgeInfo.uv2, key._start, key._end, tc, edgeList);
      }
      for (int index = 0; index < edgeList.Count; ++index)
        edgeList[index].Project(objectToViewportTransform);
      return edgeList;
    }

    private void ProcessTriangle(
      Point3D[] p,
      Point[] uv,
      Point[] tc,
      List<HitTestEdge> edgeList,
      Dictionary<InteractiveVisual3D.Edge, InteractiveVisual3D.EdgeInfo> adjInformation,
      Point3D camPosObjSpace)
    {
      Vector3D vector1 = Vector3D.CrossProduct(p[1] - p[0], p[2] - p[0]);
      Vector3D vector2 = camPosObjSpace - p[0];
      if (vector1.X == 0.0 && vector1.Y == 0.0 && vector1.Z == 0.0)
        return;
      if (Vector3D.DotProduct(vector1, vector2) > 0.0)
      {
        this.ProcessTriangleEdges(p, uv, tc, InteractiveVisual3D.PolygonSide.FRONT, edgeList, adjInformation);
        this.ProcessVisualBoundsIntersections(p, uv, tc, edgeList);
      }
      else
        this.ProcessTriangleEdges(p, uv, tc, InteractiveVisual3D.PolygonSide.BACK, edgeList, adjInformation);
    }

    private void ProcessVisualBoundsIntersections(
      Point3D[] p,
      Point[] uv,
      Point[] tc,
      List<HitTestEdge> edgeList)
    {
      List<Point3D> point3DList = new List<Point3D>();
      List<Point> pointList = new List<Point>();
      for (int index1 = 0; index1 < tc.Length; ++index1)
      {
        Point point1 = tc[index1];
        Point point2 = tc[(index1 + 1) % tc.Length];
        point3DList.Clear();
        pointList.Clear();
        bool flag = false;
        for (int index2 = 0; index2 < uv.Length; ++index2)
        {
          Point point3 = uv[index2];
          Point triUV2 = uv[(index2 + 1) % uv.Length];
          Point3D tri3D1 = p[index2];
          Point3D tri3D2 = p[(index2 + 1) % p.Length];
          if (Math.Max(point1.X, point2.X) >= Math.Min(point3.X, triUV2.X) && Math.Min(point1.X, point2.X) <= Math.Max(point3.X, triUV2.X) && Math.Max(point1.Y, point2.Y) >= Math.Min(point3.Y, triUV2.Y) && Math.Min(point1.Y, point2.Y) <= Math.Max(point3.Y, triUV2.Y))
          {
            bool coinc = false;
            Vector d = triUV2 - point3;
            double num = this.IntersectRayLine(point3, d, point1, point2, out coinc);
            if (coinc)
            {
              this.HandleCoincidentLines(point1, point2, tri3D1, tri3D2, point3, triUV2, edgeList);
              flag = true;
              break;
            }
            if (num >= 0.0 && num <= 1.0)
            {
              Point point4 = point3 + d * num;
              Point3D point3D = tri3D1 + (tri3D2 - tri3D1) * num;
              double length = (point1 - point2).Length;
              if ((point4 - point1).Length < length && (point4 - point2).Length < length)
              {
                point3DList.Add(point3D);
                pointList.Add(point4);
              }
            }
          }
        }
        if (!flag)
        {
          if (point3DList.Count >= 2)
            edgeList.Add(new HitTestEdge(point3DList[0], point3DList[1], pointList[0], pointList[1]));
          else if (point3DList.Count == 1)
          {
            Point3D inters3DPoint;
            if (this.IsPointInTriangle(point1, uv, p, out inters3DPoint))
              edgeList.Add(new HitTestEdge(point3DList[0], inters3DPoint, pointList[0], point1));
            if (this.IsPointInTriangle(point2, uv, p, out inters3DPoint))
              edgeList.Add(new HitTestEdge(point3DList[0], inters3DPoint, pointList[0], point2));
          }
          else
          {
            Point3D inters3DPoint1;
            Point3D inters3DPoint2;
            if (this.IsPointInTriangle(point1, uv, p, out inters3DPoint1) && this.IsPointInTriangle(point2, uv, p, out inters3DPoint2))
              edgeList.Add(new HitTestEdge(inters3DPoint1, inters3DPoint2, point1, point2));
          }
        }
      }
    }

    private bool IsPointInTriangle(
      Point p,
      Point[] triUVVertices,
      Point3D[] tri3DVertices,
      out Point3D inters3DPoint)
    {
      inters3DPoint = new Point3D();
      double num1 = triUVVertices[0].X - triUVVertices[2].X;
      double num2 = triUVVertices[1].X - triUVVertices[2].X;
      double num3 = triUVVertices[2].X - p.X;
      double num4 = triUVVertices[0].Y - triUVVertices[2].Y;
      double num5 = triUVVertices[1].Y - triUVVertices[2].Y;
      double num6 = triUVVertices[2].Y - p.Y;
      double num7 = num1 * num5 - num2 * num4;
      if (num7 == 0.0)
        return false;
      double num8 = (num2 * num6 - num3 * num5) / num7;
      double num9 = num2 * num4 - num1 * num5;
      if (num9 == 0.0)
        return false;
      double num10 = (num1 * num6 - num3 * num4) / num9;
      if (num8 < 0.0 || num8 > 1.0 || num10 < 0.0 || num10 > 1.0 || num8 + num10 > 1.0)
        return false;
      inters3DPoint = (Point3D) (num8 * (Vector3D) tri3DVertices[0] + num10 * (Vector3D) tri3DVertices[1] + (1.0 - num8 - num10) * (Vector3D) tri3DVertices[2]);
      return true;
    }

    private void HandleCoincidentLines(
      Point visUV1,
      Point visUV2,
      Point3D tri3D1,
      Point3D tri3D2,
      Point triUV1,
      Point triUV2,
      List<HitTestEdge> edgeList)
    {
      Point uv1;
      Point3D p1;
      Point uv2;
      Point3D p2;
      if (Math.Abs(visUV1.X - visUV2.X) > Math.Abs(visUV1.Y - visUV2.Y))
      {
        Point point1;
        Point point2;
        if (visUV1.X <= visUV2.X)
        {
          point1 = visUV1;
          point2 = visUV2;
        }
        else
        {
          point1 = visUV2;
          point2 = visUV1;
        }
        Point point3;
        Point3D point3D1;
        Point point4;
        Point3D point3D2;
        if (triUV1.X <= triUV2.X)
        {
          point3 = triUV1;
          point3D1 = tri3D1;
          point4 = triUV2;
          point3D2 = tri3D2;
        }
        else
        {
          point3 = triUV2;
          point3D1 = tri3D2;
          point4 = triUV1;
          point3D2 = tri3D1;
        }
        if (point1.X < point3.X)
        {
          uv1 = point3;
          p1 = point3D1;
        }
        else
        {
          uv1 = point1;
          p1 = point3D1 + (point1.X - point3.X) / (point4.X - point3.X) * (point3D2 - point3D1);
        }
        if (point2.X > point4.X)
        {
          uv2 = point4;
          p2 = point3D2;
        }
        else
        {
          uv2 = point2;
          p2 = point3D1 + (point2.X - point3.X) / (point4.X - point3.X) * (point3D2 - point3D1);
        }
      }
      else
      {
        Point point5;
        Point point6;
        if (visUV1.Y <= visUV2.Y)
        {
          point5 = visUV1;
          point6 = visUV2;
        }
        else
        {
          point5 = visUV2;
          point6 = visUV1;
        }
        Point point7;
        Point3D point3D3;
        Point point8;
        Point3D point3D4;
        if (triUV1.Y <= triUV2.Y)
        {
          point7 = triUV1;
          point3D3 = tri3D1;
          point8 = triUV2;
          point3D4 = tri3D2;
        }
        else
        {
          point7 = triUV2;
          point3D3 = tri3D2;
          point8 = triUV1;
          point3D4 = tri3D1;
        }
        if (point5.Y < point7.Y)
        {
          uv1 = point7;
          p1 = point3D3;
        }
        else
        {
          uv1 = point5;
          p1 = point3D3 + (point5.Y - point7.Y) / (point8.Y - point7.Y) * (point3D4 - point3D3);
        }
        if (point6.Y > point8.Y)
        {
          uv2 = point8;
          p2 = point3D4;
        }
        else
        {
          uv2 = point6;
          p2 = point3D3 + (point6.Y - point7.Y) / (point8.Y - point7.Y) * (point3D4 - point3D3);
        }
      }
      edgeList.Add(new HitTestEdge(p1, p2, uv1, uv2));
    }

    private double IntersectRayLine(Point o, Vector d, Point p1, Point p2, out bool coinc)
    {
      coinc = false;
      double num1 = p2.Y - p1.Y;
      double num2 = p2.X - p1.X;
      if (num2 == 0.0)
      {
        if (d.X != 0.0)
          return (p2.X - o.X) / d.X;
        coinc = o.X == p1.X;
        return -1.0;
      }
      double num3 = (o.X - p1.X) * num1 / num2 - o.Y + p1.Y;
      double num4 = d.Y - d.X * num1 / num2;
      if (num4 != 0.0)
        return num3 / num4;
      double num5 = -o.X * num1 / num2 + o.Y;
      double num6 = -p1.X * num1 / num2 + p1.Y;
      coinc = num5 == num6;
      return -1.0;
    }

    private void ProcessTriangleEdges(
      Point3D[] p,
      Point[] uv,
      Point[] tc,
      InteractiveVisual3D.PolygonSide polygonSide,
      List<HitTestEdge> edgeList,
      Dictionary<InteractiveVisual3D.Edge, InteractiveVisual3D.EdgeInfo> adjInformation)
    {
      for (int index = 0; index < p.Length; ++index)
      {
        Point3D point3D1 = p[index];
        Point3D point3D2 = p[(index + 1) % p.Length];
        InteractiveVisual3D.Edge key;
        Point point1;
        Point point2;
        if (point3D1.X < point3D2.X || point3D1.X == point3D2.X && point3D1.Y < point3D2.Y || point3D1.X == point3D2.X && point3D1.Y == point3D2.Y && point3D1.Z < point3D1.Z)
        {
          key = new InteractiveVisual3D.Edge(point3D1, point3D2);
          point1 = uv[index];
          point2 = uv[(index + 1) % p.Length];
        }
        else
        {
          key = new InteractiveVisual3D.Edge(point3D2, point3D1);
          point2 = uv[index];
          point1 = uv[(index + 1) % p.Length];
        }
        InteractiveVisual3D.EdgeInfo edgeInfo;
        if (adjInformation.ContainsKey(key))
        {
          edgeInfo = adjInformation[key];
        }
        else
        {
          edgeInfo = new InteractiveVisual3D.EdgeInfo();
          adjInformation[key] = edgeInfo;
        }
        ++edgeInfo.numSharing;
        bool flag = edgeInfo.hasBackFace && edgeInfo.hasFrontFace;
        if (polygonSide == InteractiveVisual3D.PolygonSide.FRONT)
        {
          edgeInfo.hasFrontFace = true;
          edgeInfo.uv1 = point1;
          edgeInfo.uv2 = point2;
        }
        else
          edgeInfo.hasBackFace = true;
        if (!flag && edgeInfo.hasBackFace && edgeInfo.hasFrontFace)
          this.HandleSilhouetteEdge(edgeInfo.uv1, edgeInfo.uv2, key._start, key._end, tc, edgeList);
      }
    }

    private void HandleSilhouetteEdge(
      Point uv1,
      Point uv2,
      Point3D p3D1,
      Point3D p3D2,
      Point[] bounds,
      List<HitTestEdge> edgeList)
    {
      List<Point3D> point3DList = new List<Point3D>();
      List<Point> pointList = new List<Point>();
      Vector d = uv2 - uv1;
      for (int index = 0; index < bounds.Length; ++index)
      {
        Point bound1 = bounds[index];
        Point bound2 = bounds[(index + 1) % bounds.Length];
        if (Math.Max(bound1.X, bound2.X) >= Math.Min(uv1.X, uv2.X) && Math.Min(bound1.X, bound2.X) <= Math.Max(uv1.X, uv2.X) && Math.Max(bound1.Y, bound2.Y) >= Math.Min(uv1.Y, uv2.Y) && Math.Min(bound1.Y, bound2.Y) <= Math.Max(uv1.Y, uv2.Y))
        {
          bool coinc = false;
          double num = this.IntersectRayLine(uv1, d, bound1, bound2, out coinc);
          if (coinc)
            return;
          if (num >= 0.0 && num <= 1.0)
          {
            Point point = uv1 + d * num;
            Point3D point3D = p3D1 + (p3D2 - p3D1) * num;
            double length = (bound1 - bound2).Length;
            if ((point - bound1).Length < length && (point - bound2).Length < length)
            {
              point3DList.Add(point3D);
              pointList.Add(point);
            }
          }
        }
      }
      if (point3DList.Count >= 2)
        edgeList.Add(new HitTestEdge(point3DList[0], point3DList[1], pointList[0], pointList[1]));
      else if (point3DList.Count == 1)
      {
        if (this.IsPointInPolygon(bounds, uv1))
          edgeList.Add(new HitTestEdge(point3DList[0], p3D1, pointList[0], uv1));
        if (!this.IsPointInPolygon(bounds, uv2))
          return;
        edgeList.Add(new HitTestEdge(point3DList[0], p3D2, pointList[0], uv2));
      }
      else
      {
        if (!this.IsPointInPolygon(bounds, uv1) || !this.IsPointInPolygon(bounds, uv2))
          return;
        edgeList.Add(new HitTestEdge(p3D1, p3D2, uv1, uv2));
      }
    }

    private bool IsPointInPolygon(Point[] polygon, Point p)
    {
      bool flag1 = false;
      for (int index = 0; index < polygon.Length; ++index)
      {
        bool flag2 = Vector.CrossProduct(polygon[(index + 1) % polygon.Length] - polygon[index], polygon[index] - p) > 0.0;
        if (index == 0)
          flag1 = flag2;
        else if (flag1 != flag2)
          return false;
      }
      return true;
    }

    private void GenerateMaterial()
    {
      this.InternalVisualBrush.Visual = (Visual) null;
      this.InternalVisualBrush = this.CreateVisualBrush();
      Material material = this.Material.Clone();
      this._content.Material = material;
      this.InternalVisualBrush.Visual = (Visual) this.InternalVisual;
      this.SwapInVisualBrush(material);
      if (!this.IsBackVisible)
        return;
      this._content.BackMaterial = material;
    }

    private VisualBrush CreateVisualBrush()
    {
      VisualBrush visualBrush = new VisualBrush();
      RenderOptions.SetCachingHint((DependencyObject) visualBrush, CachingHint.Cache);
      visualBrush.ViewportUnits = BrushMappingMode.Absolute;
      visualBrush.TileMode = TileMode.None;
      return visualBrush;
    }

    private void SwapInVisualBrush(Material material)
    {
      bool flag = false;
      Stack<Material> materialStack = new Stack<Material>();
      materialStack.Push(material);
      while (materialStack.Count > 0)
      {
        Material material1 = materialStack.Pop();
        switch (material1)
        {
          case DiffuseMaterial _:
            DiffuseMaterial diffuseMaterial = (DiffuseMaterial) material1;
            if ((bool) diffuseMaterial.GetValue(InteractiveVisual3D.IsInteractiveMaterialProperty))
            {
              diffuseMaterial.Brush = (Brush) this.InternalVisualBrush;
              flag = true;
              continue;
            }
            continue;
          case EmissiveMaterial _:
            EmissiveMaterial emissiveMaterial = (EmissiveMaterial) material1;
            if ((bool) emissiveMaterial.GetValue(InteractiveVisual3D.IsInteractiveMaterialProperty))
            {
              emissiveMaterial.Brush = (Brush) this.InternalVisualBrush;
              flag = true;
              continue;
            }
            continue;
          case SpecularMaterial _:
            SpecularMaterial specularMaterial = (SpecularMaterial) material1;
            if ((bool) specularMaterial.GetValue(InteractiveVisual3D.IsInteractiveMaterialProperty))
            {
              specularMaterial.Brush = (Brush) this.InternalVisualBrush;
              flag = true;
              continue;
            }
            continue;
          case MaterialGroup _:
            using (MaterialCollection.Enumerator enumerator = ((MaterialGroup) material1).Children.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                Material current = enumerator.Current;
                materialStack.Push(current);
              }
              continue;
            }
          default:
            throw new ArgumentException("material needs to be either a DiffuseMaterial, EmissiveMaterial, SpecularMaterial or a MaterialGroup", nameof (material));
        }
      }
      if (!flag)
        throw new ArgumentException("material needs to contain at least one material that has the IsInteractiveMaterial attached property", nameof (material));
    }

    public Visual Visual
    {
      get => (Visual) this.GetValue(InteractiveVisual3D.VisualProperty);
      set => this.SetValue(InteractiveVisual3D.VisualProperty, (object) value);
    }

    internal UIElement InternalVisual => this._internalVisual;

    private VisualBrush InternalVisualBrush
    {
      get => this._visualBrush;
      set => this._visualBrush = value;
    }

    internal static void OnVisualChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      InteractiveVisual3D interactiveVisual3D = (InteractiveVisual3D) sender;
      AdornerDecorator adornerDecorator = (AdornerDecorator) null;
      if (interactiveVisual3D.InternalVisual != null)
      {
        adornerDecorator = (AdornerDecorator) interactiveVisual3D.InternalVisual;
        if (adornerDecorator.Child is VisualDecorator)
          ((VisualDecorator) adornerDecorator.Child).Content = (Visual) null;
      }
      if (adornerDecorator == null)
        adornerDecorator = new AdornerDecorator();
      UIElement uiElement;
      if (interactiveVisual3D.Visual is UIElement)
        uiElement = (UIElement) interactiveVisual3D.Visual;
      else
        uiElement = (UIElement) new VisualDecorator()
        {
          Content = interactiveVisual3D.Visual
        };
      adornerDecorator.Child = (UIElement) null;
      adornerDecorator.Child = uiElement;
      interactiveVisual3D._internalVisual = (UIElement) adornerDecorator;
      interactiveVisual3D.InternalVisualBrush.Visual = (Visual) interactiveVisual3D.InternalVisual;
    }

    public bool IsBackVisible
    {
      get => (bool) this.GetValue(InteractiveVisual3D.IsBackVisibleProperty);
      set => this.SetValue(InteractiveVisual3D.IsBackVisibleProperty, (object) value);
    }

    internal static void OnIsBackVisiblePropertyChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      InteractiveVisual3D interactiveVisual3D = (InteractiveVisual3D) sender;
      if (interactiveVisual3D.IsBackVisible)
        interactiveVisual3D._content.BackMaterial = interactiveVisual3D._content.Material;
      else
        interactiveVisual3D._content.BackMaterial = (Material) null;
    }

    public Material Material
    {
      get => (Material) this.GetValue(InteractiveVisual3D.MaterialProperty);
      set => this.SetValue(InteractiveVisual3D.MaterialProperty, (object) value);
    }

    internal static void OnMaterialPropertyChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      ((InteractiveVisual3D) sender).GenerateMaterial();
    }

    public Geometry3D Geometry
    {
      get => (Geometry3D) this.GetValue(InteractiveVisual3D.GeometryProperty);
      set => this.SetValue(InteractiveVisual3D.GeometryProperty, (object) value);
    }

    internal static void OnGeometryChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      InteractiveVisual3D interactiveVisual3D = (InteractiveVisual3D) sender;
      interactiveVisual3D._content.Geometry = interactiveVisual3D.Geometry;
    }

    public static void SetIsInteractiveMaterial(UIElement element, bool value) => element.SetValue(InteractiveVisual3D.IsInteractiveMaterialProperty, (object) value);

    public static bool GetIsInteractiveMaterial(UIElement element) => (bool) element.GetValue(InteractiveVisual3D.IsInteractiveMaterialProperty);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Model3D Content
    {
      get => base.Content;
      set => base.Content = value;
    }

    private struct Edge
    {
      public Point3D _start;
      public Point3D _end;

      public Edge(Point3D s, Point3D e)
      {
        this._start = s;
        this._end = e;
      }
    }

    private class EdgeInfo
    {
      public bool hasFrontFace;
      public bool hasBackFace;
      public Point uv1;
      public Point uv2;
      public int numSharing;

      public EdgeInfo()
      {
        this.hasFrontFace = this.hasBackFace = false;
        this.numSharing = 0;
      }
    }

    private enum PolygonSide
    {
      FRONT,
      BACK,
    }
  }
}
