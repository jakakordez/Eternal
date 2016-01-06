using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Map_editor
{
    public partial class ResourceCollector : Form
    {
        Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
        public string CollectionResult;
        public string location = "";
        public ResourceCollector()
        {
            InitializeComponent();
        }

        private void ResourceCollection_Load(object sender, EventArgs e)
        {
            LoadFolder();
            EGE.Resources.FillTreeview(treeView1.Nodes[0].Nodes);
            treeView1.Nodes[0].Expand();
        }

        private void LoadFolder()
        {
            lstFiles.Items.Clear();
            foreach (var item in EGE.Resources.GetFolderFiles(tlsAddress.Text))
            {
                ListViewItem it = new ListViewItem(new string[] { item.Name + "." + item.Extension, EGE.Misc.sizeToString(item.Size) });
                it.Tag = item;
                lstFiles.Items.Add(it);
            }
        }

       /* private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (var p in openFileDialog1.FileNames)
                {
                    string item = EGE.Misc.pathName(p);
                    if(tabControl1.SelectedIndex == 0)
                    {
                        Bitmap img = (Bitmap)Image.FromFile(p);
                        images.Add(item, img);

                        EGE.Tools.TextureManager.AddTexture(item, img);
                        imgTextures.Images.Add(img);
                        ListViewItem lstItem = new ListViewItem(item, imgTextures.Images.Count - 1);

                        lstTextures.Items.Add(lstItem);
                    }
                    else
                    {
                        //EGE.ResourceManager.AddFile(p);
                        lstFiles.Items.Add(item);
                    }
                }
            }
        }*/

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0 && saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                images[lstFiles.SelectedItems[0].Text].Save(saveFileDialog1.FileName);
                //else File.WriteAllBytes(saveFileDialog1.FileName, EGE.ResourceManager.GetFile(lst.SelectedItems[0].Text));
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0 && MessageBox.Show("Do you want to remove this file?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                EGE.Resources.RemoveFile(lstFiles.SelectedItems[0].Text);
                lstFiles.SelectedItems[0].Remove();
            }
        }

        private void lstFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0 && e.Button == MouseButtons.Left)
            {
                if (((EGE.RFile)lstFiles.SelectedItems[0].Tag).Type == EGE.RFile.RFileType.Mesh)
                    CollectionResult = ((EGE.RFile)lstFiles.SelectedItems[0].Tag).FullName.Replace(".mesh", "");
                else CollectionResult = ((EGE.RFile)lstFiles.SelectedItems[0].Tag).FullName;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (var p in openFileDialog1.FileNames)
                {
                    EGE.Resources.AddFile(p, tlsAddress.Text);
                }
                LoadFolder();
            }
        }

        private void rebuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(lstFiles.SelectedItems.Count > 0)
            {
                EGE.Resources.BuildMesh((EGE.RFile)lstFiles.SelectedItems[0].Tag);
                LoadFolder();
                MessageBox.Show("Done");
            }
        }

        private void tlsAddress_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                LoadFolder();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tlsAddress.Text = getSelectedPath();
            LoadFolder();
        }

        private string getSelectedPath()
        {
            if (treeView1.SelectedNode.FullPath == "") return "";
            else return treeView1.SelectedNode.FullPath.Substring(1, treeView1.SelectedNode.FullPath.Length - 1) + "/";
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level > 0)
            {
                tlsAddress.Text = "";
                LoadFolder();
                EGE.Resources.RemoveFolder(getSelectedPath());
                treeView1.SelectedNode.Remove();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            up();
        }

        private void up()
        {
            if (tlsAddress.Text.Contains('/'))
            {
                tlsAddress.Text = EGE.Misc.pathUp(EGE.Misc.pathUp(tlsAddress.Text));
                if (tlsAddress.Text != "") tlsAddress.Text += "/";
                LoadFolder();
            }
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Enter_text dialog = new Enter_text();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                treeView1.SelectedNode.Nodes.Add(dialog.Text);
                treeView1.SelectedNode.Expand();
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }
    }
}
