// Decompiled with JetBrains decompiler
// Type: _3DTools.MathUtils
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DTools
{
  public static class MathUtils
  {
    public static readonly Matrix3D ZeroMatrix = new Matrix3D(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
    public static readonly Vector3D XAxis = new Vector3D(1.0, 0.0, 0.0);
    public static readonly Vector3D YAxis = new Vector3D(0.0, 1.0, 0.0);
    public static readonly Vector3D ZAxis = new Vector3D(0.0, 0.0, 1.0);

    public static double GetAspectRatio(Size size) => size.Width / size.Height;

    public static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);

    private static Matrix3D GetViewMatrix(ProjectionCamera camera)
    {
      Vector3D vector3D1 = -camera.LookDirection;
      vector3D1.Normalize();
      Vector3D vector3D2 = Vector3D.CrossProduct(camera.UpDirection, vector3D1);
      vector3D2.Normalize();
      Vector3D vector1 = Vector3D.CrossProduct(vector3D1, vector3D2);
      Vector3D position = (Vector3D) camera.Position;
      double offsetX = -Vector3D.DotProduct(vector3D2, position);
      double offsetY = -Vector3D.DotProduct(vector1, position);
      double offsetZ = -Vector3D.DotProduct(vector3D1, position);
      return new Matrix3D(vector3D2.X, vector1.X, vector3D1.X, 0.0, vector3D2.Y, vector1.Y, vector3D1.Y, 0.0, vector3D2.Z, vector1.Z, vector3D1.Z, 0.0, offsetX, offsetY, offsetZ, 1.0);
    }

    public static Matrix3D GetViewMatrix(Camera camera)
    {
      switch (camera)
      {
        case null:
          throw new ArgumentNullException(nameof (camera));
        case ProjectionCamera camera1:
          return MathUtils.GetViewMatrix(camera1);
        case MatrixCamera matrixCamera:
          return matrixCamera.ViewMatrix;
        default:
          throw new ArgumentException(string.Format("Unsupported camera type '{0}'.", (object) camera.GetType().FullName), nameof (camera));
      }
    }

    private static Matrix3D GetProjectionMatrix(
      OrthographicCamera camera,
      double aspectRatio)
    {
      double width = camera.Width;
      double num = width / aspectRatio;
      double nearPlaneDistance = camera.NearPlaneDistance;
      double farPlaneDistance = camera.FarPlaneDistance;
      double m33 = 1.0 / (nearPlaneDistance - farPlaneDistance);
      double offsetZ = nearPlaneDistance * m33;
      return new Matrix3D(2.0 / width, 0.0, 0.0, 0.0, 0.0, 2.0 / num, 0.0, 0.0, 0.0, 0.0, m33, 0.0, 0.0, 0.0, offsetZ, 1.0);
    }

    private static Matrix3D GetProjectionMatrix(
      PerspectiveCamera camera,
      double aspectRatio)
    {
      double radians = MathUtils.DegreesToRadians(camera.FieldOfView);
      double nearPlaneDistance = camera.NearPlaneDistance;
      double farPlaneDistance = camera.FarPlaneDistance;
      double m11 = 1.0 / Math.Tan(radians / 2.0);
      double m22 = aspectRatio * m11;
      double m33 = farPlaneDistance == double.PositiveInfinity ? -1.0 : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
      double offsetZ = nearPlaneDistance * m33;
      return new Matrix3D(m11, 0.0, 0.0, 0.0, 0.0, m22, 0.0, 0.0, 0.0, 0.0, m33, -1.0, 0.0, 0.0, offsetZ, 0.0);
    }

    public static Matrix3D GetProjectionMatrix(Camera camera, double aspectRatio)
    {
      switch (camera)
      {
        case null:
          throw new ArgumentNullException(nameof (camera));
        case PerspectiveCamera camera1:
          return MathUtils.GetProjectionMatrix(camera1, aspectRatio);
        case OrthographicCamera camera2:
          return MathUtils.GetProjectionMatrix(camera2, aspectRatio);
        case MatrixCamera matrixCamera:
          return matrixCamera.ProjectionMatrix;
        default:
          throw new ArgumentException(string.Format("Unsupported camera type '{0}'.", (object) camera.GetType().FullName), nameof (camera));
      }
    }

    private static Matrix3D GetHomogeneousToViewportTransform(Rect viewport)
    {
      double m11 = viewport.Width / 2.0;
      double num = viewport.Height / 2.0;
      double offsetX = viewport.X + m11;
      double offsetY = viewport.Y + num;
      return new Matrix3D(m11, 0.0, 0.0, 0.0, 0.0, -num, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, offsetX, offsetY, 0.0, 1.0);
    }

    public static Matrix3D TryWorldToViewportTransform(
      Viewport3DVisual visual,
      out bool success)
    {
      success = false;
      Matrix3D cameraTransform = MathUtils.TryWorldToCameraTransform(visual, out success);
      if (success)
      {
        cameraTransform.Append(MathUtils.GetProjectionMatrix(visual.Camera, MathUtils.GetAspectRatio(visual.Viewport.Size)));
        cameraTransform.Append(MathUtils.GetHomogeneousToViewportTransform(visual.Viewport));
        success = true;
      }
      return cameraTransform;
    }

    public static Matrix3D TryWorldToCameraTransform(
      Viewport3DVisual visual,
      out bool success)
    {
      success = false;
      Matrix3D identity = Matrix3D.Identity;
      Camera camera = visual.Camera;
      if (camera == null || visual.Viewport == Rect.Empty)
        return MathUtils.ZeroMatrix;
      Transform3D transform = camera.Transform;
      if (transform != null)
      {
        Matrix3D matrix = transform.Value;
        if (!matrix.HasInverse)
          return MathUtils.ZeroMatrix;
        matrix.Invert();
        identity.Append(matrix);
      }
      identity.Append(MathUtils.GetViewMatrix(camera));
      success = true;
      return identity;
    }

    private static Matrix3D GetWorldTransformationMatrix(
      DependencyObject visual,
      out Viewport3DVisual viewport)
    {
      Matrix3D identity = Matrix3D.Identity;
      viewport = (Viewport3DVisual) null;
      if (!(visual is Visual3D))
        throw new ArgumentException("Must be of type Visual3D.", nameof (visual));
      for (; visual != null && visual is ModelVisual3D; visual = VisualTreeHelper.GetParent(visual))
      {
        Transform3D transform3D = (Transform3D) visual.GetValue(ModelVisual3D.TransformProperty);
        if (transform3D != null)
          identity.Append(transform3D.Value);
      }
      viewport = visual as Viewport3DVisual;
      if (viewport != null)
        return identity;
      if (visual != null)
        throw new ApplicationException(string.Format("Unsupported type: '{0}'.  Expected tree of ModelVisual3Ds leading up to a Viewport3DVisual.", (object) visual.GetType().FullName));
      return MathUtils.ZeroMatrix;
    }

    public static Matrix3D TryTransformTo2DAncestor(
      DependencyObject visual,
      out Viewport3DVisual viewport,
      out bool success)
    {
      Matrix3D transformationMatrix = MathUtils.GetWorldTransformationMatrix(visual, out viewport);
      transformationMatrix.Append(MathUtils.TryWorldToViewportTransform(viewport, out success));
      return !success ? MathUtils.ZeroMatrix : transformationMatrix;
    }

    public static Matrix3D TryTransformToCameraSpace(
      DependencyObject visual,
      out Viewport3DVisual viewport,
      out bool success)
    {
      Matrix3D transformationMatrix = MathUtils.GetWorldTransformationMatrix(visual, out viewport);
      transformationMatrix.Append(MathUtils.TryWorldToCameraTransform(viewport, out success));
      return !success ? MathUtils.ZeroMatrix : transformationMatrix;
    }

    public static Rect3D TransformBounds(Rect3D bounds, Matrix3D transform)
    {
      double x1 = bounds.X;
      double y1 = bounds.Y;
      double z1 = bounds.Z;
      double x2 = bounds.X + bounds.SizeX;
      double y2 = bounds.Y + bounds.SizeY;
      double z2 = bounds.Z + bounds.SizeZ;
      Point3D[] points = new Point3D[8]
      {
        new Point3D(x1, y1, z1),
        new Point3D(x1, y1, z2),
        new Point3D(x1, y2, z1),
        new Point3D(x1, y2, z2),
        new Point3D(x2, y1, z1),
        new Point3D(x2, y1, z2),
        new Point3D(x2, y2, z1),
        new Point3D(x2, y2, z2)
      };
      transform.Transform(points);
      Point3D point3D1 = points[0];
      double val1_1;
      double num1 = val1_1 = point3D1.X;
      double val1_2;
      double num2 = val1_2 = point3D1.Y;
      double val1_3;
      double num3 = val1_3 = point3D1.Z;
      for (int index = 1; index < points.Length; ++index)
      {
        Point3D point3D2 = points[index];
        num1 = Math.Min(num1, point3D2.X);
        num2 = Math.Min(num2, point3D2.Y);
        num3 = Math.Min(num3, point3D2.Z);
        val1_1 = Math.Max(val1_1, point3D2.X);
        val1_2 = Math.Max(val1_2, point3D2.Y);
        val1_3 = Math.Max(val1_3, point3D2.Z);
      }
      return new Rect3D(num1, num2, num3, val1_1 - num1, val1_2 - num2, val1_3 - num3);
    }

    public static bool TryNormalize(ref Vector3D v)
    {
      double length = v.Length;
      if (length == 0.0)
        return false;
      v /= length;
      return true;
    }

    public static Point3D GetCenter(Rect3D box) => new Point3D(box.X + box.SizeX / 2.0, box.Y + box.SizeY / 2.0, box.Z + box.SizeZ / 2.0);
  }
}
