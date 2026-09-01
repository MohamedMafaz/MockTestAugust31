using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Session2_AdminDesktopApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Session2_AdminDesktopApp
{
    public partial class AlertDetails : Form
    {
        int id = 0;
        EnergyContext db = new EnergyContext();

        public AlertDetails(int id)
        {
            this.id = id;
            InitializeComponent();
        }

        private void AlertDetails_Load(object sender, EventArgs e)
        {
            var alert = db.Alerts.Include(x => x.Facility)
                .FirstOrDefault(x => x.AlertId == id);

            comboBox1.DataSource = new List<string> { "Resolved", "Dismissed" };

            label6.Text = alert.AlertId.ToString();
            label7.Text = alert.Facility.LocationZone;
            label8.Text = alert.SurgeReadingKw.ToString();
            label9.Text = alert.ThresholdLimitKw.ToString();
            comboBox1.SelectedText = alert.Status;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var alert = db.Alerts.Include(x => x.Facility)
              .FirstOrDefault(x => x.AlertId == id);

            alert.Status = comboBox1.Text;
            db.SaveChanges();

            MessageBox.Show("Saved Successfully");

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
