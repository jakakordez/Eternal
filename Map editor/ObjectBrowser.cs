using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using System.Reflection;

namespace Map_editor
{
    public partial class ObjectBrowser : UserControl
    {
        Dictionary<Type, int> dataTypes = new Dictionary<Type, int>();
        string previousPath = "";
        public object currentObject;
        public event EventHandler ValueChanged;

        public ObjectBrowser()
        {
            InitializeComponent();
            dataTypes.Add(typeof(string), 1);
            dataTypes.Add(typeof(int), 0);
            dataTypes.Add(typeof(float), 0);
            dataTypes.Add(typeof(bool), 0);
            dataTypes.Add(typeof(Vector2), 2);
            dataTypes.Add(typeof(Vector3), 3);
        }

        private void ObjectBrowser_Load(object sender, EventArgs e)
        {
        }

        public void LoadNodes(object obj, string title)
        {
            previousPath = "";
            treeView1.Nodes.Clear();
            currentObject = obj;
            AddNode(obj, treeView1.Nodes, title, title);
        }

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
                    AddNode(objectArray.GetValue(i), t.Nodes, i.ToString(), path + "/" + i);
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
                    AddNode(properties[i].GetValue(obj), t.Nodes, properties[i].Name, path + "/" + properties[i].Name);
                }
            }
            t.Tag = path;
            t.Name = Title;
            node.Add(t);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) treeView1.SelectedNode = e.Node;
            if (previousPath != "" && valueEditor1.GetType() != typeof(Editors.GeneralEditor))
            {
                setValue(previousPath.Split('/'), currentObject, valueEditor1.GetValue(), 0);
                if(ValueChanged != null) ValueChanged.Invoke(this, null);
            }
            object val = getValue(e.Node.Tag.ToString());
            Controls.Remove(valueEditor1);
            valueEditor1 = Editors.ValueEditor.GetAppropriateEditor(val);
            valueEditor1.Dock = DockStyle.Fill;

            Controls.Add(valueEditor1);
            valueEditor1.Realign();
            if (valueEditor1.GetType() != typeof(Editors.GeneralEditor)) previousPath = e.Node.Tag.ToString();
            else previousPath = "";
        }

        object getValue(string path)
        {
            string[] pathParts = path.Split('/');
            object v = currentObject;
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (pathParts[i] == "" || i >= pathParts.Length) break;
                if (v.GetType().IsArray && i < pathParts.Length-1) v = ((Array)v).GetValue(Convert.ToInt32(pathParts[i]));
                else if (v.GetType().GetProperty(pathParts[i]) != null) v = v.GetType().GetProperty(pathParts[i]).GetValue(v);
            }
            return v;
        }

        PropertyInfo findProperty(string[] pathParts, object o, int startingIndex)
        {
            PropertyInfo k = null;
            for (int i = startingIndex; i < pathParts.Length; i++)
            {
                k = o.GetType().GetProperty(pathParts[i]);
                if (k != null) break;
            }
            return k;
        }

        public object setValue(string[] path, object v, object o, int i)
        {
            if (i >= path.Length) return o;
            if (v.GetType().IsArray)
            {
                object currentValue = ((Array)v).GetValue(Convert.ToInt32(path[i]));
                ((Array)v).SetValue(setValue(path, currentValue, o, i+1), Convert.ToInt32(path[i]));
            }
            else
            {
                int k = 1;
                PropertyInfo prop = null;
                for (k = i; k < path.Length; k++)
                {
                    prop = v.GetType().GetProperty(path[k]);
                    if (prop != null) break;
                }
                if (prop != null) prop.SetValue(v, setValue(path, prop.GetValue(v), o, k+1));
                else
                {
                    FieldInfo field = v.GetType().GetField(path[0]);
                    k = i;
                    
                    while (field == null && k < path.Length) field = v.GetType().GetField(path[k++]);
                    if (field != null) field.SetValue(v, setValue(path, field.GetValue(v), o, k+1));
                }
            }
            return v;
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object arr = getValue(treeView1.SelectedNode.Tag.ToString());
            Array arrayObject = (Array)arr;
            Array copyArray = Array.CreateInstance(arr.GetType().GetElementType(), arrayObject.Length + 1);
            Array.Copy(arrayObject, 0, copyArray, 0, arrayObject.Length);


            if (arr.GetType().GetElementType() == typeof(EGE.Environment.NodeReference))
            {
                copyArray.SetValue(new EGE.Environment.NodeReference(Vector3.Zero), arrayObject.Length);
            }
            else if (arr.GetType().GetElementType().GetMethod("Create", BindingFlags.Public | BindingFlags.Static) != null)
            {
                object newObj = arr.GetType().GetElementType().GetMethod("Create", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
                copyArray.SetValue(newObj, arrayObject.Length);
            }
            else copyArray.SetValue(Activator.CreateInstance(arr.GetType().GetElementType()), arrayObject.Length);
            setValue(treeView1.SelectedNode.Tag.ToString().Split('/'), currentObject, copyArray, 0);
            UpdateArray(treeView1.SelectedNode.Tag.ToString());
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            object arr = getValue(pathUp(treeView1.SelectedNode.Tag.ToString()));
            Array arrayObject = (Array)arr;
            Array copyArray = Array.CreateInstance(arr.GetType().GetElementType(), arrayObject.Length - 1);
            int j = 0;
            for (int i = 0; i < arrayObject.Length; i++)
            {
                if (i != Convert.ToInt32(pathName(treeView1.SelectedNode.Tag.ToString())))
                {
                    copyArray.SetValue(arrayObject.GetValue(i), j++);
                }
            }
            setValue(pathUp(treeView1.SelectedNode.Tag.ToString()).Split('/'), currentObject, copyArray, 0);
            UpdateArray(pathUp(treeView1.SelectedNode.Tag.ToString()));
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
            AddNode(getValue(path), parent.Nodes, nod.Name, path);
        }

        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            invokeWorldMethod(treeView1.SelectedNode.Tag + "/Build");
        }

        public object invokeWorldMethod(string path)
        {
            string[] pathParts = path.Split('/');
            object v = currentObject;
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (v.GetType().IsArray) v = ((Array)v).GetValue(Convert.ToInt32(pathParts[i]));
                else if (v.GetType().GetProperty(pathParts[i]) != null) v = v.GetType().GetProperty(pathParts[i]).GetValue(v);
                else if(v.GetType().GetMethod(pathParts[i]) != null) v.GetType().GetMethod(pathParts[i]).Invoke(v, null);
            }
            return v;
        }

        public static string pathUp(string path)
        {
            string[] p = path.Replace("\\", "/").Split('/');
            return string.Join("/", p, 0, p.Length - 1);
        }

        public static string pathName(string path)
        {
            string[] p = path.Replace("\\", "/").Split('/');
            return p[p.Length - 1];
        }
    }
}
