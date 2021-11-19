// Decompiled with JetBrains decompiler
// Type: _3DTools.VisualDecorator
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;

namespace _3DTools
{
    internal class VisualDecorator : FrameworkElement
    {
        private Visual _visual;

        public VisualDecorator() => this._visual = (Visual) null;

        public Visual Content
        {
            get => this._visual;
            set
            {
                if (this._visual == value)
                    return;
                Visual visual = this._visual;
                Visual child = value;
                this.RemoveVisualChild(visual);
                this.RemoveLogicalChild((object) visual);
                this._visual = value;
                this.AddLogicalChild((object) child);
                this.AddVisualChild(child);
            }
        }

        protected override int VisualChildrenCount => this.Content == null ? 0 : 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0 && this.Content != null)
                return this._visual;
            throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Out of range visual requested");
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
    }
}