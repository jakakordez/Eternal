using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;

namespace Map_editor.Editors
{
    class Color4Editor : ValueEditor
    {
        NumericUpDown[] nmrValHolders;

        public Color4Editor()
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
                nmrValHolders[i].DecimalPlaces = 0;
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
            SetValue(new Color4(Convert.ToSingle(nmrValHolders[0].Value/255m), Convert.ToSingle(nmrValHolders[1].Value/255m), Convert.ToSingle(nmrValHolders[2].Value/255m), 255));
        }

        public override void SetValue(object value)
        {
            nmrValHolders[0].Value = Convert.ToDecimal(((Color4)value).R)*255m;
            nmrValHolders[1].Value = Convert.ToDecimal(((Color4)value).G)*255m;
            nmrValHolders[2].Value = Convert.ToDecimal(((Color4)value).B)*255m;
            base.SetValue(value);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Color4Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "Color4Editor";
            this.ResumeLayout(false);

        }
    }
}
