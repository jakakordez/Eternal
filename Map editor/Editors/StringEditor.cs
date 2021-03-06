﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_editor.Editors
{
    class StringEditor : ValueEditor
    {
        ToolStripButton insertButton;
        TextBox txtText;

        public StringEditor()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StringEditor));
            txtText = new TextBox();
            txtText.Top = 5;
            txtText.Left = 50;
            txtText.Width = 140;
            txtText.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top|AnchorStyles.Right;
            txtText.TextChanged += TxtText_TextChanged;
            Controls.Add(txtText);

            ToolStrip tlsInsert = new ToolStrip();
            insertButton = new ToolStripButton();

            insertButton.Click += InsertButton_Click;          

            insertButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            insertButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            insertButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            insertButton.Size = new System.Drawing.Size(32, 22);

            tlsInsert.Dock = DockStyle.None;
            tlsInsert.Items.Add(insertButton);
            tlsInsert.Location = new System.Drawing.Point(0, 2);

            Controls.Add(tlsInsert);
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            ResourceCollector tc = new ResourceCollector();
            if (tc.ShowDialog() == DialogResult.OK)
            {
                txtText.Text = tc.CollectionResult;
                base.SetValue(tc.CollectionResult);
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
