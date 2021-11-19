using System;
using _3DTools;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;
using System.Windows.Media.Media3D;

public class PlaneSurface
{
    public static void CreatePlaneSurface(List<Point3D> XY, Color surfaceColor, Viewport3D viewport)
    {
        MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
        for (int i = 0; i < 2; i++)
        {
            Point3D sered = new Point3D();
            foreach (Point3D toch in XY)
            {
                sered.X += toch.X;
                sered.Y += toch.Y;
                sered.Z += toch.Z;
                meshGeometry3D.Positions.Add(toch);
            }
            sered.X /= XY.Count;
            sered.Y /= XY.Count;
            sered.Z /= XY.Count;
            meshGeometry3D.Positions.Add(sered);
        }

        for (int i = 0; i < XY.Count; i++)
        {
            meshGeometry3D.TriangleIndices.Add(i);
            meshGeometry3D.TriangleIndices.Add(i+1);
            meshGeometry3D.TriangleIndices.Add(XY.Count);
        }
        meshGeometry3D.TriangleIndices.Add(XY.Count-1);
        meshGeometry3D.TriangleIndices.Add(0);
        meshGeometry3D.TriangleIndices.Add(XY.Count);
        for (int i = XY.Count+1; i < 2*XY.Count+1; i++)
        {
            meshGeometry3D.TriangleIndices.Add(2*XY.Count+1);
            meshGeometry3D.TriangleIndices.Add(i+1);
            meshGeometry3D.TriangleIndices.Add(i);
        }
        
        meshGeometry3D.TriangleIndices.Add(2*XY.Count+1);
        meshGeometry3D.TriangleIndices.Add(XY.Count+1);
        meshGeometry3D.TriangleIndices.Add(2*XY.Count);
        
        Material material = (Material) new DiffuseMaterial((Brush) new SolidColorBrush()
        {
            Color = surfaceColor
        });
    
        var material2 = new MaterialGroup();
        var diffuse = new DiffuseMaterial(new SolidColorBrush(surfaceColor));
        var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 1000);
        var emmMat = new EmissiveMaterial(new SolidColorBrush(Colors.Black));
    
        material2.Children.Add(diffuse);
        //material2.Children.Add(specMat);
        material2.Children.Add(emmMat);

        GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) meshGeometry3D, material2);
        viewport.Children.Add((Visual3D) new ModelVisual3D()
        {
            Content = (Model3D) geometryModel3D
        });
 
        
        
    }
    
        public static void CreateTorec(List<Point3D> XY, Color surfaceColor, Viewport3D viewport)
    {
        MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
        for (int i = 0; i < 2; i++)
        {
            Point3D sered = new Point3D();
            foreach (Point3D toch in XY)
            {
                meshGeometry3D.Positions.Add(toch);
            }
            sered.X = 0.0;
            sered.Y = 0.0;
            sered.Z = 0.0;
            meshGeometry3D.Positions.Add(sered);
        }

        for (int i = 0; i < XY.Count; i++)
        {
            meshGeometry3D.TriangleIndices.Add(i);
            meshGeometry3D.TriangleIndices.Add(i+1);
            meshGeometry3D.TriangleIndices.Add(XY.Count);
        }
        //meshGeometry3D.TriangleIndices.Add(XY.Count-1);
        //meshGeometry3D.TriangleIndices.Add(0);
        //meshGeometry3D.TriangleIndices.Add(XY.Count);
        for (int i = XY.Count+1; i < 2*XY.Count+1; i++)
        {
            meshGeometry3D.TriangleIndices.Add(2*XY.Count+1);
            meshGeometry3D.TriangleIndices.Add(i+1);
            meshGeometry3D.TriangleIndices.Add(i);
        }
        
        //meshGeometry3D.TriangleIndices.Add(2*XY.Count+1);
        //meshGeometry3D.TriangleIndices.Add(XY.Count+1);
        //meshGeometry3D.TriangleIndices.Add(2*XY.Count);
        
        Material material = (Material) new DiffuseMaterial((Brush) new SolidColorBrush()
        {
            Color = surfaceColor
        });
    
        var material2 = new MaterialGroup();
        var diffuse = new DiffuseMaterial(new SolidColorBrush(surfaceColor));
        var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 1000);
        var emmMat = new EmissiveMaterial(new SolidColorBrush(Colors.Black));
    
        material2.Children.Add(diffuse);
        //material2.Children.Add(specMat);
        material2.Children.Add(emmMat);

        GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) meshGeometry3D, material2);
        viewport.Children.Add((Visual3D) new ModelVisual3D()
        {
            Content = (Model3D) geometryModel3D
        });
 
        
        
    }
    
}
