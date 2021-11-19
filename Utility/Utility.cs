// Decompiled with JetBrains decompiler
// Type: Utility
// Assembly: Utility, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 27F001A9-7FE1-4F60-B581-151A6B4319F9
// Assembly location: D:\C#\WPF\test\test\Utility.dll

using System;
using _3DTools;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

public class Utility
{
  public static void CreateRectangleFace(
    Point3D p0,
    Point3D p1,
    Point3D p2,
    Point3D p3,
    Color surfaceColor,
    Viewport3D viewport)
  {
    MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
    meshGeometry3D.Positions.Add(p0);
    meshGeometry3D.Positions.Add(p1);
    meshGeometry3D.Positions.Add(p2);
    meshGeometry3D.Positions.Add(p3);
    
    meshGeometry3D.Positions.Add(p0);
    meshGeometry3D.Positions.Add(p1);
    meshGeometry3D.Positions.Add(p2);
    meshGeometry3D.Positions.Add(p3);
    
    meshGeometry3D.TriangleIndices.Add(0);
    meshGeometry3D.TriangleIndices.Add(1);
    meshGeometry3D.TriangleIndices.Add(2);
    meshGeometry3D.TriangleIndices.Add(2);
    meshGeometry3D.TriangleIndices.Add(3);
    meshGeometry3D.TriangleIndices.Add(0);
    
    meshGeometry3D.TriangleIndices.Add(4);
    meshGeometry3D.TriangleIndices.Add(7);
    meshGeometry3D.TriangleIndices.Add(6);
    meshGeometry3D.TriangleIndices.Add(6);
    meshGeometry3D.TriangleIndices.Add(5);
    meshGeometry3D.TriangleIndices.Add(4);
    Material material = (Material) new DiffuseMaterial((Brush) new SolidColorBrush()
    {
      Color = surfaceColor
    });
    
    var material2 = new MaterialGroup();
    var diffuse = new DiffuseMaterial(new SolidColorBrush(surfaceColor));
    var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 1000);
    var emmMat = new EmissiveMaterial(new SolidColorBrush(Colors.Black));
    
    material2.Children.Add(diffuse);
    //material2.Children.Add(specMat);
    material2.Children.Add(emmMat);

    GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) meshGeometry3D, material2);
    viewport.Children.Add((Visual3D) new ModelVisual3D()
    {
      Content = (Model3D) geometryModel3D
    });
  }

  public static void CreateWireframe(
    Point3D p0,
    Point3D p1,
    Point3D p2,
    Point3D p3,
    Color lineColor,
    Viewport3D viewport)
  {
    viewport.Children.Add((Visual3D) new ScreenSpaceLines3D()
    {
      Points = {
        p0,
        p1,
        p1,
        p2,
        p2,
        p3,
        p3,
        p0
      },
      Color = lineColor,
      Thickness = 1.0
    });
  }

  public static Point3D GetNormalize(
    Point3D pt)
  {
    pt.X = pt.X;
    pt.Y = pt.Y;
    pt.Z = pt.Z;
    return pt;
  }

  public static void XYZcoord(Viewport3D viewport)
  {
    viewport.Children.Add((Visual3D) new ScreenSpaceLines3D()
    {
      Points = {
        new Point3D(0,0,0),
        new Point3D(1,0,0),
      },
      Color = Colors.Red,
      Thickness = 2.0
    });
    
    viewport.Children.Add((Visual3D) new ScreenSpaceLines3D()
    {
      Points = {
        new Point3D(0,0,0),
        new Point3D(0,1,0),
      },
      Color = Colors.Blue,
      Thickness = 2.0
    });
    
    viewport.Children.Add((Visual3D) new ScreenSpaceLines3D()
    {
      Points = {
        new Point3D(0,0,0),
        new Point3D(0,0,1),
      },
      Color = Colors.Green,
      Thickness = 2.0
    });
    
  }
  
}
