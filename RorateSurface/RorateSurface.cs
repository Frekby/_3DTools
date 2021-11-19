// Decompiled with JetBrains decompiler
// Type: RorateSurface
// Assembly: RotateSurface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2766D183-1922-4B1F-9057-A23CEE1E2AD7
// Assembly location: D:\C#\WPF\test\test\RotateSurface.dll

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

public class RorateSurface
{
  private List<Point3D> curvePoints = new List<Point3D>();
  private int thetaDiv = 20;
  private double xmin = -1.0;
  private double xmax = 1.0;
  private double ymin = -1.0;
  private double ymax = 1.0;
  private double zmin = -1.0;
  private double zmax = 1.0;
  private Color lineColor = Colors.Black;
  private Color surfaceColor = Colors.White;
  private Point3D center = new Point3D();
  private bool isHiddenLine = false;
  private bool isWireframe = true;
  private Viewport3D viewport3d = new Viewport3D();

  public bool IsWireframe
  {
    get => this.isWireframe;
    set => this.isWireframe = value;
  }

  public int ThetaDiv
  {
    get => this.thetaDiv;
    set => this.thetaDiv = value;
  }

  public bool IsHiddenLine
  {
    get => this.isHiddenLine;
    set => this.isHiddenLine = value;
  }

  public Color LineColor
  {
    get => this.lineColor;
    set => this.lineColor = value;
  }

  public Color SurfaceColor
  {
    get => this.surfaceColor;
    set => this.surfaceColor = value;
  }

  public List<Point3D> CurvePoints
  {
    get => this.curvePoints;
    set => this.curvePoints = value;
  }

  public double Xmin
  {
    get => this.xmin;
    set => this.xmin = value;
  }

  public double Xmax
  {
    get => this.xmax;
    set => this.xmax = value;
  }

  public double Ymin
  {
    get => this.ymin;
    set => this.ymin = value;
  }

  public double Ymax
  {
    get => this.ymax;
    set => this.ymax = value;
  }

  public double Zmin
  {
    get => this.zmin;
    set => this.zmin = value;
  }

  public double Zmax
  {
    get => this.zmax;
    set => this.zmax = value;
  }

  public Point3D Center
  {
    get => this.center;
    set => this.center = value;
  }

  public Viewport3D Viewport3d
  {
    get => this.viewport3d;
    set => this.viewport3d = value;
  }

  private Point3D GetPosition(double r, double y, double theta)
  {
    double x = r * Math.Cos(theta);
    double z = -r * Math.Sin(theta);
    return new Point3D(x, y, z);
  }

  public void CreateSurface()
  {
    Point3D[,] point3DArray1 = new Point3D[this.ThetaDiv, this.CurvePoints.Count];
    for (int index1 = 0; index1 < this.ThetaDiv; ++index1)
    {
      double theta = (double) (index1 * 2) * Math.PI / (double) (this.ThetaDiv - 1);
      for (int index2 = 0; index2 < this.CurvePoints.Count; ++index2)
      {
        double x = this.CurvePoints[index2].X;
        double z = this.CurvePoints[index2].Z;
        double r = Math.Sqrt(x * x + z * z);
        point3DArray1[index1, index2] = this.GetPosition(r, this.CurvePoints[index2].Y, theta);
        point3DArray1[index1, index2] += (Vector3D) this.Center;
        point3DArray1[index1, index2] = Utility.GetNormalize(point3DArray1[index1, index2]);
      }
    }
    Point3D[] point3DArray2 = new Point3D[4];
    for (int index3 = 0; index3 < this.ThetaDiv - 1; ++index3)
    {
      for (int index4 = 0; index4 < this.CurvePoints.Count - 1; ++index4)
      {
        point3DArray2[0] = point3DArray1[index3, index4];
        point3DArray2[1] = point3DArray1[index3 + 1, index4];
        point3DArray2[2] = point3DArray1[index3 + 1, index4 + 1];
        point3DArray2[3] = point3DArray1[index3, index4 + 1];
        if (!this.IsHiddenLine)
          Utility.CreateRectangleFace(point3DArray2[0], point3DArray2[1], point3DArray2[2], point3DArray2[3], this.SurfaceColor, this.Viewport3d);
        if (this.IsWireframe)
          Utility.CreateWireframe(point3DArray2[0], point3DArray2[1], point3DArray2[2], point3DArray2[3], this.LineColor, this.Viewport3d);
      }
    }
  }
}
