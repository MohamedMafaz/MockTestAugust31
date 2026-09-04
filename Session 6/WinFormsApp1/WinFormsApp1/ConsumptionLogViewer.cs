using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class ConsumptionLogViewer : Form
    {

        bool isloading = true;
        public ConsumptionLogViewer()
        {
            InitializeComponent();
        }

        public class DropDownDTO
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        private async void ConsumptionLogViewer_Load(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7290/api/customers");
            request.Headers.Add("accept", "*/*");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var customers = JsonSerializer.Deserialize<List<DropDownDTO>>(await response.Content.ReadAsStringAsync());


            comboBox1.DataSource = customers;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "id";

           await LOadTheData();

            isloading = false;
        }



        public class ResponseDTO
        {
            public decimal totalEnergyConsumed { get; set; }
            public decimal totalEnergyCost { get; set; }
            public Result[] result { get; set; }
        }

        public class Result
        {
            public int logId { get; set; }
            public string meterSerialNumber { get; set; }
            public DateTime timestamp { get; set; }
            public decimal unitsKwh { get; set; }
            public decimal voltage { get; set; }
            public decimal currentAmps { get; set; }
            public decimal powerKw { get; set; }
            public bool isPeakHour { get; set; }
            public decimal peakHourPricePerUnit { get; set; }
            public decimal pricePerUnit { get; set; }
        }

        private async Task LOadTheData()
        {
            var startdtae = dateTimePicker1.Checked ? dateTimePicker1.Value.ToString("yyyy-MM-dd") : null;
            var endDare = dateTimePicker2.Checked ? dateTimePicker2.Value.ToString("yyyy-MM-dd") : null;
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7290/api/logspgae?customerId={(int)comboBox1.SelectedValue}&startDate={startdtae}&endDate={endDare}");
            //MessageBox.Show($"https://localhost:7290/api/logspgae?customerId={(int)comboBox1.SelectedValue}&startDate={startdtae}&endDate={endDare}");
            request.Headers.Add("accept", "*/*");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<ResponseDTO>(await response.Content.ReadAsStringAsync());

            label6.Text = Math.Round(result.totalEnergyConsumed,2).ToString();
            label7.Text = Math.Round(result.totalEnergyCost).ToString();

            dataGridView1.DataSource = result.result.Select(x=>new
            {
                x.logId,
                x.meterSerialNumber,
                x.timestamp,
                x.unitsKwh,
                x.voltage,
                x.currentAmps,
                x.powerKw,
                x.isPeakHour
            }).ToList() ;


            dataGridView1.Columns["logId"].HeaderText = "Log ID";
            dataGridView1.Columns["meterSerialNumber"].HeaderText = "Meter Serial";
            dataGridView1.Columns["timestamp"].HeaderText = "Timestamp";
            dataGridView1.Columns["unitsKwh"].HeaderText = "Units (kWh)";
            dataGridView1.Columns["voltage"].HeaderText = "Voltage";
            dataGridView1.Columns["currentAmps"].HeaderText = "Current (A)";
            dataGridView1.Columns["powerKw"].HeaderText = "Power (kW)";

            dataGridView1.Columns["isPeakHour"].HeaderText = "Peak Hour Status";

            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                if ((bool)row.Cells["isPeakHour"].Value == true)
                {
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#DC3545");
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
            }



        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
               await LOadTheData();
            }
        }

        private async void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
               await LOadTheData();
            }
        }

        private async void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
                await LOadTheData();
            }
        }
    }
}
