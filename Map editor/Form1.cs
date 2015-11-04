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
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Map_editor
{
    public partial class Form1 : Form
    {
        Dictionary<Type, int> dataTypes = new Dictionary<Type, int>();
        public static World currentWorld;
        bool OpenGLLoaded = false;
        string MapPath = "";
        public Form1()
        {
            InitializeComponent();
            dataTypes.Add(typeof(string), 1);
            dataTypes.Add(typeof(int), 0);
            dataTypes.Add(typeof(float), 0);
            dataTypes.Add(typeof(bool), 0);
            dataTypes.Add(typeof(Vector2), 2);
            dataTypes.Add(typeof(Vector3), 3);
            currentWorld = new World();
            
            //setWorldValue("CurrentMap/CurrentTerrain/Roads/0", currentWorld, null);
        }

        public static object getWorldValue(string path)
        {
            string[] pathParts = path.Split('/');
            object v = currentWorld;
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (v.GetType().IsArray) v = ((Array)v).GetValue(Convert.ToInt32(pathParts[i]));
                else v = v.GetType().GetProperty(pathParts[i]).GetValue(v);
            }
            return v;
        }

        public static object invokeWorldMethod(string path)
        {
            string[] pathParts = path.Split('/');
            object v = currentWorld;
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (v.GetType().IsArray) v = ((Array)v).GetValue(Convert.ToInt32(pathParts[i]));
                else if (v.GetType().GetProperty(pathParts[i]) != null) v = v.GetType().GetProperty(pathParts[i]).GetValue(v);
                else v.GetType().GetMethod(pathParts[i]).Invoke(v, null);
            }
            return v;
        }

        public static object setWorldValue(string path, object v, object o)
        {
            string[] pathParts = path.Split('/');
            if (path == "") return o;
            string newPath = String.Join("/", pathParts, 1, pathParts.Length - 1);
            if (v.GetType().IsArray)
            {
                object currentValue = ((Array)v).GetValue(Convert.ToInt32(pathParts[0]));
                ((Array)v).SetValue(setWorldValue(newPath, currentValue, o), Convert.ToInt32(pathParts[0]));
            }
            else
            {
                PropertyInfo prop = v.GetType().GetProperty(pathParts[0]);
                if (prop != null)
                {
                    prop.SetValue(v, setWorldValue(newPath, prop.GetValue(v), o));
                }
                else
                {
                    FieldInfo field = v.GetType().GetField(pathParts[0]);
                    field.SetValue(v, setWorldValue(newPath, field.GetValue(v), o));
                }
            }
            return v;
        }

        #region Files
        public void LoadMap(string path)
        {
            MapPath = path;
            currentWorld.LoadData(path);
            treeView1.Nodes.Clear();
            AddNode(currentWorld.CurrentMap, treeView1.Nodes, "CurrentMap", "CurrentMap");
            mapView1.UpdateWorld();
            currentWorld.Init();
            currentWorld.Build();
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
            currentWorld = new World();
            MapPath = "";
            treeView1.Nodes.Clear();
            AddNode(currentWorld.CurrentMap, treeView1.Nodes, "CurrentMap", "CurrentMap");
        }
        #endregion

        #region Nodes
        void AddNode(object obj, TreeNodeCollection node, string Title, string path)
        {
            Type type = obj.GetType();
            TreeNode t = new TreeNode(Title, 4, 4);
            
            if (type.IsArray)
            {
                Array objectArray = (Array)obj;
                t = new TreeNode(Title, 5, 5);
                t.ContextMenuStrip = ctxArray;
                for (int i = 0; i < objectArray.Length; i++)
                {
                    AddNode(objectArray.GetValue(i), t.Nodes, i.ToString(), path+"/"+i);
                    t.Nodes[i].ContextMenuStrip = ctxNode;
                }
            }
            else if (dataTypes.ContainsKey(type))
            {
                /*int index = dataTypes.FirstOrDefault(x => x.Value == type).Key;*/
                t = new TreeNode(Title, dataTypes[type], dataTypes[type]);
            }
            else
            {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    AddNode(properties[i].GetValue(obj), t.Nodes, properties[i].Name, path+"/"+properties[i].Name);
                }
            }
            t.Tag = path;
            t.Name = Title;
            node.Add(t);            
        }

        string previousPath = "";
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) treeView1.SelectedNode = e.Node;
            if (previousPath != "" && valueEditor1.GetType() != typeof(Editors.GeneralEditor))
            {
                setWorldValue(previousPath, currentWorld, valueEditor1.GetValue());
            }
            object val = getWorldValue(e.Node.Tag.ToString());
            splitContainer1.Panel1.Controls.Remove(valueEditor1);
            valueEditor1 = Editors.ValueEditor.GetAppropriateEditor(val);
            valueEditor1.Dock = DockStyle.Fill;

            splitContainer1.Panel1.Controls.Add(valueEditor1);
            valueEditor1.Realign();
            if (valueEditor1.GetType() != typeof(Editors.GeneralEditor)) previousPath = e.Node.Tag.ToString();
            else previousPath = "";
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
                currentWorld.Update(glControl1.Focused, 0);
                fpsCounter.Restart();
                currentWorld.Draw(glControl1.Focused);
                fpsCounter.Stop();
                if (fpsCounter.ElapsedMilliseconds == 0) lblFPS.Text = "<1000 FPS";
                else lblFPS.Text = (1 / fpsCounter.ElapsedMilliseconds*1000) + " FPS";
            }
            catch { }
            
            glControl1.SwapBuffers();
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) tabControl1.SelectedIndex = 0;
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
            if (tabControl1.SelectedIndex == 1) glControl1.Focus();
        }
        #endregion

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (valueEditor1 != null) valueEditor1.Realign();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            valueEditor1.Realign();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object arr = getWorldValue(Misc.pathUp(treeView1.SelectedNode.Tag.ToString()));
            Array arrayObject = (Array)arr;
            Array copyArray = Array.CreateInstance(arr.GetType().GetElementType(), arrayObject.Length - 1);
            int j = 0;
            for (int i = 0; i < arrayObject.Length; i++)
            {
                if (i != Convert.ToInt32(Misc.pathName(treeView1.SelectedNode.Tag.ToString())))
                {
                    copyArray.SetValue(arrayObject.GetValue(i), j++);
                }
            }
            setWorldValue(Misc.pathUp(treeView1.SelectedNode.Tag.ToString()), currentWorld, copyArray);
            UpdateArray(Misc.pathUp(treeView1.SelectedNode.Tag.ToString()));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabPage2.Show(); // Switch to page 2 for OpenGL control load
            tabPage1.Show(); // Switch back to page 1
            mapView1.UpdateLocation += MapView1_UpdateLocation;
            mapView1.MoveNode += MapView1_MoveNode;
            LoadMap("C:\\Users\\jakak\\Desktop\\mapa");
        }

        private void MapView1_MoveNode(double X, double Y, object argument)
        {
            
        }

        private void MapView1_UpdateLocation(double X, double Y, object arg)
        {
            lblLocation.Text= string.Format("X: {0,7:0.00} Y: {1,7:0.00}", X, Y);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object arr = getWorldValue(treeView1.SelectedNode.Tag.ToString());
            Array arrayObject = (Array)arr;
            Array copyArray = Array.CreateInstance(arr.GetType().GetElementType(), arrayObject.Length + 1);
            Array.Copy(arrayObject, 0, copyArray, 0, arrayObject.Length);
            
            copyArray.SetValue(Activator.CreateInstance(arr.GetType().GetElementType()), arrayObject.Length);
            setWorldValue(treeView1.SelectedNode.Tag.ToString(), currentWorld, copyArray);
            UpdateArray(treeView1.SelectedNode.Tag.ToString());
        }

        private void UpdateArray(string path)
        {
            string[] pathParts = path.Split('/');
            TreeNode nod = treeView1.Nodes[0];
            
            for (int i = 1; i < pathParts.Length; i++)
            {
                nod = nod.Nodes.Find(pathParts[i], false)[0];
            }
            TreeNode parent = nod.Parent;
            parent.Nodes.Remove(nod);
            AddNode(getWorldValue(path), parent.Nodes, nod.Name, path);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new TextureCollection().Show();
        }
    }
}
