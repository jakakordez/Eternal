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
            InitializeComponent();
            OldValue = Value;
            textBox1.Text = Value;
            Text = title;
        }

        public Enter_text(string title, string Value, string[] options)
        {
            InitializeComponent();
            OldValue = Value;
            textBox1.Text = Value;
            Text = title;
            comboBox1.Enabled = true;
            comboBox1.Items.AddRange(options);
            comboBox1.SelectedIndex = 0;
        }
        public string NewValue, OldValue;
        public int selectedOption = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Text = textBox1.Text;
            NewValue = Text;
            selectedOption = comboBox1.SelectedIndex;
            Close();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Text = textBox1.Text;
                NewValue = Text;
                selectedOption = comboBox1.SelectedIndex;
                Close();
            }
            if(e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
