// Decompiled with JetBrains decompiler
// Type: _3DTools.MeshConverter`1
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace _3DTools
{
    public abstract class MeshConverter<TargetType> : IValueConverter
    {
        object IValueConverter.Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            if (targetType != typeof (TargetType))
                throw new ArgumentException(string.Format("MeshConverter must target a {0}", (object) typeof (TargetType).Name));
            return value is MeshGeometry3D mesh ? this.Convert(mesh, parameter) : throw new ArgumentException("MeshConverter can only convert from a MeshGeometry3D");
        }

        object IValueConverter.ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public abstract object Convert(MeshGeometry3D mesh, object parameter);
    }
}