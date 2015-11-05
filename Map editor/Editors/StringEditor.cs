using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_editor.Editors
{
    class StringEditor : ValueEditor
    {
        ToolStripSplitButton insertButton;
        TextBox txtText;

        public StringEditor()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StringEditor));
            txtText = new TextBox();
            txtText.Top = 5;
            txtText.Left = 50;
            txtText.Width = 140;
            txtText.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            txtText.TextChanged += TxtText_TextChanged;
            Controls.Add(txtText);

            ToolStrip tlsInsert = new ToolStrip();
            insertButton = new ToolStripSplitButton();            

            ToolStripMenuItem tsmTexture = new ToolStripMenuItem("Texture");
            tsmTexture.Click += TsmTexture_Click;

            ToolStripMenuItem tsmHeightfield = new ToolStripMenuItem("Heightfield");
            tsmHeightfield.Click += TsmTexture_Click;

            insertButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            insertButton.DropDownItems.AddRange(new ToolStripItem[] { tsmTexture, tsmHeightfield });
            insertButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            insertButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            insertButton.Size = new System.Drawing.Size(32, 22);

            tlsInsert.Dock = DockStyle.None;
            tlsInsert.Items.Add(insertButton);
            tlsInsert.Location = new System.Drawing.Point(0, 2);

            Controls.Add(tlsInsert);
        }

        private void TsmTexture_Click(object sender, EventArgs e)
        {
            TextureCollection tc = new TextureCollection();
            if (tc.ShowDialog() == DialogResult.OK)
            {
                txtText.Text = tc.TextureResult;
                base.SetValue(tc.TextureResult);
            }
        }

        private void TxtText_TextChanged(object sender, EventArgs e)
        {
            base.SetValue(txtText.Text);
        }

        public override void SetValue(object value)
        {
            txtText.Text = value.ToString();
            base.SetValue(value);
        }
    }
}
