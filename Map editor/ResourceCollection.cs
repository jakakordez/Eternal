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
        public ResourceCollector()
        {
            InitializeComponent();
        }

        private void ResourceCollection_Load(object sender, EventArgs e)
        {
            images = EGE.Tools.TextureManager.GetTextures();
            foreach (var item in images)
            {
                imgTextures.Images.Add(item.Value);
                ListViewItem lstItem = new ListViewItem(item.Key, imgTextures.Images.Count - 1);
                lstTextures.Items.Add(lstItem);
            }
            foreach (var item in EGE.Tools.ResourceManager.Files)
            {
                lstFiles.Items.Add(new ListViewItem(new string[] { item.Key, EGE.Misc.sizeToString(item.Value) }));
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
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
                        EGE.Tools.ResourceManager.AddFile(p);
                        lstFiles.Items.Add(item);
                    }
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView lst;
            if (tabControl1.SelectedIndex == 0) lst = lstTextures;
            else lst = lstFiles;
            if (lst.SelectedItems.Count > 0 && saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (tabControl1.SelectedIndex == 0) images[lst.SelectedItems[0].Text].Save(saveFileDialog1.FileName);
                else File.WriteAllBytes(saveFileDialog1.FileName, EGE.Tools.ResourceManager.GetResource(lst.SelectedItems[0].Text));
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (lstTextures.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show("Do you want to remove the file?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        images.Remove(lstTextures.SelectedItems[0].Text);
                        EGE.Tools.TextureManager.RemoveTexture(lstTextures.SelectedItems[0].Text);
                        lstTextures.SelectedItems[0].Remove();
                    }
                }
            }
            else if (lstFiles.SelectedItems.Count > 0 && MessageBox.Show("Do you want to remove the file?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                EGE.Tools.ResourceManager.RemoveFile(lstFiles.SelectedItems[0].Text);
                lstFiles.SelectedItems[0].Remove();
            }

        }

        private void lstTextures_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView lst;
            if (tabControl1.SelectedIndex == 0) lst = lstTextures;
            else lst = lstFiles;
            if (lst.SelectedItems.Count > 0 && e.Button == MouseButtons.Left)
            {
                CollectionResult = lst.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
