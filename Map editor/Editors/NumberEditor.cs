using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_editor.Editors
{
    class NumberEditor:ValueEditor
    {
        NumericUpDown nmrValueHolder;
        Type numberType;

        public NumberEditor()
        {
            nmrValueHolder = new NumericUpDown();
            nmrValueHolder.Top = 5;
            nmrValueHolder.Left = 5;
            nmrValueHolder.Width = 140;
            nmrValueHolder.Maximum = decimal.MaxValue;
            nmrValueHolder.Minimum = decimal.MinValue;
            nmrValueHolder.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            nmrValueHolder.ValueChanged += NmrValueHolder_ValueChanged;
            Controls.Add(nmrValueHolder);
        }

        private void NmrValueHolder_ValueChanged(object sender, EventArgs e)
        {
            if (numberType == typeof(int)) SetValue(Convert.ToInt32(nmrValueHolder.Value));
            else if (numberType == typeof(ulong)) SetValue(Convert.ToUInt64(nmrValueHolder.Value));
            else SetValue(Convert.ToSingle(nmrValueHolder.Value));
        }

        public override void SetValue(object value)
        {
            numberType = value.GetType();
            nmrValueHolder.DecimalPlaces = numberType==typeof(Single) ? 3 : 0;
            nmrValueHolder.Value = Convert.ToDecimal(value);
            base.SetValue(value);
        }
    }
}
