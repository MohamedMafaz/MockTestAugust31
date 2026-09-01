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
    public partial class AlertListing : Form
    {

        EnergyContext db = new EnergyContext();
        DataTable dt = new DataTable();
        bool isloading = true;
        public AlertListing()
        {
            InitializeComponent();
        }

        private void AlertListing_Load(object sender, EventArgs e)
        {
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("FacilityName", typeof(string));
            dt.Columns.Add("EventTimestamp", typeof(DateTime));
            dt.Columns.Add("Severity", typeof(string));
            dt.Columns.Add("Status", typeof(string));

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Id"].HeaderText = "Alert ID";
            dataGridView1.Columns["FacilityName"].HeaderText = "Facility Name";
            dataGridView1.Columns["EventTimestamp"].HeaderText = "Event Timestamp";


            dataGridView1.Columns.Add(new DataGridViewLinkColumn
            {
                Name = "Details",
                Text = "Details",
                HeaderText = "Actions",
                UseColumnTextForLinkValue = true
            });

            comboBox1.DataSource = db.Alerts.AsEnumerable().Select(x => x.Status).Distinct().Prepend("All").ToList();


            LoadTheData();

            isloading = false;
        }

        private void LoadTheData()
        {
            var result = db.Alerts.Where(x => (string.IsNullOrEmpty(textBox1.Text) || x.Facility.FacilityName.ToLower().Contains(textBox1.Text.ToLower())) &&
            (comboBox1.SelectedIndex == 0 || x.Status == comboBox1.Text) &&
            (!dateTimePicker1.Checked || DateOnly.FromDateTime(x.EventTimestamp) >= DateOnly.FromDateTime(dateTimePicker1.Value))
            && (!dateTimePicker2.Checked || DateOnly.FromDateTime(x.EventTimestamp) <= DateOnly.FromDateTime(dateTimePicker2.Value))
            ).Select(x => new
            {
                x.AlertId,
                x.Facility.FacilityName,
                x.EventTimestamp,
                x.Severity,
                x.Status
            }).ToList();

            dt.Rows.Clear();

            foreach (var item in result)
            {
                dt.Rows.Add(item.AlertId, item.FacilityName, item.EventTimestamp, item.Severity, item.Status);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadTheData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
                LoadTheData();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
                LoadTheData();
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
                LoadTheData();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Details")
            {
                var id = (int)dataGridView1.Rows[e.RowIndex].Cells["Id"].Value;

                var form = new AlertDetails(id);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadTheData();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var facilities = db.Facilities.ToList();

            
            foreach (var item in facilities)
            {
                var alert = new Alert
                {
                    FacilityId = item.FacilityId,
                    MeterId = 1001,
                    EventTimestamp = DateTime.Now,
                    SurgeReadingKw = 10,
                    ThresholdLimitKw = 90,
                    Severity = "Low",
                    Status = "Pending",
                    Notes = ""
                };

                db.Alerts.Add(alert);
            }

            db.SaveChanges();
            MessageBox.Show("hell");
        }
    }
}
