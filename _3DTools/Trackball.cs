// Decompiled with JetBrains decompiler
// Type: _3DTools.Trackball
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace _3DTools
{
  public class Trackball
  {
    private FrameworkElement _eventSource;
    private Point _previousPosition2D;
    private Vector3D _previousPosition3D = new Vector3D(0.0, 0.0, 1.0);
    private Transform3DGroup _transform;
    private ScaleTransform3D _scale = new ScaleTransform3D();
    private AxisAngleRotation3D _rotation = new AxisAngleRotation3D();

    public Trackball()
    {
      this._transform = new Transform3DGroup();
      this._transform.Children.Add((Transform3D) this._scale);
      this._transform.Children.Add((Transform3D) new RotateTransform3D((Rotation3D) this._rotation));
    }

    public Transform3D Transform => (Transform3D) this._transform;

    public FrameworkElement EventSource
    {
      get => this._eventSource;
      set
      {
        if (this._eventSource != null)
        {
          this._eventSource.MouseDown -= new MouseButtonEventHandler(this.OnMouseDown);
          this._eventSource.MouseUp -= new MouseButtonEventHandler(this.OnMouseUp);
          this._eventSource.MouseMove -= new MouseEventHandler(this.OnMouseMove);
        }
        this._eventSource = value;
        this._eventSource.MouseDown += new MouseButtonEventHandler(this.OnMouseDown);
        this._eventSource.MouseUp += new MouseButtonEventHandler(this.OnMouseUp);
        this._eventSource.MouseMove += new MouseEventHandler(this.OnMouseMove);
      }
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
      Mouse.Capture((IInputElement) this.EventSource, CaptureMode.Element);
      this._previousPosition2D = e.GetPosition((IInputElement) this.EventSource);
      this._previousPosition3D = this.ProjectToTrackball(this.EventSource.ActualWidth, this.EventSource.ActualHeight, this._previousPosition2D);
    }

    private void OnMouseUp(object sender, MouseEventArgs e) => Mouse.Capture((IInputElement) this.EventSource, CaptureMode.None);

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      Point position = e.GetPosition((IInputElement) this.EventSource);
      if (e.LeftButton == MouseButtonState.Pressed)
        this.Track(position);
      else if (e.RightButton == MouseButtonState.Pressed)
        this.Zoom(position);
      this._previousPosition2D = position;
    }

    private void Track(Point currentPosition)
    {
      Vector3D trackball = this.ProjectToTrackball(this.EventSource.ActualWidth, this.EventSource.ActualHeight, currentPosition);
      Quaternion quaternion = new Quaternion(this._rotation.Axis, this._rotation.Angle) * new Quaternion(Vector3D.CrossProduct(this._previousPosition3D, trackball), -Vector3D.AngleBetween(this._previousPosition3D, trackball));
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
