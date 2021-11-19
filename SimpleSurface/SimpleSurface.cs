// Decompiled with JetBrains decompiler
// Type: SimpleSurface
// Assembly: SimpleSurface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 423692F6-C5F0-4A10-A227-FEAA70F134AB
// Assembly location: D:\C#\WPF\test\test\SimpleSurface.dll

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

public class SimpleSurface
{
  private double xmin = -3.0;
  private double xmax = 3.0;
  private double ymin = -8.0;
  private double ymax = 8.0;
  private double zmin = -3.0;
  private double zmax = 3.0;
  private int nx = 30;
  private int nz = 30;
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

  public int Nx
  {
    get => this.nx;
    set => this.nx = value;
  }

  public int Nz
  {
    get => this.nz;
    set => this.nz = value;
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

  public void CreateSurface(SimpleSurface.Function f)
  {
    double num1 = (this.Xmax - this.Xmin) / (double) this.Nx;
    double num2 = (this.Zmax - this.Zmin) / (double) this.Nz;
    if (this.Nx < 2 || this.Nz < 2)
      return;
    Point3D[,] point3DArray1 = new Point3D[this.Nx, this.Nz];
    for (int index1 = 0; index1 < this.Nx; ++index1)
    {
      double x = this.Xmin + (double) index1 * num1;
      for (int index2 = 0; index2 < this.Nz; ++index2)
      {
        double z = this.Zmin + (double) index2 * num2;
        point3DArray1[index1, index2] = f(x, z);
        point3DArray1[index1, index2] += (Vector3D) this.Center;
        point3DArray1[index1, index2] = Utility.GetNormalize(point3DArray1[index1, index2]);
      }
    }
    Point3D[] point3DArray2 = new Point3D[4];
    for (int index3 = 0; index3 < this.Nx - 1; ++index3)
    {
      for (int index4 = 0; index4 < this.Nz - 1; ++index4)
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

  public delegate Point3D Function(double x, double z);
}
