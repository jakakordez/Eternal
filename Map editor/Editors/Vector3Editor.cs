using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace Map_editor.Editors
{
    class Vector3Editor : ValueEditor
    {
        NumericUpDown[] nmrValHolders;

        public Vector3Editor()
        {
            nmrValHolders = new NumericUpDown[3];
            for (int i = 0; i < 3; i++)
            {
                nmrValHolders[i] = new NumericUpDown();
                nmrValHolders[i].Top = 5;
                nmrValHolders[i].Maximum = decimal.MaxValue;
                nmrValHolders[i].Minimum = decimal.MinValue;
                nmrValHolders[i].Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
                nmrValHolders[i].ValueChanged += NmrValueHolder_ValueChanged;
                nmrValHolders[i].DecimalPlaces = 3;
                Controls.Add(nmrValHolders[i]);
            }
            Realign();
        }

        public override void Realign()
        {
            for (int i = 0; i < 3; i++)
            {
                
                Controls[i].Width = (Width-20)/3;
                Controls[i].Left = (nmrValHolders[i].Width+5)*i+5;
            }
        }

        private void NmrValueHolder_ValueChanged(object sender, EventArgs e)
        {
            SetValue(new Vector3(Convert.ToSingle(nmrValHolders[0].Value), Convert.ToSingle(nmrValHolders[1].Value), Convert.ToSingle(nmrValHolders[2].Value)));
        }

        public override void SetValue(object value)
        {
            nmrValHolders[0].Value = Convert.ToDecimal(((Vector3)value).X);
            nmrValHolders[1].Value = Convert.ToDecimal(((Vector3)value).Y);
            nmrValHolders[2].Value = Convert.ToDecimal(((Vector3)value).Z);
            base.SetValue(value);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Vector3Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "Vector3Editor";
            this.ResumeLayout(false);

        }
    }
}
