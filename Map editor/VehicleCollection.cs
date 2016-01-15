using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EGE.Vehicles;

namespace Map_editor
{
    public partial class VehicleCollection : Form
    {
        public VehicleCollection()
        {
            InitializeComponent();
        }

        private void VehicleCollection_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, Vehicle> item in Vehicles.VehicleCollection)
            {
                string[] items = item.Key.Split('/');
                ListViewItem i = new ListViewItem(items);
                i.Tag = item.Key;
                lstVehicles.Items.Add(i);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = 0;
            string s;
            while (true)
            {
                i++;
                s = cmbVehicleType.Text+"/unknown/unknown" + i;
                if (Vehicles.VehicleCollection.ContainsKey(s)) continue;
                Vehicle v = Vehicle.VehicleFromString(cmbVehicleType.Text);
                Vehicles.VehicleCollection.Add(s, v);
                ListViewItem j = new ListViewItem(s.Split('/'));
                j.Tag = s;
                lstVehicles.Items.Add(j);
                break;
            }
        }

        private void lstVehicles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstVehicles.SelectedIndices.Count > 0)
                objectBrowser1.LoadNodes(Vehicles.VehicleCollection[lstVehicles.SelectedItems[0].Tag.ToString()], "Vehicle");
        }

        private void objectBrowser1_Load(object sender, EventArgs e)
        {

        }
    }
}
