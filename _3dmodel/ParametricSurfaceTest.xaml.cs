using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

namespace WpfApplication1
{
    public partial class ParametricSurfaceTest : Window
    {
        private ParametricSurface ps = new ParametricSurface();
        public ParametricSurfaceTest()
        {
            InitializeComponent();
            ps.IsHiddenLine = false;
            ps.IsWireframe = false;
            ps.Viewport3d = viewport;
            //Trackball trackball = new Trackball();
            //trackball.EventSource = background;
            //viewport.Camera.Transform = trackball.Transform;
            
            var trackball = new WpfApplication1.Trackball();
            trackball.EventSource = background;
            viewport.Camera.Transform = trackball.Transform;
            light.Transform = trackball.RotateTransform;
            
            //AddTorus();
            AddTorus1();

            Color cc = Color.FromRgb(234, 0, 0);
            Utility.CreateRectangleFace(new Point3D(0, 0, 0), new Point3D(2, 0, 0), new Point3D(2, 1, 0), new Point3D(0, 1, 0),
                cc, ps.Viewport3d);
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