// Decompiled with JetBrains decompiler
// Type: _3DTools.TrackballDecorator
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DTools
{
  public class TrackballDecorator : Viewport3DDecorator
  {
    private Point _previousPosition2D;
    private Vector3D _previousPosition3D = new Vector3D(0.0, 0.0, 1.0);
    private Transform3DGroup _transform;
    private ScaleTransform3D _scale = new ScaleTransform3D();
    private AxisAngleRotation3D _rotation = new AxisAngleRotation3D();
    private Border _eventSource;

    public TrackballDecorator()
    {
      this._transform = new Transform3DGroup();
      this._transform.Children.Add((Transform3D) this._scale);
      this._transform.Children.Add((Transform3D) new RotateTransform3D((Rotation3D) this._rotation));
      this._eventSource = new Border();
      this._eventSource.Background = (Brush) Brushes.Transparent;
      this.PreViewportChildren.Add((UIElement) this._eventSource);
    }

    public Transform3D Transform => (Transform3D) this._transform;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      this._previousPosition2D = e.GetPosition((IInputElement) this);
      this._previousPosition3D = this.ProjectToTrackball(this.ActualWidth, this.ActualHeight, this._previousPosition2D);
      if (Mouse.Captured != null)
        return;
      Mouse.Capture((IInputElement) this, CaptureMode.Element);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);
      if (!this.IsMouseCaptured)
        return;
      Mouse.Capture((IInputElement) this, CaptureMode.None);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!this.IsMouseCaptured)
        return;
      Point position = e.GetPosition((IInputElement) this);
      if (position == this._previousPosition2D)
        return;
      if (e.LeftButton == MouseButtonState.Pressed)
        this.Track(position);
      else if (e.RightButton == MouseButtonState.Pressed)
        this.Zoom(position);
      this._previousPosition2D = position;
      Viewport3D viewport3D = this.Viewport3D;
      if (viewport3D == null || viewport3D.Camera == null)
        return;
      if (viewport3D.Camera.IsFrozen)
        viewport3D.Camera = viewport3D.Camera.Clone();
      if (viewport3D.Camera.Transform == this._transform)
        return;
      viewport3D.Camera.Transform = (Transform3D) this._transform;
    }

    private void Track(Point currentPosition)
    {
      Vector3D trackball = this.ProjectToTrackball(this.ActualWidth, this.ActualHeight, currentPosition);
      Vector3D axisOfRotation = Vector3D.CrossProduct(this._previousPosition3D, trackball);
      double num = Vector3D.AngleBetween(this._previousPosition3D, trackball);
      if (axisOfRotation.Length == 0.0)
        return;
      Quaternion quaternion = new Quaternion(this._rotation.Axis, this._rotation.Angle) * new Quaternion(axisOfRotation, -num);
      this._rotation.Axis = quaternion.Axis;
      this._rotation.Angle = quaternion.Angle;
      this._previousPosition3D = trackball;
    }

    private Vector3D ProjectToTrackball(double width, double height, Point point)
    {
      double num1 = point.X / (width / 2.0);
      double num2 = point.Y / (height / 2.0);
      double x = num1 - 1.0;
      double y = 1.0 - num2;
      double d = 1.0 - x * x - y * y;
      double z = d > 0.0 ? Math.Sqrt(d) : 0.0;
      return new Vector3D(x, y, z);
    }

    private void Zoom(Point currentPosition)
    {
      double num = Math.Exp((currentPosition.Y - this._previousPosition2D.Y) / 100.0);
      this._scale.ScaleX *= num;
      this._scale.ScaleY *= num;
      this._scale.ScaleZ *= num;
    }
  }
}
