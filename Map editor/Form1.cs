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
            LoadMap("C:\\Users\\jakak\\Desktop\\mapa");
        }

        #region Files
        public void LoadMap(string path)
        {
            currentWorld.LoadData(path);
            treeView1.Nodes.Clear();
            AddNode(currentWorld, treeView1.Nodes, "Map");
            mapView1.DrawWorld();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                MapPath = folderBrowserDialog1.SelectedPath;
                LoadMap(MapPath);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MapPath != "") currentWorld.SaveData(MapPath);
            else {
                var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    MapPath = dialog.SelectedPath;
                    currentWorld.SaveData(MapPath);
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentWorld = new World();
            MapPath = "";
            treeView1.Nodes.Clear();
            AddNode(currentWorld, treeView1.Nodes, "World");
        }
        #endregion

        #region Nodes
        void AddNode(object obj, TreeNodeCollection node, string Title)
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
                    AddNode(objectArray.GetValue(i), t.Nodes, i.ToString());
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
                    AddNode(properties[i].GetValue(obj), t.Nodes, properties[i].Name);
                }
            }
            t.Tag = obj;
            if (t.Tag == null) System.Diagnostics.Debugger.Break();
            node.Add(t);            
        }

        TreeNode previousNode;
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) treeView1.SelectedNode = e.Node;
            if (previousNode != null && valueEditor1.GetType() != typeof(Editors.GeneralEditor))
            {
                previousNode.Tag = valueEditor1.GetValue();
                RefreshParents(previousNode);
            }
            splitContainer1.Panel1.Controls.Remove(valueEditor1);
            valueEditor1 = Editors.ValueEditor.GetAppropriateEditor(e.Node.Tag);
            valueEditor1.Dock = DockStyle.Fill;

            splitContainer1.Panel1.Controls.Add(valueEditor1);
            valueEditor1.Realign();
            if (valueEditor1.GetType() != typeof(Editors.GeneralEditor)) previousNode = e.Node;
        }

        private void RefreshParents(TreeNode settingNode)
        {
            int level = settingNode.Level;
            for (int i = level; i > 0; i--)
            {
                object parentTag = settingNode.Parent.Tag;
                if (!parentTag.GetType().IsArray)
                {
                    parentTag.GetType().GetProperty(settingNode.Text).SetValue(parentTag, settingNode.Tag);
                }
                else
                {
                    Array parent = (Array)parentTag;
                    parent.SetValue(settingNode.Tag, Convert.ToInt32(settingNode.Text));
                }
                settingNode = settingNode.Parent;
            }

            mapView1.DrawWorld();
        }

        private void RefreshChildren(TreeNode settingNode)
        {
            settingNode.Nodes.Clear();
            Array nodeArray = (Array)settingNode.Tag;
            for (int i = 0; i < nodeArray.Length; i++)
            {
                AddNode(nodeArray.GetValue(i), settingNode.Nodes, i.ToString());
                settingNode.Nodes[i].ContextMenuStrip = ctxNode;
            }
        }

        #endregion

        #region OpenGL

        private void glControl1_Load(object sender, EventArgs e)
        {
            OpenGLLoaded = true;
            GLWidth = glControl1.Width;
            GLHeight = glControl1.Height;
            GL.Viewport(0, 0, GLWidth, GLHeight);
            tmrPreview.Start();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!OpenGLLoaded) return;
        }
        int GLWidth, GLHeight;

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!OpenGLLoaded) return;
            GLWidth = glControl1.Width;
            GLHeight = glControl1.Height;
            GL.Viewport(0, 0, GLWidth, GLHeight);

            base.OnResize(e);
        }

        private void tmpPreview_Tick(object sender, EventArgs e)
        {           
            try
            {
                World w = (World)treeView1.Nodes[0].Tag;
                w.Update(glControl1.Focused, 0);
                w.Resize(glControl1.Width, glControl1.Height);
                w.Draw(glControl1.Focused);
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
            TreeNode selectedNode = treeView1.SelectedNode;
            TreeNode parentNode = selectedNode.Parent;
            Array arrayObject = (Array)parentNode.Tag;
            Array copyArray = Array.CreateInstance(selectedNode.Tag.GetType(), arrayObject.Length - 1);
            int j = 0;
            for (int i = 0; i < arrayObject.Length; i++)
            {
                if (i != Convert.ToInt32(selectedNode.Text))
                {
                    copyArray.SetValue(arrayObject.GetValue(i), j++);
                }
            }
            parentNode.Tag = copyArray;

            RefreshChildren(parentNode);
            RefreshParents(parentNode);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mapView1.UpdateLocation += MapView1_UpdateLocation;
            mapView1.MoveNode += MapView1_MoveNode;
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
            TreeNode selectedNode = treeView1.SelectedNode;
            Array arrayObject = (Array)selectedNode.Tag;
            Array copyArray = Array.CreateInstance(selectedNode.Tag.GetType().GetElementType(), arrayObject.Length + 1);
            Array.Copy(arrayObject, 0, copyArray, 0, arrayObject.Length);
            
            copyArray.SetValue(Activator.CreateInstance(treeView1.SelectedNode.Tag.GetType().GetElementType()), arrayObject.Length);
            selectedNode.Tag = copyArray;

            RefreshChildren(selectedNode);
            RefreshParents(selectedNode);
        }
    }
}
