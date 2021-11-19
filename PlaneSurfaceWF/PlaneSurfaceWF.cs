using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Forms;


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
        for (int i = XY.Count+1; i < 2*XY.Count+1; i++)
        {
            
            meshGeometry3D.TriangleIndices.Add(2*XY.Count+1);
            meshGeometry3D.TriangleIndices.Add(i+1);
            meshGeometry3D.TriangleIndices.Add(i);
            
        }
        
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

public class plane
    {
        
        public int nomer;
        private List<TextBox> XYtoch_textbox = new List<TextBox>();
        public List<float> XYtoch = new List<float>();

        public plane()
        {
            
        }
        
        public void plane_creat(TableLayoutPanel table1)
        {
            var _table = new TableLayoutPanel() {AutoSize = true};
            var _text_name = new TextBox() {Text = "Поверхность" + nomer.ToString(), AutoSize = true};
            var _kol_toch = new TextBox() {Text = "3", AutoSize = true};
            var _table_XY = new TableLayoutPanel() {AutoSize = true};
            
            _table.Controls.Add(new Label(){Text = "Наименование", AutoSize = true},0,0);
            
            _table.Controls.Add(_text_name,0,1);
            
            _table.Controls.Add(new Label(){Text = "Количество точек", AutoSize = true},0,2);

            _kol_toch.TextChanged += delegate(object? o, EventArgs args) { kol_tochec(o, args, _table); };
            _kol_toch.KeyPress += VVod_int;
            _table.Controls.Add(_kol_toch,0,3);
            
            

            for (int i = 0; i < 3; i++)
            {
                _table_XY.Controls.Add(new Label(){Text = "X"+i.ToString(), AutoSize = true},0,2*i);
                _table_XY.Controls.Add(new Label(){Text = "Y"+i.ToString(), AutoSize = true},1,2*i);
                var X = new TextBox() {AutoSize = true};
                var Y = new TextBox() {AutoSize = true};
                _table_XY.Controls.Add(X,0,2*i+1);
                _table_XY.Controls.Add(Y,1,2*i+1);
                XYtoch_textbox.Add(X);
                XYtoch_textbox.Add(Y);
            }
            _table.Controls.Add(_table_XY,0,4);
                
            
            table1.Controls.Add(_table,0,nomer);

            //Console.WriteLine(nomer);
            
        }
        
        private void kol_tochec(object sender, EventArgs e, TableLayoutPanel tableadd)
        {
            TextBox current = (TextBox)sender;
            try 
            {
                TableLayoutPanel tableXY = new TableLayoutPanel() {AutoSize = true};
                XYtoch_textbox.Clear();
                if (Convert.ToInt32(current.Text)>=3)
                {
                    for (int i = 0; i < Convert.ToInt32(current.Text); i++)
                    {
                        tableXY.Controls.Add(new Label(){Text = "X"+i.ToString(), AutoSize = true},0,2*i);
                        tableXY.Controls.Add(new Label(){Text = "Y"+i.ToString(), AutoSize = true},1,2*i);
                        var X = new TextBox() {AutoSize = true};
                        var Y = new TextBox() {AutoSize = true};
                        tableXY.Controls.Add(X,0,2*i+1);
                        tableXY.Controls.Add(Y,1,2*i+1);
                        XYtoch_textbox.Add(X);
                        XYtoch_textbox.Add(Y);
                    }

                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        tableXY.Controls.Add(new Label(){Text = "X"+i.ToString(), AutoSize = true},0,2*i);
                        tableXY.Controls.Add(new Label(){Text = "Y"+i.ToString(), AutoSize = true},1,2*i);
                        var X = new TextBox() {AutoSize = true};
                        var Y = new TextBox() {AutoSize = true};
                        tableXY.Controls.Add(X,0,2*i+1);
                        tableXY.Controls.Add(Y,1,2*i+1);
                        XYtoch_textbox.Add(X);
                        XYtoch_textbox.Add(Y);
                    }
                }
                tableadd.Controls.Remove(tableadd.GetControlFromPosition(0, 4));
                tableadd.Controls.Add(tableXY,0,4);
                Console.WriteLine(Convert.ToInt32(current.Text));
            }
            catch
            {
                // ignored
            }
        }

        private void VVod_int(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
 
            if (!Char.IsDigit(number))
            {
                e.Handled = true;
            }
        }

        public List<Point3D> XY()
        {
            List<Point3D> _XY = new List<Point3D>();

            for (int i = 0; i < XYtoch_textbox.Count;)
            {
                float x = Convert.ToSingle(XYtoch_textbox[i].Text);
                float y = Convert.ToSingle(XYtoch_textbox[i + 1].Text);
                _XY.Add(new Point3D(x,y,0));
                i = (i + 2);
            }
            
            /*foreach (TextBox TB in XYtoch_textbox)
            {
                _XY.Add(Convert.ToSingle(TB.Text));
                Console.WriteLine(Convert.ToSingle(TB.Text));
            }*/

            return _XY;
        }
 
    }
}
