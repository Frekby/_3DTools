// Decompiled with JetBrains decompiler
// Type: _3DTools.Viewport3DDecorator
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace _3DTools
{
  [ContentProperty("Content")]
  public abstract class Viewport3DDecorator : FrameworkElement, IAddChild
  {
    private UIElementCollection _preViewportChildren;
    private UIElementCollection _postViewportChildren;
    private UIElement _content;

    public Viewport3DDecorator()
    {
      this._preViewportChildren = new UIElementCollection((UIElement) this, (FrameworkElement) this);
      this._postViewportChildren = new UIElementCollection((UIElement) this, (FrameworkElement) this);
      this._content = (UIElement) null;
    }

    public UIElement Content
    {
      get => this._content;
      set
      {
        switch (value)
        {
          case Viewport3D _:
          case Viewport3DDecorator _:
            if (this._content == value)
              break;
            UIElement content = this._content;
            UIElement newContent = value;
            this.RemoveVisualChild((Visual) content);
            this.RemoveLogicalChild((object) content);
            this._content = value;
            this.AddLogicalChild((object) newContent);
            this.AddVisualChild((Visual) newContent);
            this.OnViewport3DDecoratorContentChange(content, newContent);
            this.BindToContentsWidthHeight(newContent);
            this.InvalidateMeasure();
            break;
          default:
            throw new ArgumentException("Not a valid child type", nameof (value));
        }
      }
    }

    private void BindToContentsWidthHeight(UIElement newContent)
    {
      Binding binding1 = new Binding("Width");
      binding1.Mode = BindingMode.OneWay;
      Binding binding2 = new Binding("Height");
      binding2.Mode = BindingMode.OneWay;
      binding1.Source = (object) newContent;
      binding2.Source = (object) newContent;
      BindingOperations.SetBinding((DependencyObject) this, FrameworkElement.WidthProperty, (BindingBase) binding1);
      BindingOperations.SetBinding((DependencyObject) this, FrameworkElement.HeightProperty, (BindingBase) binding2);
      Binding binding3 = new Binding("MaxWidth");
      binding3.Mode = BindingMode.OneWay;
      Binding binding4 = new Binding("MaxHeight");
      binding4.Mode = BindingMode.OneWay;
      binding3.Source = (object) newContent;
      binding4.Source = (object) newContent;
      BindingOperations.SetBinding((DependencyObject) this, FrameworkElement.MaxWidthProperty, (BindingBase) binding3);
      BindingOperations.SetBinding((DependencyObject) this, FrameworkElement.MaxHeightProperty, (BindingBase) binding4);
      Binding binding5 = new Binding("MinWidth");
      binding5.Mode = BindingMode.OneWay;
      Binding binding6 = new Binding("MinHeight");
      binding6.Mode = BindingMode.OneWay;
      binding5.Source = (object) newContent;
      binding6.Source = (object) newContent;
      BindingOperations.SetBinding((DependencyObject) this, FrameworkElement.MinWidthProperty, (BindingBase) binding5);
      BindingOperations.SetBinding((DependencyObject) this, FrameworkElement.MinHeightProperty, (BindingBase) binding6);
    }

    protected virtual void OnViewport3DDecoratorContentChange(
      UIElement oldContent,
      UIElement newContent)
    {
    }

    public Viewport3D Viewport3D
    {
      get
      {
        Viewport3D viewport3D = (Viewport3D) null;
        Viewport3DDecorator viewport3Ddecorator = this;
        UIElement content;
        while (true)
        {
          content = viewport3Ddecorator.Content;
          if (content != null)
          {
            if (!(content is Viewport3D))
              viewport3Ddecorator = (Viewport3DDecorator) content;
            else
              break;
          }
          else
            goto label_5;
        }
        viewport3D = (Viewport3D) content;
label_5:
        return viewport3D;
      }
    }

    protected UIElementCollection PreViewportChildren => this._preViewportChildren;

    protected UIElementCollection PostViewportChildren => this._postViewportChildren;

    protected override int VisualChildrenCount => this.PreViewportChildren.Count + this.PostViewportChildren.Count + (this.Content == null ? 0 : 1);

    protected override Visual GetVisualChild(int index)
    {
      int num = index;
      if (index < this.PreViewportChildren.Count)
        return (Visual) this.PreViewportChildren[index];
      index -= this.PreViewportChildren.Count;
      if (this.Content != null && index == 0)
        return (Visual) this.Content;
      index -= this.Content == null ? 0 : 1;
      if (index < this.PostViewportChildren.Count)
        return (Visual) this.PostViewportChildren[index];
      throw new ArgumentOutOfRangeException(nameof (index), (object) num, "Out of range visual requested");
    }

    protected override IEnumerator LogicalChildren
    {
      get
      {
        Visual[] visualArray = new Visual[this.VisualChildrenCount];
        for (int index = 0; index < this.VisualChildrenCount; ++index)
          visualArray[index] = this.GetVisualChild(index);
        return visualArray.GetEnumerator();
      }
    }

    protected override Size MeasureOverride(Size constraint)
    {
      Size size = new Size();
      this.MeasurePreViewportChildren(constraint);
      if (this.Content != null)
      {
        this.Content.Measure(constraint);
        size = this.Content.DesiredSize;
      }
      this.MeasurePostViewportChildren(constraint);
      return size;
    }

    protected virtual void MeasurePreViewportChildren(Size constraint) => this.MeasureUIElementCollection(this.PreViewportChildren, constraint);

    protected virtual void MeasurePostViewportChildren(Size constraint) => this.MeasureUIElementCollection(this.PostViewportChildren, constraint);

    private void MeasureUIElementCollection(UIElementCollection collection, Size constraint)
    {
      foreach (UIElement uiElement in collection)
        uiElement.Measure(constraint);
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      this.ArrangePreViewportChildren(arrangeSize);
      if (this.Content != null)
        this.Content.Arrange(new Rect(arrangeSize));
      this.ArrangePostViewportChildren(arrangeSize);
      return arrangeSize;
    }

    protected virtual void ArrangePreViewportChildren(Size arrangeSize) => this.ArrangeUIElementCollection(this.PreViewportChildren, arrangeSize);

    protected virtual void ArrangePostViewportChildren(Size arrangeSize) => this.ArrangeUIElementCollection(this.PostViewportChildren, arrangeSize);

    private void ArrangeUIElementCollection(UIElementCollection collection, Size constraint)
    {
      foreach (UIElement uiElement in collection)
        uiElement.Arrange(new Rect(constraint));
    }

    void IAddChild.AddChild(object value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.Content = this.Content == null ? (UIElement) value : throw new ArgumentException("Viewport3DDecorator can only have one child");
    }

    void IAddChild.AddText(string text)
    {
      for (int index = 0; index < text.Length; ++index)
      {
        if (!char.IsWhiteSpace(text[index]))
          throw new ArgumentException("Non whitespace in add text", text);
      }
    }
  }
}
