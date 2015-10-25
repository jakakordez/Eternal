using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_editor.Editors
{
    class ListEditor:ValueEditor
    {
        public ListEditor()
        {
            Button btnAdd = new Button();
            btnAdd.Text = "+";
            btnAdd.Width = 20;
            btnAdd.Left = 50;
            btnAdd.Top = 5;
            btnAdd.Click += BtnAdd_Click;
            Label lblCount = new Label();
            lblCount.Left = 5;
            lblCount.Top = 5;
                
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            
        }

        public override void SetValue(object value)
        {
 
            base.SetValue(value);
        }
    }
}
