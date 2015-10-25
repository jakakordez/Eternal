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
        TextBox txtText;

        public StringEditor()
        {
            txtText = new TextBox();
            txtText.Top = 5;
            txtText.Left = 5;
            txtText.Width = 140;
            txtText.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            txtText.TextChanged += TxtText_TextChanged;
            Controls.Add(txtText);
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
