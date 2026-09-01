using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Session2_AdminDesktopApp.Models;
using System.Data;

namespace Session2_AdminDesktopApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        EnergyContext db = new EnergyContext();
        DataTable dt = new DataTable();

        private void Form1_Load(object sender, EventArgs e)
        {
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("FacilityName", typeof(string));
            dt.Columns.Add("LocationZone", typeof(string));
            dt.Columns.Add("PeakLoad", typeof(decimal));
            dt.Columns.Add("DialyAvg", typeof(decimal));

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Id"].HeaderText = "Meter ID";
            dataGridView1.Columns["FacilityName"].HeaderText = "Facility Name";
            dataGridView1.Columns["LocationZone"].HeaderText = "Location Zone";
            dataGridView1.Columns["PeakLoad"].HeaderText = "Peak Load (kW)";
            dataGridView1.Columns["DialyAvg"].HeaderText = "Daily Avg (kWh)";


            dataGridView1.Columns.Add(new DataGridViewLinkColumn
            {
                Text = "Edit",
                Name = "Edit",
                HeaderText = "Actions",
                UseColumnTextForLinkValue = true
            });


            dataGridView1.Columns.Add(new DataGridViewLinkColumn
            {
                Text = "Delete",
                Name = "Delete",
                HeaderText = "Actions",
                UseColumnTextForLinkValue = true
            });


            LoadTheData();
        }

        private void LoadTheData()
        {
            var result = db.EnergyMeters.Include(x=>x.Facility).Include(x=>x.ConsumptionLogs).AsEnumerable()
                .Where(x => string.IsNullOrEmpty(textBox1.Text) || x.FacilityId.ToString().Contains(textBox1.Text) || x.Facility.FacilityName.ToLower().Contains(textBox1.Text.ToLower()) || x.LocationZone.ToLower().Contains(textBox1.Text.ToLower())).Select(x => new
            {
                x.MeterId,
                x.Facility.FacilityName,
                x.LocationZone,
                PeakLoad = x.ConsumptionLogs.Any() ?  x.ConsumptionLogs.Max(a => a.PowerKw) :0,
                DailyAvg = x.ConsumptionLogs.Any() ?  x.ConsumptionLogs.AsEnumerable().GroupBy(a => new { date = a.Timestamp.ToString("yyyy-MM-dd") }).Select(a => new
                {
                    a.Key,
                    Total = a.Sum(z => z.PowerKw)
                }).Average(x => x.Total) : 0
            }).ToList();

            dt.Rows.Clear();

            foreach (var item in result)
            {
                dt.Rows.Add(item.MeterId, item.FacilityName, item.LocationZone, item.PeakLoad, item.DailyAvg);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadTheData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var id = (int)dataGridView1.Rows[e.RowIndex].Cells["Id"].Value;
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var msg = MessageBox.Show("Are you sure", "Do you want to delete this meter", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (msg == DialogResult.OK)
                    {
                        var meter = db.EnergyMeters.FirstOrDefault(x => x.MeterId == id);

                        db.EnergyMeters.Remove(meter);
                        db.SaveChanges();

                        MessageBox.Show("Removed Successfully");

                        LoadTheData();
                    }
                }
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Edit")
                {
                    var form = new AddMeter(id);

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadTheData();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new AddMeter();

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadTheData();
            }
        }
    }
}
