using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3dWPFlib
{
    public partial class ParametricSurfaceTest : Window
    {
        private ParametricSurface ps = new ParametricSurface();
        public ParametricSurfaceTest(List<dynamic> pover)
        {
            InitializeComponent();
            ps.IsHiddenLine = false;
            ps.IsWireframe = false;
            ps.Viewport3d = viewport;
            var trackball = new Trackball();
            trackball.EventSource = background;
            viewport.Camera.Transform = trackball.Transform;
            light.Transform = trackball.RotateTransform;
            Utility.XYZcoord(viewport);

            //AddTorus();
            //AddTorus1();

            Color cc = Color.FromRgb(234, 0, 0);
            for (int i = 0; i < pover.Count; i++)
            {
                if (pover[i].tip==0)
                {
                    PlaneSurface.CreatePlaneSurface(pover[i].XY(),cc,ps.Viewport3d);                    
                }
                if (pover[i].tip==1)
                {
                    PlaneSurface.CreateTorec(pover[i].XY(),cc,ps.Viewport3d);                    
                }
            }
            //Utility.CreateRectangleFace(new Point3D(0, 0, 0), new Point3D(2, 0, 0), new Point3D(2, 1, 0), new Point3D(0, 1, 0),
                //cc, ps.Viewport3d);
        }
        private void AddTorus()
        {
            ps.Umin = 0;
            ps.Umax = 1;
            ps.Vmin = 0;
            ps.Vmax = 2 * Math.PI;
            ps.Nu = 20;
            ps.Nv = 20;
            ps.CreateSurface(Torus);
            Color cc = Color.FromRgb(1, 0, 0);
            ps.SurfaceColor = cc;
        }
        
        private void AddTorus1()
        {
            ps.Umin = 0;
            ps.Umax = 1;
            ps.Vmin = 0;
            ps.Vmax = 2 * Math.PI;
            ps.Nu = 20;
            ps.Nv = 20;
            ps.CreateSurface(Torus);
        }
        private Point3D Torus(double u, double v)
        {
            double x = Math.Sqrt(1-u*u)* Math.Cos(v);
            double z = u;
            double y = Math.Sqrt(1-u*u)* Math.Sin(v);
            return new Point3D(x, y, z);
        }
        
        
    }
}