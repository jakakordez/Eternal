﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_editor.Editors
{
    class GeneralEditor : ValueEditor
    {
        Label lblTitle;

        public GeneralEditor()
        {
            lblTitle = new Label();
            lblTitle.Top = 5;
            lblTitle.Left = 5;
            lblTitle.Width = 140;
            Controls.Add(lblTitle);
        }

        public override void SetValue(object value)
        {
            if (value == null) lblTitle.Text = "";
            else lblTitle.Text = value.ToString();
            base.SetValue(value);
        }

        public override void Realign()
        {
            lblTitle.Width = Width;
        }
    }
}
