using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using WinFormsApp1;

namespace Main_winform
{
    public class surface
    {
        private Form_main mainf;
        
        public Form_main mainF
        {
            set => this.mainf = value;
        }
        
        public int nomer;
        public int tip=2;
        private List<TextBox> a11a22a33a0z1z2f1f2_textbox = new List<TextBox>();
        private List<TextBox> TxTyTzalfabeta = new List<TextBox>();
        public List<float> a11a22a33a0z1z2f1f2 = new List<float>();
        
        public surface()
        {
            
        }
        
        public void torec_creat(TableLayoutPanel table1)
        {
            var _GB = new Panel() {AutoSize = true, };
            var _table_parametr = new TableLayoutPanel() {AutoSize = true, Location = new Point(0,0)};
            var _text_name = new TextBox() {Text = "Поверхность" + nomer.ToString(), AutoSize = true};
            var button_del = new Button() {AutoSize = true, Text = "Удалить"};

            var _table_a1a2a3z0z1f1f2 = new TableLayoutPanel() {AutoSize = true, Location = new Point(0, 100)};
            
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
            
            var a11 = new TextBox() {AutoSize = true};
            var a22 = new TextBox() {AutoSize = true};
            var a33 = new TextBox() {AutoSize = true};
            var a0 = new TextBox() {AutoSize = true};
            var z1 = new TextBox() {AutoSize = true};
            var z2 = new TextBox() {AutoSize = true};
            var f1 = new TextBox() {AutoSize = true};
            var f2 = new TextBox() {AutoSize = true};
            a11a22a33a0z1z2f1f2_textbox.Add(a11);
            a11a22a33a0z1z2f1f2_textbox.Add(a22);
            a11a22a33a0z1z2f1f2_textbox.Add(a33);
            a11a22a33a0z1z2f1f2_textbox.Add(a0);
            a11a22a33a0z1z2f1f2_textbox.Add(z1);
            a11a22a33a0z1z2f1f2_textbox.Add(z2);
            a11a22a33a0z1z2f1f2_textbox.Add(f1);
            a11a22a33a0z1z2f1f2_textbox.Add(f2);

            _table_parametr.Controls.Add(new Label(){Text = "Наименование", AutoSize = true},0,0);
            
            _table_parametr.Controls.Add(_text_name,0,1);

            button_del.Click += delate;
            _table_parametr.Controls.Add(button_del ,1,1);
            
            _table_parametr.Controls.Add(new Label(){Text = "Tx", AutoSize = true},0,2);
            _table_parametr.Controls.Add(new Label(){Text = "Ty", AutoSize = true},1,2);
            _table_parametr.Controls.Add(new Label(){Text = "Tz", AutoSize = true},2,2);
            _table_parametr.Controls.Add(new Label(){Text = "Альфа", AutoSize = true},3,2);
            _table_parametr.Controls.Add(new Label(){Text = "Бета", AutoSize = true},4,2);
            
            _table_parametr.Controls.Add(Tx,0,3);
            _table_parametr.Controls.Add(Ty,1,3);
            _table_parametr.Controls.Add(Tz,2,3);
            _table_parametr.Controls.Add(alfa,3,3);
            _table_parametr.Controls.Add(beta,4,3);
            
            _table_a1a2a3z0z1f1f2.Controls.Add(new Label(){Text = "a11", AutoSize = true},0,0);
            _table_a1a2a3z0z1f1f2.Controls.Add(new Label(){Text = "a22", AutoSize = true},1,0);
            _table_a1a2a3z0z1f1f2.Controls.Add(new Label(){Text = "a33", AutoSize = true},2,0);
            _table_a1a2a3z0z1f1f2.Controls.Add(new Label(){Text = "a0", AutoSize = true},3,0);
            _table_a1a2a3z0z1f1f2.Controls.Add(new Label(){Text = "z1", AutoSize = true},4,0);
            _table_a1a2a3z0z1f1f2.Controls.Add(new Label(){Text = "z2", AutoSize = true},5,0);
            _table_a1a2a3z0z1f1f2.Controls.Add(new Label(){Text = "fi1", AutoSize = true},6,0);
            _table_a1a2a3z0z1f1f2.Controls.Add(new Label(){Text = "fi2", AutoSize = true},7,0);
            
            _table_a1a2a3z0z1f1f2.Controls.Add(a11,0,1);
            _table_a1a2a3z0z1f1f2.Controls.Add(a22,1,1);
            _table_a1a2a3z0z1f1f2.Controls.Add(a33,2,1);
            _table_a1a2a3z0z1f1f2.Controls.Add(a0,3,1);
            _table_a1a2a3z0z1f1f2.Controls.Add(z1,4,1);
            _table_a1a2a3z0z1f1f2.Controls.Add(z2,5,1);
            _table_a1a2a3z0z1f1f2.Controls.Add(f1,6,1);
            _table_a1a2a3z0z1f1f2.Controls.Add(f2,7,1);

            _GB.Controls.Add(_table_parametr);
            _GB.Controls.Add(_table_a1a2a3z0z1f1f2);
            table1.Controls.Add(_GB,0,nomer);
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