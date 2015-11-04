using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_editor
{
    public partial class TextureCollection : Form
    {
        Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
        public string TextureResult;

        public TextureCollection()
        {
            InitializeComponent();
        }

        private void TextureCollection_Load(object sender, EventArgs e)
        {
            images = EGE.Tools.TextureManager.GetTextures();
            foreach (var item in images)
            {
                imgTextures.Images.Add(item.Value);
                ListViewItem lstItem = new ListViewItem(item.Key, imgTextures.Images.Count - 1);
                listView1.Items.Add(lstItem);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                foreach (var p in openFileDialog1.FileNames)
                {
                    string item = EGE.Misc.pathName(p);
                    Bitmap img = (Bitmap)Image.FromFile(p);
                    images.Add(item, img);
                    
                    EGE.Tools.TextureManager.AddTexture(item, img);
                    imgTextures.Images.Add(img);
                    ListViewItem lstItem = new ListViewItem(item, imgTextures.Images.Count - 1);
                    
                    listView1.Items.Add(lstItem);
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0 && saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                images[listView1.SelectedItems[0].Text].Save(saveFileDialog1.FileName);
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                images.Remove(listView1.SelectedItems[0].Text);
                EGE.Tools.TextureManager.RemoveTexture(listView1.SelectedItems[0].Text);
                listView1.SelectedItems[0].Remove();
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                TextureResult = listView1.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
