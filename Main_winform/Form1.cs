#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;  
using static Main_winform.plane;
using System.Runtime.InteropServices;
using _3dWPFlib;
using Main_winform;
using ParametricSurfaceTest = _3dWPFlib.ParametricSurfaceTest;


namespace WinFormsApp1
{
    
    public partial class Form_main : Form
    {



        // для вывода в консоль
        [DllImport( "kernel32.dll" )]
        private static extern bool AttachConsole( int dwProcessId );
        private const int AttachParentProcess = -1;
        //
        
        private static Form_main _main_form = new Form_main();
        
        private Panel _panel;
        private Panel _panel1;
        public TableLayoutPanel _table;
        public int _i=0;
        public List<dynamic> list = new List<dynamic>();
        
        //private plane

        private Form_main()
        {
            InitializeComponent();
            tabc();
            //panel_create();
            //panel1_create();
            //list.Add(new List<dynamic>(){1,4,5});

        }

        private void tabc()
        {
            TabControl _tabControl = new TabControl() {AutoSize = true, Dock = DockStyle.Fill};
            TabPage newTabPage_geom = new TabPage(){AutoSize = true, Text = "Геометрия"};
            TabPage newTabPage_resh = new TabPage(){AutoSize = true, Text = "Решение"};
            _tabControl.TabPages.Add(newTabPage_geom);
            _tabControl.TabPages.Add(newTabPage_resh);

            panel_create(_tabControl);
            panel1_create(_tabControl);
            Controls.Add(_tabControl);
        }
        
        private void panel_create(TabControl tabControl)
        {
            _panel = new Panel();
            _panel.Width = 882;
            _panel.Height = 400;
            _panel.AutoScroll = true;
            _panel.BorderStyle = BorderStyle.Fixed3D;
            tabControl.TabPages[0].Controls.Add(_panel);
            
            
            _table = new TableLayoutPanel();
            _table.AutoSize = true;
            _table.ColumnCount = 1;
            _table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            _panel.Controls.Add(_table);
            _table.Visible = false;


        }

        private void panel1_create(TabControl tabControl)
        {
            _panel1 = new Panel();
            _panel1.Width = 882;
            _panel1.Height = 50;
            _panel1.Location = new Point(0, 400);
            _panel1.BorderStyle = BorderStyle.Fixed3D;
            tabControl.TabPages[0].Controls.Add(_panel1);

            Button _button2 = new Button(){Text = "Добавить n-угольник", AutoSize = true};
            _button2.Location=new Point(0, 0);
            _button2.Click += create_nugol;
            _panel1.Controls.Add(_button2);
            
            Button _button_torec = new Button(){Text = "Добавить торец", AutoSize = true};
            _button_torec.Location=new Point(_button2.Width, 0);
            _button_torec.Click += create_torec;
            _panel1.Controls.Add(_button_torec);
            
            Button _button_surf = new Button(){Text = "Добавить поверхность", AutoSize = true};
            _button_surf.Location=new Point(_button2.Width+_button_torec.Width, 0);
            _button_surf.Click += create_surf;
            _panel1.Controls.Add(_button_surf);
            
            Button _button3 = new Button(){Text = "Просмотр", AutoSize = true};
            _button3.Location=new Point(_button2.Width+_button_torec.Width+_button_surf.Width, 0);
            _button3.Click += smotr;
            _panel1.Controls.Add(_button3);
        }
        
        
        private void smotr(object? sender, EventArgs e)
        {
            var wpfwindow = new ParametricSurfaceTest(list);
            ElementHost.EnableModelessKeyboardInterop(wpfwindow); 
            wpfwindow.Show();
        }
        
        private void create_nugol(object? sender, EventArgs e)
        {
            if (_table.Visible == false)
            {
                _table.Visible = true;
            }
            plane _plane = new plane();
            _plane.nomer = _i;
            _plane.mainF = _main_form;
            _plane.plane_creat(_table);
            _table.RowCount += 1;
            
            list.Add(_plane);
            _i += 1;
            //Console.WriteLine(list[0][0]);
            //foreach (var item in list)
            //{
                //Console.WriteLine(item.GetType());
            //}
        }
        
        private void create_torec(object? sender, EventArgs e)
        {
            if (_table.Visible == false)
            {
                _table.Visible = true;
            }
            torec _torec = new torec();
            _torec.nomer = _i;
            _torec.mainF = _main_form;
            _torec.torec_creat(_table);
            _table.RowCount += 1;
            
            list.Add(_torec);
            _i += 1;
            //Console.WriteLine(list[0][0]);
            //foreach (var item in list)
            //{
            //Console.WriteLine(item.GetType());
            //}
        }
        
        private void create_surf(object? sender, EventArgs e)
        {
            if (_table.Visible == false)
            {
                _table.Visible = true;
            }
            surface _surface = new surface();
            _surface.nomer = _i;
            _surface.mainF = _main_form;
            _surface.torec_creat(_table);
            _table.RowCount += 1;
            
            list.Add(_surface);
            _i += 1;
            //Console.WriteLine(list[0][0]);
            //foreach (var item in list)
            //{
            //Console.WriteLine(item.GetType());
            //}
        }
        
        [STAThread]
        static void Main()
        {
            //для вывода в консоль
            AttachConsole( AttachParentProcess );
            //
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(_main_form);
            //_form1.AutoScroll = true;
        }
        
        
    }
}