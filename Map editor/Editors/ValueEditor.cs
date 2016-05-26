using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;

namespace Map_editor.Editors
{
    public partial class ValueEditor : UserControl
    {
        static Dictionary<Type, ValueEditor> types = new Dictionary<Type, ValueEditor>
        {
            {typeof(int), new NumberEditor() },
            {typeof(float), new NumberEditor() },
            {typeof(ulong), new NumberEditor() },
            {typeof(string), new StringEditor() },
            {typeof(Vector3), new Vector3Editor() },
            {typeof(Vector2), new Vector2Editor() },
            {typeof(Color4), new Color4Editor() }
        };
        public static ValueEditor GetAppropriateEditor(object Value)
        {
            ValueEditor ed;
            if (Value != null && types.ContainsKey(Value.GetType())) ed = types[Value.GetType()];
            else ed = new GeneralEditor();
            ed.SetValue(Value);
            return ed;
        }
        object Value;
        public ValueEditor()
        {
            InitializeComponent();
        }

        public virtual object GetValue()
        {
            return Value;
        }

        public virtual void SetValue(object value)
        {
            Value = value;
        }

        private void NumberEditor_Load(object sender, EventArgs e)
        {

        }

        public virtual void Realign()
        {

        }
    }
}
