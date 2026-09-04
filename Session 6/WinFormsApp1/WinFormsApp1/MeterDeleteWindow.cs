using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class MeterDeleteWindow : Form
    {
        int id = 0;
        public MeterDeleteWindow(int id)
        {
            this.id = id;
            InitializeComponent();
        }



        public class EnergyLogDTO
        {
            public DateTime timestamp { get; set; }
            public float unitsKwh { get; set; }
            public float voltage { get; set; }
            public float currentAmps { get; set; }
        }
        List<EnergyLogDTO> list = new();
        private async void MeterDeleteWindow_Load(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7290/api/energyLogs?id={id}");
            request.Headers.Add("accept", "*/*");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            list = JsonSerializer.Deserialize<List<EnergyLogDTO>>(await response.Content.ReadAsStringAsync());



            dataGridView1.DataSource = list;

            dataGridView1.Columns["timestamp"].HeaderText = "Timestamp";
            dataGridView1.Columns["unitsKwh"].HeaderText = "UnitsKWh";
            dataGridView1.Columns["voltage"].HeaderText = "Voltage";
            dataGridView1.Columns["currentAmps"].HeaderText = "CurrentAmps";


        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var msd = MessageBox.Show("Are you sure?", $"Warning: This will permanently erase {list.Count()} energy logs and meter {AppData.selectedserail}. Continue?", MessageBoxButtons.YesNo);

            if (msd == DialogResult.Yes)
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7290/api/meters/{id}");
                request.Headers.Add("accept", "*/*");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());

                MessageBox.Show("Deleted Successfully");

                DialogResult = DialogResult.OK;
                Close();

            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
