// Decompiled with JetBrains decompiler
// Type: _3DTools.MeshTextureCoordinateConverter
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DTools
{
    public abstract class MeshTextureCoordinateConverter : MeshConverter<PointCollection>
    {
        public override object Convert(MeshGeometry3D mesh, object parameter)
        {
            string source = parameter as string;
            if (parameter != null && source == null)
                throw new ArgumentException("Parameter must be a string.");
            Vector3D yaxis = MathUtils.YAxis;
            if (source != null)
            {
                yaxis = Vector3D.Parse(source);
                MathUtils.TryNormalize(ref yaxis);
            }
            return this.Convert(mesh, yaxis);
        }

        public abstract object Convert(MeshGeometry3D mesh, Vector3D dir);
    }
}