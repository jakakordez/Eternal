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
        public event EventHandler UpdateWorld;
        public event EventHandler<ulong> NavigateNode;

        public ObjectBrowser()
        {
            InitializeComponent();
            dataTypes.Add(typeof(string), 1);
            dataTypes.Add(typeof(int), 0);
            dataTypes.Add(typeof(float), 0);
            dataTypes.Add(typeof(bool), 0);
            dataTypes.Add(typeof(Vector2), 2);
            dataTypes.Add(typeof(Vector3), 3);
            dataTypes.Add(typeof(ulong), 0);
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
            treeView1.Sort();
            treeView1.Nodes[0].Expand();
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
            else if (typeof(EGE.Environment.ObjectCollection).IsAssignableFrom(type))
            {
                var a = ((EGE.Environment.ObjectCollection)obj).GetNodes();
                t = new TreeNode(Title, 5, 5);
                t.ContextMenuStrip = ctxCollection;
                for (int i = 0; i < a.Length; i++)
                {
                    AddNode(a[i].Value, t.Nodes, a[i].Key, path + "/" + a[i].Key);
                    t.Nodes[i].ContextMenuStrip = ctxCollectionItem;
                }
            }
            else if (dataTypes.ContainsKey(type))
            {
                /*int index = dataTypes.FirstOrDefault(x => x.Value == type).Key;*/
                t = new TreeNode(Title, dataTypes[type], dataTypes[type]);
            }
            else
            {
                PropertyInfo[] properties = type.GetProperties();
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

            if (val.GetType() == typeof(EGE.Environment.NodeReference)) NavigateNode.Invoke(this, ((EGE.Environment.NodeReference)val).ID);
        }

        object getValue(string path)
        {
            string[] pathParts = path.Split('/');
            object v = currentObject;
            for (int i = 0; i < pathParts.Length; i++)
            {
                Type t = v.GetType();
                if (pathParts[i] == "") break;
                if (t.IsArray && i < pathParts.Length) v = ((Array)v).GetValue(Convert.ToInt32(pathParts[i]));
                if (typeof(EGE.Environment.ObjectCollection).IsAssignableFrom(t)) v = ((EGE.Environment.ObjectCollection)v).Get(pathParts[i]);
                else if (t.GetProperty(pathParts[i]) != null) v = t.GetProperty(pathParts[i]).GetValue(v);
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
            Type t = v.GetType();
            if (t.IsArray)
            {
                object currentValue = ((Array)v).GetValue(Convert.ToInt32(path[i]));
                ((Array)v).SetValue(setValue(path, currentValue, o, i+1), Convert.ToInt32(path[i]));
            }
            else if (typeof(EGE.Environment.ObjectCollection).IsAssignableFrom(t))
            {
                var collection = ((EGE.Environment.ObjectCollection)v);
                collection.Set(path[i], (EGE.Environment.Object)setValue(path, collection.Get(path[i]), o, i + 1));
            }
            else
            {
                int k = 1;
                PropertyInfo prop = null;
                for (k = i; k < path.Length; k++)
                {
                    prop = t.GetProperty(path[k]);
                    if (prop != null) break;
                }
                if (prop != null) prop.SetValue(v, setValue(path, prop.GetValue(v), o, k+1));
                else
                {
                    FieldInfo field = t.GetField(path[0]);
                    k = i;
                    
                    while (field == null && k < path.Length) field = t.GetField(path[k++]);
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
            UpdateWorld.Invoke(this, null);
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
            UpdateWorld.Invoke(this, null);
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
            treeView1.Sort();
            treeView1.SelectedNode = parent;
            parent.Expand();
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

        public void Realign()
        {
            if (valueEditor1 != null) valueEditor1.Realign();
        }

        private void prependToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object arr = getValue(treeView1.SelectedNode.Tag.ToString());
            Array arrayObject = (Array)arr;
            Array copyArray = Array.CreateInstance(arr.GetType().GetElementType(), arrayObject.Length + 1);
            Array.Copy(arrayObject, 0, copyArray, 1, arrayObject.Length);


            if (arr.GetType().GetElementType() == typeof(EGE.Environment.NodeReference))
            {
                copyArray.SetValue(new EGE.Environment.NodeReference(Vector3.Zero), 0);
            }
            else if (arr.GetType().GetElementType().GetMethod("Create", BindingFlags.Public | BindingFlags.Static) != null)
            {
                object newObj = arr.GetType().GetElementType().GetMethod("Create", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
                copyArray.SetValue(newObj, 0);
            }
            else copyArray.SetValue(Activator.CreateInstance(arr.GetType().GetElementType()), 0);
            setValue(treeView1.SelectedNode.Tag.ToString().Split('/'), currentObject, copyArray, 0);
            UpdateArray(treeView1.SelectedNode.Tag.ToString());
            UpdateWorld.Invoke(this, null);
        }

        private void changeKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Enter_text et = new Enter_text("Change key", pathName(treeView1.SelectedNode.Tag.ToString()));
            if(et.ShowDialog() == DialogResult.OK)
            {
                EGE.Environment.ObjectCollection collection = (EGE.Environment.ObjectCollection)getValue(pathUp(treeView1.SelectedNode.Tag.ToString()));
                collection.Add(et.NewValue, collection.Get(et.OldValue));
                collection.Remove(et.OldValue);
                UpdateArray(pathUp(treeView1.SelectedNode.Tag.ToString()));
                UpdateWorld.Invoke(this, null);
            }
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Enter_text et = new Enter_text("Enter key", "");
            if (et.ShowDialog() == DialogResult.OK)
            {
                EGE.Environment.ObjectCollection collection = (EGE.Environment.ObjectCollection)getValue(treeView1.SelectedNode.Tag.ToString());
                collection.Add(et.NewValue, new EGE.Environment.Object());
                UpdateArray(pathUp(treeView1.SelectedNode.Tag.ToString()));
                UpdateWorld.Invoke(this, null);
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EGE.Environment.ObjectCollection collection = (EGE.Environment.ObjectCollection)getValue(pathUp(treeView1.SelectedNode.Tag.ToString()));
            collection.Remove(pathName(treeView1.SelectedNode.Tag.ToString()));
            UpdateArray(pathUp(treeView1.SelectedNode.Tag.ToString()));
            UpdateWorld.Invoke(this, null);
        }
    }
}
