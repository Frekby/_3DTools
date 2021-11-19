// Decompiled with JetBrains decompiler
// Type: _3DTools.SphericalTextureCoordinateGenerator
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System.Windows.Media.Media3D;

namespace _3DTools
{
    public class SphericalTextureCoordinateGenerator : MeshTextureCoordinateConverter
    {
        public override object Convert(MeshGeometry3D mesh, Vector3D dir) => (object) MeshUtils.GenerateSphericalTextureCoordinates(mesh, dir);
    }
}