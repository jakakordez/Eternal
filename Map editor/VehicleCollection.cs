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
            Vehicles.SaveVehicles();
        }

        private void lstVehicles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstVehicles.SelectedIndices.Count > 0)
                objectBrowser1.LoadNodes(Vehicles.VehicleCollection[lstVehicles.SelectedItems[0].Tag.ToString()], "Vehicle");
        }

        private void objectBrowser1_Load(object sender, EventArgs e)
        {

        }

        private void objectBrowser1_ValueChanged(object sender, EventArgs e)
        {
            Vehicles.SaveVehicles();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstVehicles.SelectedItems.Count > 0 && MessageBox.Show("Delete", "Do you really want to delete this vehicle?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Vehicles.RemoveVehicle(lstVehicles.SelectedItems[0].Tag.ToString());
                lstVehicles.SelectedItems[0].Remove();
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstVehicles.SelectedItems.Count > 0)
            {
                Enter_text t = new Enter_text();
                if (t.ShowDialog() == DialogResult.OK)
                {
                    ListViewItem l = lstVehicles.SelectedItems[0];
                    l.SubItems[2].Text = t.Text;

                    string oldKey = l.Tag.ToString();
                    string[] p = oldKey.Split('/');
                    l.Tag = p[0] + "/" + p[1] + "/" + t.Text;
                    Vehicles.RenameVehicle(oldKey, l.Tag.ToString());
                }
            }
        }

        private void changeManufacturerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstVehicles.SelectedItems.Count > 0)
            {
                Enter_text t = new Enter_text();
                if (t.ShowDialog() == DialogResult.OK)
                {
                    ListViewItem l = lstVehicles.SelectedItems[0];
                    l.SubItems[1].Text = t.Text;

                    string oldKey = l.Tag.ToString();
                    string[] p = oldKey.Split('/');
                    l.Tag = p[0] + "/" + t.Text + "/" + p[2];
                    Vehicles.RenameVehicle(oldKey, l.Tag.ToString());
                }
            }
        }
    }
}
