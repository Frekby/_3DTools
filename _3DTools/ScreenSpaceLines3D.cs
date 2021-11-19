// Decompiled with JetBrains decompiler
// Type: _3DTools.ScreenSpaceLines3D
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DTools
{
  public class ScreenSpaceLines3D : ModelVisual3D
  {
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof (Color), typeof (Color), typeof (ScreenSpaceLines3D), new PropertyMetadata((object) Colors.White, new PropertyChangedCallback(ScreenSpaceLines3D.OnColorChanged)));
    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof (Thickness), typeof (double), typeof (ScreenSpaceLines3D), new PropertyMetadata((object) 1.0, new PropertyChangedCallback(ScreenSpaceLines3D.OnThicknessChanged)));
    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(nameof (Points), typeof (Point3DCollection), typeof (ScreenSpaceLines3D), new PropertyMetadata((object) null, new PropertyChangedCallback(ScreenSpaceLines3D.OnPointsChanged)));
    private Matrix3D _visualToScreen;
    private Matrix3D _screenToVisual;
    private readonly GeometryModel3D _model;
    private readonly MeshGeometry3D _mesh;

    public ScreenSpaceLines3D()
    {
      this._mesh = new MeshGeometry3D();
      this._model = new GeometryModel3D();
      this._model.Geometry = (Geometry3D) this._mesh;
      this.SetColor(this.Color);
      this.Content = (Model3D) this._model;
      this.Points = new Point3DCollection();
      CompositionTarget.Rendering += new EventHandler(this.OnRender);
    }

    private static void OnColorChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      ((ScreenSpaceLines3D) sender).SetColor((Color) args.NewValue);
    }

    private void SetColor(Color color)
    {
      MaterialGroup materialGroup = new MaterialGroup();
      materialGroup.Children.Add((Material) new DiffuseMaterial((Brush) new SolidColorBrush(Colors.Black)));
      materialGroup.Children.Add((Material) new EmissiveMaterial((Brush) new SolidColorBrush(color)));
      materialGroup.Freeze();
      this._model.Material = (Material) materialGroup;
      this._model.BackMaterial = (Material) materialGroup;
    }

    public Color Color
    {
      get => (Color) this.GetValue(ScreenSpaceLines3D.ColorProperty);
      set => this.SetValue(ScreenSpaceLines3D.ColorProperty, (object) value);
    }

    private static void OnThicknessChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      ((ScreenSpaceLines3D) sender).GeometryDirty();
    }

    public double Thickness
    {
      get => (double) this.GetValue(ScreenSpaceLines3D.ThicknessProperty);
      set => this.SetValue(ScreenSpaceLines3D.ThicknessProperty, (object) value);
    }

    private static void OnPointsChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      ((ScreenSpaceLines3D) sender).GeometryDirty();
    }

    public Point3DCollection Points
    {
      get => (Point3DCollection) this.GetValue(ScreenSpaceLines3D.PointsProperty);
      set => this.SetValue(ScreenSpaceLines3D.PointsProperty, (object) value);
    }

    private void OnRender(object sender, EventArgs e)
    {
      if (this.Points.Count == 0 && this._mesh.Positions.Count == 0 || !this.UpdateTransforms())
        return;
      this.RebuildGeometry();
    }

    private void GeometryDirty() => this._visualToScreen = MathUtils.ZeroMatrix;

    private void RebuildGeometry()
    {
      double halfThickness = this.Thickness / 2.0;
      int num = this.Points.Count / 2;
      Point3DCollection positions = new Point3DCollection(num * 4);
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = index1 * 2;
        Point3D point1 = this.Points[index2];
        Point3D point2 = this.Points[index2 + 1];
        this.AddSegment(positions, point1, point2, halfThickness);
      }
      positions.Freeze();
      this._mesh.Positions = positions;
      Int32Collection int32Collection = new Int32Collection(this.Points.Count * 3);
      for (int index = 0; index < this.Points.Count / 2; ++index)
      {
        int32Collection.Add(index * 4 + 2);
        int32Collection.Add(index * 4 + 1);
        int32Collection.Add(index * 4);
        int32Collection.Add(index * 4 + 2);
        int32Collection.Add(index * 4 + 3);
        int32Collection.Add(index * 4 + 1);
      }
      int32Collection.Freeze();
      this._mesh.TriangleIndices = int32Collection;
    }

    private void AddSegment(
      Point3DCollection positions,
      Point3D startPoint,
      Point3D endPoint,
      double halfThickness)
    {
      Vector3D vector3D = endPoint * this._visualToScreen - startPoint * this._visualToScreen;
      vector3D.Z = 0.0;
      vector3D.Normalize();
      Vector delta = new Vector(-vector3D.Y, vector3D.X) * halfThickness;
      Point3D pOut1;
      Point3D pOut2;
      this.Widen(startPoint, delta, out pOut1, out pOut2);
      positions.Add(pOut1);
      positions.Add(pOut2);
      this.Widen(endPoint, delta, out pOut1, out pOut2);
      positions.Add(pOut1);
      positions.Add(pOut2);
    }

    private void Widen(Point3D pIn, Vector delta, out Point3D pOut1, out Point3D pOut2)
    {
      Point4D point4D1 = (Point4D) pIn * this._visualToScreen;
      Point4D point4D2 = point4D1;
      point4D1.X += delta.X * point4D1.W;
      point4D1.Y += delta.Y * point4D1.W;
      point4D2.X -= delta.X * point4D2.W;
      point4D2.Y -= delta.Y * point4D2.W;
      Point4D point4D3 = point4D1 * this._screenToVisual;
      Point4D point4D4 = point4D2 * this._screenToVisual;
      pOut1 = new Point3D(point4D3.X / point4D3.W, point4D3.Y / point4D3.W, point4D3.Z / point4D3.W);
      pOut2 = new Point3D(point4D4.X / point4D4.W, point4D4.Y / point4D4.W, point4D4.Z / point4D4.W);
    }

    private bool UpdateTransforms()
    {
      bool success;
      Matrix3D matrix3D = MathUtils.TryTransformTo2DAncestor((DependencyObject) this, out Viewport3DVisual _, out success);
      if (!success || !matrix3D.HasInverse)
      {
        this._mesh.Positions = (Point3DCollection) null;
        return false;
      }
      if (matrix3D == this._visualToScreen)
        return false;
      this._visualToScreen = this._screenToVisual = matrix3D;
      this._screenToVisual.Invert();
      return true;
    }

    public void MakeWireframe(Model3D model)
    {
      this.Points.Clear();
      if (model == null)
        return;
      Matrix3DStack matrixStack = new Matrix3DStack();
      matrixStack.Push(Matrix3D.Identity);
      this.WireframeHelper(model, matrixStack);
    }

    private void WireframeHelper(Model3D model, Matrix3DStack matrixStack)
    {
      Transform3D transform = model.Transform;
      if (transform != null)
      {
        if (transform != Transform3D.Identity)
          matrixStack.Prepend(model.Transform.Value);
      }
      try
      {
        switch (model)
        {
          case Model3DGroup group2:
            this.WireframeHelper(group2, matrixStack);
            break;
          case GeometryModel3D model3:
            this.WireframeHelper(model3, matrixStack);
            break;
        }
      }
      finally
      {
        if (transform != null && transform != Transform3D.Identity)
          matrixStack.Pop();
      }
    }

    private void WireframeHelper(Model3DGroup group, Matrix3DStack matrixStack)
    {
      foreach (Model3D child in group.Children)
        this.WireframeHelper(child, matrixStack);
    }

    private void WireframeHelper(GeometryModel3D model, Matrix3DStack matrixStack)
    {
      if (!(model.Geometry is MeshGeometry3D geometry))
        return;
      Point3D[] point3DArray = new Point3D[geometry.Positions.Count];
      geometry.Positions.CopyTo(point3DArray, 0);
      matrixStack.Peek().Transform(point3DArray);
      Int32Collection triangleIndices = geometry.TriangleIndices;
      if (triangleIndices.Count > 0)
      {
        int num = point3DArray.Length - 1;
        int index = 2;
        for (int count = triangleIndices.Count; index < count; index += 3)
        {
          int i0 = triangleIndices[index - 2];
          int i1 = triangleIndices[index - 1];
          int i2 = triangleIndices[index];
          if (0 > i0 || i0 > num || 0 > i1 || i1 > num || 0 > i2 || i2 > num)
            break;
          this.AddTriangle(point3DArray, i0, i1, i2);
        }
      }
      else
      {
        int num = 2;
        for (int length = point3DArray.Length; num < length; num += 3)
        {
          int i0 = num - 2;
          int i1 = num - 1;
          int i2 = num;
          this.AddTriangle(point3DArray, i0, i1, i2);
        }
      }
    }

    private void AddTriangle(Point3D[] positions, int i0, int i1, int i2)
    {
      this.Points.Add(positions[i0]);
      this.Points.Add(positions[i1]);
      this.Points.Add(positions[i1]);
      this.Points.Add(positions[i2]);
      this.Points.Add(positions[i2]);
      this.Points.Add(positions[i0]);
    }
  }
}
