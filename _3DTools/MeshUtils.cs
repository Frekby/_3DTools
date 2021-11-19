// Decompiled with JetBrains decompiler
// Type: _3DTools.MeshUtils
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DTools
{
  public static class MeshUtils
  {
    public static PointCollection GenerateCylindricalTextureCoordinates(
      MeshGeometry3D mesh,
      Vector3D dir)
    {
      if (mesh == null)
        return (PointCollection) null;
      Rect3D bounds = mesh.Bounds;
      PointCollection pointCollection = new PointCollection(mesh.Positions.Count);
      foreach (Point3D transformPoint in MeshUtils.TransformPoints(ref bounds, mesh.Positions, ref dir))
        pointCollection.Add(new Point(MeshUtils.GetUnitCircleCoordinate(-transformPoint.Z, transformPoint.X), 1.0 - MeshUtils.GetPlanarCoordinate(transformPoint.Y, bounds.Y, bounds.SizeY)));
      return pointCollection;
    }

    public static PointCollection GenerateSphericalTextureCoordinates(
      MeshGeometry3D mesh,
      Vector3D dir)
    {
      if (mesh == null)
        return (PointCollection) null;
      Rect3D bounds = mesh.Bounds;
      PointCollection pointCollection = new PointCollection(mesh.Positions.Count);
      foreach (Point3D transformPoint in MeshUtils.TransformPoints(ref bounds, mesh.Positions, ref dir))
      {
        Vector3D v = new Vector3D(transformPoint.X, transformPoint.Y, transformPoint.Z);
        MathUtils.TryNormalize(ref v);
        pointCollection.Add(new Point(MeshUtils.GetUnitCircleCoordinate(-v.Z, v.X), 1.0 - (Math.Asin(v.Y) / Math.PI + 0.5)));
      }
      return pointCollection;
    }

    public static PointCollection GeneratePlanarTextureCoordinates(
      MeshGeometry3D mesh,
      Vector3D dir)
    {
      if (mesh == null)
        return (PointCollection) null;
      Rect3D bounds = mesh.Bounds;
      PointCollection pointCollection = new PointCollection(mesh.Positions.Count);
      foreach (Point3D transformPoint in MeshUtils.TransformPoints(ref bounds, mesh.Positions, ref dir))
        pointCollection.Add(new Point(MeshUtils.GetPlanarCoordinate(transformPoint.X, bounds.X, bounds.SizeX), MeshUtils.GetPlanarCoordinate(transformPoint.Z, bounds.Z, bounds.SizeZ)));
      return pointCollection;
    }

    internal static double GetPlanarCoordinate(double end, double start, double width) => (end - start) / width;

    internal static double GetUnitCircleCoordinate(double y, double x) => Math.Atan2(y, x) / (2.0 * Math.PI) + 0.5;

    internal static IEnumerable<Point3D> TransformPoints(
      ref Rect3D bounds,
      Point3DCollection points,
      ref Vector3D dir)
    {
      if (dir == MathUtils.YAxis)
        return (IEnumerable<Point3D>) points;
      Vector3D axisOfRotation = Vector3D.CrossProduct(dir, MathUtils.YAxis);
      double angleInDegrees = Vector3D.AngleBetween(dir, MathUtils.YAxis);
      Quaternion quaternion = axisOfRotation.X != 0.0 || axisOfRotation.Y != 0.0 || axisOfRotation.Z != 0.0 ? new Quaternion(axisOfRotation, angleInDegrees) : new Quaternion(MathUtils.XAxis, angleInDegrees);
      Vector3D vector3D = new Vector3D(bounds.X + bounds.SizeX / 2.0, bounds.Y + bounds.SizeY / 2.0, bounds.Z + bounds.SizeZ / 2.0);
      Matrix3D identity = Matrix3D.Identity;
      identity.Translate(-vector3D);
      identity.Rotate(quaternion);
      int count = points.Count;
      Point3D[] point3DArray = new Point3D[count];
      for (int index = 0; index < count; ++index)
        point3DArray[index] = identity.Transform(points[index]);
      bounds = MathUtils.TransformBounds(bounds, identity);
      return (IEnumerable<Point3D>) point3DArray;
    }
  }
}
