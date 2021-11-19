// Decompiled with JetBrains decompiler
// Type: _3DTools.Interactive3DDecorator
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DTools
{
  public class Interactive3DDecorator : Viewport3DDecorator
  {
    private const double BUFFER_SIZE = 2.0;
    public static readonly DependencyProperty DebugProperty = DependencyProperty.Register(nameof (Debug), typeof (bool), typeof (Interactive3DDecorator), new PropertyMetadata((object) false, new PropertyChangedCallback(Interactive3DDecorator.OnDebugPropertyChanged)));
    public static readonly DependencyProperty ContainsInkProperty = DependencyProperty.Register(nameof (ContainsInk), typeof (bool), typeof (Interactive3DDecorator), new PropertyMetadata((object) false));
    private Decorator _hiddenVisual;
    private Decorator _oldHiddenVisual;
    private Decorator _oldKeyboardFocusVisual;
    private TranslateTransform _hiddenVisTranslate;
    private ScaleTransform _hiddenVisScale;
    private TransformGroup _hiddenVisTransform;
    private bool _mouseCaptureInHiddenVisual;
    private double _offsetX;
    private double _offsetY;
    private double _scale;
    private Interactive3DDecorator.ClosestIntersectionInfo _closestIntersectInfo;
    private Interactive3DDecorator.ClosestIntersectionInfo _lastValidClosestIntersectInfo;
    private Interactive3DDecorator.DebugEdgesAdorner _DEBUGadorner;
    private bool _isInPosition;

    public Interactive3DDecorator()
    {
      this.ClipToBounds = true;
      this._offsetX = this._offsetY = 0.0;
      this._scale = 1.0;
      this._hiddenVisTranslate = new TranslateTransform(this._offsetX, this._offsetY);
      this._hiddenVisScale = new ScaleTransform(this._scale, this._scale);
      this._hiddenVisTransform = new TransformGroup();
      this._hiddenVisTransform.Children.Add((Transform) this._hiddenVisScale);
      this._hiddenVisTransform.Children.Add((Transform) this._hiddenVisTranslate);
      this._hiddenVisual = new Decorator();
      this._hiddenVisual.Opacity = 0.0;
      this._hiddenVisual.RenderTransform = (Transform) this._hiddenVisTransform;
      this._oldHiddenVisual = new Decorator();
      this._oldHiddenVisual.Opacity = 0.0;
      this._oldKeyboardFocusVisual = new Decorator();
      this._oldKeyboardFocusVisual.Opacity = 0.0;
      this.PostViewportChildren.Add((UIElement) this._oldHiddenVisual);
      this.PostViewportChildren.Add((UIElement) this._oldKeyboardFocusVisual);
      this.PostViewportChildren.Add((UIElement) this._hiddenVisual);
      this._closestIntersectInfo = (Interactive3DDecorator.ClosestIntersectionInfo) null;
      this._lastValidClosestIntersectInfo = (Interactive3DDecorator.ClosestIntersectionInfo) null;
      this.AllowDrop = true;
    }

    protected override void MeasurePostViewportChildren(Size constraint)
    {
      Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
      foreach (UIElement postViewportChild in this.PostViewportChildren)
        postViewportChild.Measure(availableSize);
    }

    protected override void ArrangePostViewportChildren(Size arrangeSize)
    {
      foreach (UIElement postViewportChild in this.PostViewportChildren)
        postViewportChild.Arrange(new Rect(postViewportChild.DesiredSize));
    }

    protected override void OnPreviewDragOver(DragEventArgs e)
    {
      base.OnPreviewDragOver(e);
      if (this.Viewport3D == null)
        return;
      this.ArrangeHiddenVisual(e.GetPosition((IInputElement) this.Viewport3D), true);
    }

    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      e.Effects = DragDropEffects.None;
      e.Handled = true;
    }

    protected override void OnDragEnter(DragEventArgs e)
    {
      base.OnDragEnter(e);
      e.Effects = DragDropEffects.None;
      e.Handled = true;
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      base.OnPreviewMouseMove(e);
      if (this._isInPosition)
      {
        this._isInPosition = false;
      }
      else
      {
        if (this.Viewport3D == null || !this.ArrangeHiddenVisual(e.GetPosition((IInputElement) this.Viewport3D), false))
          return;
        e.Handled = true;
        this._isInPosition = true;
        if (this.ContainsInk)
          this.InvalidateArrange();
        Mouse.Synchronize();
      }
    }

    private bool ArrangeHiddenVisual(Point mouseposition, bool scaleHiddenVisual)
    {
      bool flag = false;
      Viewport3D viewport3D = this.Viewport3D;
      if (viewport3D != null)
      {
        PointHitTestParameters hitTestParameters = new PointHitTestParameters(mouseposition);
        this._closestIntersectInfo = (Interactive3DDecorator.ClosestIntersectionInfo) null;
        this._mouseCaptureInHiddenVisual = this._hiddenVisual.IsMouseCaptureWithin;
        VisualTreeHelper.HitTest((Visual) viewport3D, new HitTestFilterCallback(this.InteractiveMV3DFilter), new HitTestResultCallback(this.HTResult), (HitTestParameters) hitTestParameters);
        if (this._closestIntersectInfo == null && this._mouseCaptureInHiddenVisual && this._lastValidClosestIntersectInfo != null)
          this.HandleMouseCaptureButOffMesh(this._lastValidClosestIntersectInfo.InteractiveModelVisual3DHit, mouseposition);
        else if (this._closestIntersectInfo != null)
          this._lastValidClosestIntersectInfo = this._closestIntersectInfo;
        flag = this.UpdateHiddenVisual(this._closestIntersectInfo, mouseposition, scaleHiddenVisual);
      }
      return flag;
    }

    private void HandleMouseCaptureButOffMesh(InteractiveVisual3D imv3DHit, Point mousePos)
    {
      UIElement captured = (UIElement) Mouse.Captured;
      Rect descendantBounds = VisualTreeHelper.GetDescendantBounds((Visual) captured);
      GeneralTransform ancestor = captured.TransformToAncestor((Visual) this._hiddenVisual);
      Point[] pointArray = new Point[4]
      {
        ancestor.Transform(new Point(descendantBounds.Left, descendantBounds.Top)),
        ancestor.Transform(new Point(descendantBounds.Right, descendantBounds.Top)),
        ancestor.Transform(new Point(descendantBounds.Right, descendantBounds.Bottom)),
        ancestor.Transform(new Point(descendantBounds.Left, descendantBounds.Bottom))
      };
      Point[] texCoordsOfInterest = new Point[4];
      for (int index = 0; index < pointArray.Length; ++index)
        texCoordsOfInterest[index] = Interactive3DDecorator.VisualCoordsToTextureCoords(pointArray[index], (UIElement) this._hiddenVisual);
      List<HitTestEdge> visualEdges = imv3DHit.GetVisualEdges(texCoordsOfInterest);
      if (this.Debug)
      {
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual) this);
        if (this._DEBUGadorner == null)
        {
          this._DEBUGadorner = new Interactive3DDecorator.DebugEdgesAdorner((UIElement) this, visualEdges);
          adornerLayer.Add((Adorner) this._DEBUGadorner);
        }
        else
        {
          adornerLayer.Remove((Adorner) this._DEBUGadorner);
          this._DEBUGadorner = new Interactive3DDecorator.DebugEdgesAdorner((UIElement) this, visualEdges);
          adornerLayer.Add((Adorner) this._DEBUGadorner);
        }
      }
      this.FindClosestIntersection(mousePos, visualEdges, imv3DHit);
    }

    private void FindClosestIntersection(
      Point mousePos,
      List<HitTestEdge> edges,
      InteractiveVisual3D imv3DHit)
    {
      double num1 = double.MaxValue;
      Point uv = new Point();
      for (int index = 0; index < edges.Count; ++index)
      {
        Vector vector1 = mousePos - edges[index]._p1Transformed;
        Vector vector2 = edges[index]._p2Transformed - edges[index]._p1Transformed;
        double d = vector2 * vector2;
        Point point;
        double length;
        if (d == 0.0)
        {
          point = edges[index]._p1Transformed;
          length = vector1.Length;
        }
        else
        {
          double num2 = vector2 * vector1;
          point = num2 >= 0.0 ? (num2 <= d ? edges[index]._p1Transformed + num2 / d * vector2 : edges[index]._p2Transformed) : edges[index]._p1Transformed;
          length = (mousePos - point).Length;
        }
        if (length < num1)
        {
          num1 = length;
          uv = d == 0.0 ? edges[index]._uv1 : (point - edges[index]._p1Transformed).Length / Math.Sqrt(d) * (edges[index]._uv2 - edges[index]._uv1) + edges[index]._uv1;
        }
      }
      if (num1 == double.MaxValue)
        return;
      UIElement captured = (UIElement) Mouse.Captured;
      UIElement internalVisual = imv3DHit.InternalVisual;
      Rect descendantBounds = VisualTreeHelper.GetDescendantBounds((Visual) captured);
      Point visualCoords = Interactive3DDecorator.TextureCoordsToVisualCoords(uv, internalVisual);
      Point point1 = internalVisual.TransformToDescendant((Visual) captured).Transform(visualCoords);
      if (point1.X <= descendantBounds.Left + 1.0)
        point1.X -= 2.0;
      if (point1.Y <= descendantBounds.Top + 1.0)
        point1.Y -= 2.0;
      if (point1.X >= descendantBounds.Right - 1.0)
        point1.X += 2.0;
      if (point1.Y >= descendantBounds.Bottom - 1.0)
        point1.Y += 2.0;
      this._closestIntersectInfo = new Interactive3DDecorator.ClosestIntersectionInfo(Interactive3DDecorator.VisualCoordsToTextureCoords(captured.TransformToAncestor((Visual) internalVisual).Transform(point1), internalVisual), imv3DHit.InternalVisual, imv3DHit);
    }

    public HitTestFilterBehavior InteractiveMV3DFilter(DependencyObject o)
    {
      HitTestFilterBehavior testFilterBehavior = HitTestFilterBehavior.Continue;
      if (o is Visual3D && this._mouseCaptureInHiddenVisual)
      {
        if (o is InteractiveVisual3D)
        {
          if (((InteractiveVisual3D) o).InternalVisual != this._hiddenVisual.Child)
            testFilterBehavior = HitTestFilterBehavior.ContinueSkipSelf;
        }
        else
          testFilterBehavior = HitTestFilterBehavior.ContinueSkipSelf;
      }
      return testFilterBehavior;
    }

    private bool UpdateHiddenVisual(
      Interactive3DDecorator.ClosestIntersectionInfo isectInfo,
      Point mousePos,
      bool scaleHiddenVisual)
    {
      bool flag = false;
      double newOffsetX;
      double newOffsetY;
      if (isectInfo != null)
      {
        UIElement uiElementHit = isectInfo.UIElementHit;
        if (this._hiddenVisual.Child != uiElementHit)
        {
          UIElement child = this._hiddenVisual.Child;
          if (this._oldHiddenVisual.Child == uiElementHit)
            this._oldHiddenVisual.Child = (UIElement) null;
          if (this._oldKeyboardFocusVisual.Child == uiElementHit)
            this._oldKeyboardFocusVisual.Child = (UIElement) null;
          if (this._oldHiddenVisual.Child == child)
            this._oldHiddenVisual.Child = (UIElement) null;
          if (this._oldKeyboardFocusVisual.Child == child)
            this._oldKeyboardFocusVisual.Child = (UIElement) null;
          Decorator decorator = child == null || !child.IsKeyboardFocusWithin ? this._oldHiddenVisual : this._oldKeyboardFocusVisual;
          this._hiddenVisual.Child = uiElementHit;
          decorator.Child = child;
          flag = true;
        }
        Point visualCoords = Interactive3DDecorator.TextureCoordsToVisualCoords(isectInfo.PointHit, (UIElement) this._hiddenVisual);
        newOffsetX = mousePos.X - visualCoords.X;
        newOffsetY = mousePos.Y - visualCoords.Y;
      }
      else
      {
        newOffsetX = this.ActualWidth + 1.0;
        newOffsetY = this.ActualHeight + 1.0;
      }
      double newScale = !scaleHiddenVisual ? 1.0 : Math.Max(this.Viewport3D.RenderSize.Width, this.Viewport3D.RenderSize.Height);
      return flag | this.PositionHiddenVisual(newOffsetX, newOffsetY, newScale, mousePos);
    }

    private bool PositionHiddenVisual(
      double newOffsetX,
      double newOffsetY,
      double newScale,
      Point mousePosition)
    {
      bool flag = false;
      if (newOffsetX != this._offsetX || newOffsetY != this._offsetY || this._scale != newScale)
      {
        this._offsetX = newOffsetX;
        this._offsetY = newOffsetY;
        this._scale = newScale;
        this._hiddenVisTranslate.X = this._scale * (this._offsetX - mousePosition.X) + mousePosition.X;
        this._hiddenVisTranslate.Y = this._scale * (this._offsetY - mousePosition.Y) + mousePosition.Y;
        this._hiddenVisScale.ScaleX = this._scale;
        this._hiddenVisScale.ScaleY = this._scale;
        flag = true;
      }
      return flag;
    }

    private static Point TextureCoordsToVisualCoords(Point uv, UIElement uiElem)
    {
      Rect descendantBounds = VisualTreeHelper.GetDescendantBounds((Visual) uiElem);
      return new Point(uv.X * descendantBounds.Width + descendantBounds.Left, uv.Y * descendantBounds.Height + descendantBounds.Top);
    }

    private static Point VisualCoordsToTextureCoords(Point pt, UIElement uiElem)
    {
      Rect descendantBounds = VisualTreeHelper.GetDescendantBounds((Visual) uiElem);
      return new Point((pt.X - descendantBounds.Left) / (descendantBounds.Right - descendantBounds.Left), (pt.Y - descendantBounds.Top) / (descendantBounds.Bottom - descendantBounds.Top));
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo info)
    {
      base.OnRenderSizeChanged(info);
      TranslateTransform translateTransform = new TranslateTransform(info.NewSize.Width + 1.0, 0.0);
      this._oldKeyboardFocusVisual.RenderTransform = (Transform) translateTransform;
      this._oldHiddenVisual.RenderTransform = (Transform) translateTransform;
    }

    private HitTestResultBehavior HTResult(HitTestResult rawresult)
    {
      RayHitTestResult rayHitResult = rawresult as RayHitTestResult;
      HitTestResultBehavior testResultBehavior = HitTestResultBehavior.Continue;
      if (rayHitResult != null)
      {
        this._closestIntersectInfo = this.GetIntersectionInfo(rayHitResult);
        testResultBehavior = HitTestResultBehavior.Stop;
      }
      return testResultBehavior;
    }

    private Interactive3DDecorator.ClosestIntersectionInfo GetIntersectionInfo(
      RayHitTestResult rayHitResult)
    {
      Interactive3DDecorator.ClosestIntersectionInfo intersectionInfo = (Interactive3DDecorator.ClosestIntersectionInfo) null;
      if (rayHitResult is RayMeshGeometry3DHitTestResult geometry3DhitTestResult && geometry3DhitTestResult.VisualHit is InteractiveVisual3D visualHit)
      {
        MeshGeometry3D meshHit = geometry3DhitTestResult.MeshHit;
        UIElement internalVisual = visualHit.InternalVisual;
        if (internalVisual != null)
        {
          double vertexWeight1 = geometry3DhitTestResult.VertexWeight1;
          double vertexWeight2 = geometry3DhitTestResult.VertexWeight2;
          double vertexWeight3 = geometry3DhitTestResult.VertexWeight3;
          int vertexIndex1 = geometry3DhitTestResult.VertexIndex1;
          int vertexIndex2 = geometry3DhitTestResult.VertexIndex2;
          int vertexIndex3 = geometry3DhitTestResult.VertexIndex3;
          if (meshHit.TextureCoordinates != null && vertexIndex1 < meshHit.TextureCoordinates.Count && vertexIndex2 < meshHit.TextureCoordinates.Count && vertexIndex3 < meshHit.TextureCoordinates.Count)
          {
            Point textureCoordinate1 = meshHit.TextureCoordinates[vertexIndex1];
            Point textureCoordinate2 = meshHit.TextureCoordinates[vertexIndex2];
            Point textureCoordinate3 = meshHit.TextureCoordinates[vertexIndex3];
            intersectionInfo = new Interactive3DDecorator.ClosestIntersectionInfo(new Point(textureCoordinate1.X * vertexWeight1 + textureCoordinate2.X * vertexWeight2 + textureCoordinate3.X * vertexWeight3, textureCoordinate1.Y * vertexWeight1 + textureCoordinate2.Y * vertexWeight2 + textureCoordinate3.Y * vertexWeight3), internalVisual, visualHit);
          }
        }
      }
      return intersectionInfo;
    }

    public bool Debug
    {
      get => (bool) this.GetValue(Interactive3DDecorator.DebugProperty);
      set => this.SetValue(Interactive3DDecorator.DebugProperty, (object) value);
    }

    internal static void OnDebugPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      Interactive3DDecorator interactive3Ddecorator = (Interactive3DDecorator) sender;
      if ((bool) e.NewValue)
      {
        interactive3Ddecorator._hiddenVisual.Opacity = 0.2;
      }
      else
      {
        interactive3Ddecorator._hiddenVisual.Opacity = 0.0;
        if (interactive3Ddecorator._DEBUGadorner == null)
          return;
        AdornerLayer.GetAdornerLayer((Visual) interactive3Ddecorator).Remove((Adorner) interactive3Ddecorator._DEBUGadorner);
        interactive3Ddecorator._DEBUGadorner = (Interactive3DDecorator.DebugEdgesAdorner) null;
      }
    }

    public bool ContainsInk
    {
      get => (bool) this.GetValue(Interactive3DDecorator.ContainsInkProperty);
      set => this.SetValue(Interactive3DDecorator.ContainsInkProperty, (object) value);
    }

    public class DebugEdgesAdorner : Adorner
    {
      private List<HitTestEdge> _edges;

      public DebugEdgesAdorner(UIElement adornedElement, List<HitTestEdge> edges)
        : base(adornedElement)
      {
        this._edges = edges;
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
        Pen pen = new Pen((Brush) new SolidColorBrush(Colors.Navy), 1.5);
        for (int index = 0; index < this._edges.Count; ++index)
          drawingContext.DrawLine(pen, this._edges[index]._p1Transformed, this._edges[index]._p2Transformed);
      }
    }

    private class ClosestIntersectionInfo
    {
      private Point _pointHit;
      private UIElement _uiElemHit;
      private InteractiveVisual3D _imv3DHit;

      public ClosestIntersectionInfo(Point p, UIElement v, InteractiveVisual3D iv3D)
      {
        this._pointHit = p;
        this._uiElemHit = v;
        this._imv3DHit = iv3D;
      }

      public Point PointHit
      {
        get => this._pointHit;
        set => this._pointHit = value;
      }

      public UIElement UIElementHit
      {
        get => this._uiElemHit;
        set => this._uiElemHit = value;
      }

      public InteractiveVisual3D InteractiveModelVisual3DHit
      {
        get => this._imv3DHit;
        set => this._imv3DHit = value;
      }
    }
  }
}
