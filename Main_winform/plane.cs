using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using WinFormsApp1;

namespace Main_winform
{
    
    
    
    public class plane
    {
        private Form_main mainf;
        
        public Form_main mainF
        {
            set => this.mainf = value;
        }
        
        public int nomer;
        public int tip=0;
        private List<TextBox> XYtoch_textbox = new List<TextBox>();
        private List<TextBox> TxTyTzalfabeta = new List<TextBox>();
        public List<float> XYtoch = new List<float>();
        

        public plane()
        {
            
        }
        
        public void plane_creat(TableLayoutPanel table1)
        {
            var _GB = new Panel() {AutoSize = true, };
            var _table_parametr = new TableLayoutPanel() {AutoSize = true, Location = new Point(0,0)};
            var _text_name = new TextBox() {Text = "Поверхность" + nomer.ToString(), AutoSize = true};
            var _kol_toch = new TextBox() {Text = "3", AutoSize = true};
            var _table_XY = new TableLayoutPanel() {AutoSize = true, Location = new Point(0,100)};
            var button_del = new Button() {AutoSize = true, Text = "Удалить"};
            
            var Tx = new TextBox() {AutoSize = true};
            var Ty = new TextBox() {AutoSize = true};
            var Tz = new TextBox() {AutoSize = true};
            var alfa = new TextBox() {AutoSize = true};
            var beta = new TextBox() {AutoSize = true};
            TxTyTzalfabeta.Add(Tx);
            TxTyTzalfabeta.Add(Ty);
            TxTyTzalfabeta.Add(Tz);
            TxTyTzalfabeta.Add(alfa);
            TxTyTzalfabeta.Add(beta);

            _table_parametr.Controls.Add(new Label(){Text = "Наименование", AutoSize = true},0,0);
            
            _table_parametr.Controls.Add(_text_name,0,1);

            button_del.Click += delate;
            _table_parametr.Controls.Add(button_del ,1,1);
            
            _table_parametr.Controls.Add(new Label(){Text = "Количество точек", AutoSize = true},0,2);
            _table_parametr.Controls.Add(new Label(){Text = "Tx", AutoSize = true},1,2);
            _table_parametr.Controls.Add(new Label(){Text = "Ty", AutoSize = true},2,2);
            _table_parametr.Controls.Add(new Label(){Text = "Tz", AutoSize = true},3,2);
            _table_parametr.Controls.Add(new Label(){Text = "Альфа", AutoSize = true},4,2);
            _table_parametr.Controls.Add(new Label(){Text = "Бета", AutoSize = true},5,2);
            
            _kol_toch.TextChanged += delegate(object? o, EventArgs args) { kol_tochec(o, args, _GB); };
            _kol_toch.KeyPress += VVod_int;
            _table_parametr.Controls.Add(_kol_toch,0,3);
            
            _table_parametr.Controls.Add(Tx,1,3);
            _table_parametr.Controls.Add(Ty,2,3);
            _table_parametr.Controls.Add(Tz,3,3);
            _table_parametr.Controls.Add(alfa,4,3);
            _table_parametr.Controls.Add(beta,5,3);

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
            //_table_parametr.Controls.Add(_table_XY,0,4);
            _GB.Controls.Add(_table_parametr);
            _GB.Controls.Add(_table_XY);
            
            //table1.Controls.Add(_table_parametr,0,nomer);
            table1.Controls.Add(_GB,0,nomer);
            //Console.WriteLine(nomer);
            
        }

        private void delate(object sender, EventArgs e)
        {
            Console.WriteLine(nomer);
            for (int i = nomer+1; i < mainf.list.Count; i++)
            {
                mainf.list[i].nomer -= 1;
                Console.WriteLine(mainf.list[i].nomer);
            }
            RemoveArbitraryRow(mainf._table, nomer);
            mainf.list.RemoveAt(nomer);
            mainf._i -= 1;
        }
        
        private void kol_tochec(object sender, EventArgs e, Panel GB)
        {
            TextBox current = (TextBox)sender;
            try 
            {
                TableLayoutPanel tableXY = new TableLayoutPanel() {AutoSize = true, Location = new Point(0,100)};
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

                GB.Controls.RemoveAt(1);
                GB.Controls.Add(tableXY);
                //Console.WriteLine(Convert.ToInt32(current.Text));
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
 
        public static void RemoveArbitraryRow(TableLayoutPanel panel, int rowIndex)
        {
            //Console.WriteLine(panel.RowCount);
            //Console.WriteLine(rowIndex);
            if (rowIndex >= panel.RowCount)
            {
                return;
            }

            // delete all controls of row that we want to delete
            for (int i = 0; i < panel.ColumnCount; i++)
            {
                var control = panel.GetControlFromPosition(i, rowIndex);
                panel.Controls.Remove(control);
            }

            // move up row controls that comes after row we want to remove
            for (int i = rowIndex + 1; i < panel.RowCount; i++)
            {
                for (int j = 0; j < panel.ColumnCount; j++)
                {
                    var control = panel.GetControlFromPosition(j, i);
                    if (control != null)
                    {
                        panel.SetRow(control, i - 1);
                    }
                }
            }

            var removeStyle = panel.RowCount - 1;

            if (panel.RowStyles.Count > removeStyle)
                panel.RowStyles.RemoveAt(removeStyle);

            panel.RowCount--;
        } 
        
    }
}