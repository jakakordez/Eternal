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
    public partial class Enter_text : Form
    {
        public Enter_text()
        {
            InitializeComponent();
        }
        public Enter_text(string title, string Value)
        {
            OldValue = Value;
            textBox1.Text = Value;
            Text = title;
        }
        public string NewValue, OldValue;

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Text = textBox1.Text;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
