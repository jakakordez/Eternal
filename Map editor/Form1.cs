using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EGE;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Threading;

namespace Map_editor
{
    public partial class Form1 : Form
    {
        public static World currentWorld;
        bool OpenGLLoaded = false;
        string MapPath = "";
        public Form1()
        {
            InitializeComponent();
            currentWorld = new World(true);
        }

        public static object getWorldValue(string path)
        {
            string[] pathParts = path.Split('/');
            object v = currentWorld;
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (v.GetType().IsArray) v = ((Array)v).GetValue(Convert.ToInt32(pathParts[i]));
                else
                    v = v.GetType().GetProperty(pathParts[i]).GetValue(v);
            }
            return v;
        }
        #region Files
        public void LoadMap(string path)
        {
            toolStrip1.Enabled = true;
            MapPath = path;
            currentWorld.LoadData(path);
            objectBrowser1.LoadNodes(currentWorld.CurrentMap, "CurrentMap");
            currentWorld.Init();
            currentWorld.Build();
            hostedComponent1.UpdateWorld();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) LoadMap(folderBrowserDialog1.SelectedPath);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (MapPath != "") currentWorld.SaveData(MapPath);
            else {
                var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    MapPath = dialog.SelectedPath;
                    currentWorld.SaveData(MapPath);
                }
            }
            Cursor = Cursors.Default;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentWorld = new World(true);
            MapPath = "";
            objectBrowser1.LoadNodes(currentWorld.CurrentMap, "CurrentMap");
        }
        #endregion

        #region OpenGL

        private void glControl1_Load(object sender, EventArgs e)
        {
            OpenGLLoaded = true;
            currentWorld.Resize(glControl1.Width, glControl1.Height);
            tmrPreview.Start();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!OpenGLLoaded) return;
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!OpenGLLoaded) return;
            currentWorld.Resize(glControl1.Width, glControl1.Height);
            base.OnResize(e);
        }
        System.Diagnostics.Stopwatch fpsCounter = new System.Diagnostics.Stopwatch();
        private void tmpPreview_Tick(object sender, EventArgs e)
        {
            try
            {
                fpsCounter.Reset();
                fpsCounter.Start();
                currentWorld.Update(tabControl1.SelectedIndex == 1, 0);   
                currentWorld.Draw(tabControl1.SelectedIndex == 1);
                fpsCounter.Stop();
                hostedComponent1.Step = (float)Math.Pow(2, Controller.MouseScroll / 10f);
                lblStep.Text = string.Format("S:{0,7:0.00}", hostedComponent1.Step);
                if (fpsCounter.ElapsedMilliseconds == 0) lblFPS.Text = "∞ FPS";
                else lblFPS.Text = (1f / fpsCounter.ElapsedMilliseconds * 1000f) + " FPS";
            }
            catch { }

            glControl1.SwapBuffers();
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) tabControl1.SelectedIndex = 0;
            else hostedComponent1.ProcessInput(System.Windows.Input.KeyInterop.KeyFromVirtualKey((int)e.KeyCode));
        }

        private void glControl1_Enter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void glControl1_Leave(object sender, EventArgs e)
        {
            Cursor.Show();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                int h = (int)(System.Windows.SystemParameters.PrimaryScreenWidth / 2);
                int w = (int)(System.Windows.SystemParameters.PrimaryScreenHeight / 2);
                Cursor.Position = new Point(h, w);
                glControl1.Focus();
            }
        }
        #endregion

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            objectBrowser1.Realign();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            objectBrowser1.Realign();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

            tabPage2.Show(); // Switch to page 2 for OpenGL control load
            tabPage1.Show(); // Switch back to page 1
            hostedComponent1.UpdateLocation += MapView1_UpdateLocation;
            //hostedComponent1.MoveNode += MapView1_MoveNode;
            LoadMap(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"\\mapa");
        }

        /*private void MapView1_MoveNode(double X, double Y, double Z, double Step, object argument, string additionalData)
        {
            
        }*/

        private void MapView1_UpdateLocation(double X, double Y, double? Z, object arg, string additionalData)
        {
            string output;
            if(Z == null) output = string.Format("X:{0,7:0.00}  Y:{1,7:0.00}"+additionalData, X, Y);
            else output = string.Format("X:{0,7:0.00}  Y:{1,7:0.00}  Z:{2,7:0.00}" + additionalData, X, Y, Z);
            lblLocation.Text = output;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new ResourceCollector().Show();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.CurrentDirectory+"\\..\\..\\..\\Eternal\\bin\\Debug\\Eternal.exe");
        }

        private void objectBrowser1_NavigateNode(object sender, string e)
        {
            hostedComponent1.FocusNode(e);
        }

        private void objectBrowser1_UpdateWorld(object sender, EventArgs e)
        {
            hostedComponent1.UpdateWorld();
        }

        public void tsbPointer_Click(object sender, EventArgs e)
        {
            ToolStripButton clickedButton = (ToolStripButton)sender;
            tsbPointerNone.Checked = (int)clickedButton.Tag == 1;
            tsbPointerMove.Checked = (int)clickedButton.Tag == 2;
            tsbPointerHeight.Checked = (int)clickedButton.Tag == 3;
            tsbPointerRotateX.Checked = (int)clickedButton.Tag == 4;
            tsbPointerRotateY.Checked = (int)clickedButton.Tag == 5;
            tsbPointerRotateZ.Checked = (int)clickedButton.Tag == 6;
            tsbPointerDelete.Checked = (int)clickedButton.Tag == 7;
            hostedComponent1.CurrentFunction = (mapView.PointerFunction)clickedButton.Tag;
        }

        private Vector3 objectBrowser1_PickLocation()
        {
            return hostedComponent1.PickLocation();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            currentWorld.Exit();
        }
    }
}
