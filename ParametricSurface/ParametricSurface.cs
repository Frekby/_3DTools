// Decompiled with JetBrains decompiler
// Type: ParametricSurface
// Assembly: ParametricSurface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D708E329-3A64-4FEA-AEF5-419EDD255873
// Assembly location: D:\C#\WPF\test\test\ParametricSurface.dll

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

public class ParametricSurface
{
  private int nu = 30;
  private int nv = 30;
  private double umin = -3.0;
  private double umax = 3.0;
  private double vmin = -8.0;
  private double vmax = 8.0;
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

  public double Umin
  {
    get => this.umin;
    set => this.umin = value;
  }

  public double Umax
  {
    get => this.umax;
    set => this.umax = value;
  }

  public double Vmin
  {
    get => this.vmin;
    set => this.vmin = value;
  }

  public double Vmax
  {
    get => this.vmax;
    set => this.vmax = value;
  }

  public int Nu
  {
    get => this.nu;
    set => this.nu = value;
  }

  public int Nv
  {
    get => this.nv;
    set => this.nv = value;
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

  public void CreateSurface(ParametricSurface.Function f)
  {
    double num1 = (this.Umax - this.Umin) / (double) (this.Nu - 1);
    double num2 = (this.Vmax - this.Vmin) / (double) (this.Nv - 1);
    if (this.Nu < 2 || this.Nv < 2)
      return;
    Point3D[,] point3DArray1 = new Point3D[this.Nu, this.Nv];
    for (int index1 = 0; index1 < this.Nu; ++index1)
    {
      double u = this.Umin + (double) index1 * num1;
      for (int index2 = 0; index2 < this.Nv; ++index2)
      {
        double v = this.Vmin + (double) index2 * num2;
        point3DArray1[index1, index2] = f(u, v);
        point3DArray1[index1, index2] += (Vector3D) this.Center;
        point3DArray1[index1, index2] = Utility.GetNormalize(point3DArray1[index1, index2]);
      }
    }
    Point3D[] point3DArray2 = new Point3D[4];
    for (int index3 = 0; index3 < this.Nu - 1; ++index3)
    {
      for (int index4 = 0; index4 < this.Nv - 1; ++index4)
      {
        point3DArray2[0] = point3DArray1[index3, index4];
        point3DArray2[1] = point3DArray1[index3, index4 + 1];
        point3DArray2[2] = point3DArray1[index3 + 1, index4 + 1];
        point3DArray2[3] = point3DArray1[index3 + 1, index4];
        if (!this.IsHiddenLine)
          Utility.CreateRectangleFace(point3DArray2[0], point3DArray2[1], point3DArray2[2], point3DArray2[3], this.SurfaceColor, this.Viewport3d);
        if (this.IsWireframe)
          Utility.CreateWireframe(point3DArray2[0], point3DArray2[1], point3DArray2[2], point3DArray2[3], this.LineColor, this.Viewport3d);
      }
    }
  }

  public delegate Point3D Function(double u, double v);
}
