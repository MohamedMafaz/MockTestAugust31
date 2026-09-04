using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        List<GetMeterDTO> meterlist = new();

        bool isloading = true;
        public Form1()
        {
            InitializeComponent();
        }



        public class GetMeterDTO
        {
            public int meterId { get; set; }
            public string meterSerialNumber { get; set; }
            public string customerName { get; set; }
            public float maxVoltageCapacity { get; set; }
            public float dailyUsageLimitKw { get; set; }
            public bool isActive { get; set; }

            public string? description { get; set; }

            public bool anyLogs { get; set; }
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadFormApi();

            LoadDataGridView();

            isloading = false;
        }

        private async Task LoadFormApi()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7290/api/meters");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            meterlist = JsonSerializer.Deserialize<List<GetMeterDTO>>(await response.Content.ReadAsStringAsync());


            comboBox1.DataSource = meterlist.Select(x => x.customerName).Distinct().Prepend("All").ToList();

            LoadDataGridView();
        }

        private void LoadDataGridView()
        {
            var result = meterlist.Where(x => (string.IsNullOrEmpty(textBox1.Text) || x.meterSerialNumber.ToLower().Contains(textBox1.Text.ToLower()) || (x.description != null && x.description.ToLower().Contains(textBox1.Text.ToLower())))
            && (comboBox1.SelectedIndex == 0 || x.customerName == comboBox1.Text) &&
            x.maxVoltageCapacity >= (float)numericUpDown1.Value && x.maxVoltageCapacity <= (float)numericUpDown2.Value
            ).Select(x => new
            {
                x.meterId,
                x.meterSerialNumber,
                x.customerName,
                x.maxVoltageCapacity,
                x.dailyUsageLimitKw,
                Status = x.isActive ? "Active" : "Inactive"
            }).ToList();

            AppData.maxid = result.Max(x => x.meterId);


            dataGridView1.DataSource = result;

            dataGridView1.Columns["meterId"].HeaderText = "Meter ID";
            dataGridView1.Columns["meterSerialNumber"].HeaderText = "Serial Number";
            dataGridView1.Columns["customerName"].HeaderText = "Customer Name";
            dataGridView1.Columns["maxVoltageCapacity"].HeaderText = "Max Voltage (V)";
            dataGridView1.Columns["dailyUsageLimitKw"].HeaderText = "Daily Limit (kW)";

            if (dataGridView1.Columns["Edit"] == null)
            {
                dataGridView1.Columns.Add(new DataGridViewLinkColumn
                {
                    Name = "Edit",
                    Text = "Edit",
                    HeaderText = "Actions",
                    UseColumnTextForLinkValue = true
                });

                dataGridView1.Columns.Add(new DataGridViewLinkColumn
                {
                    Name = "Delete",
                    Text = "Delete",
                    HeaderText = "Actions",
                    UseColumnTextForLinkValue = true
                });
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadDataGridView();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
                LoadDataGridView();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
                LoadDataGridView();
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (!isloading)
            {
                LoadDataGridView();
            }
        }

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var id = (int)dataGridView1.Rows[e.RowIndex].Cells["meterId"].Value;

                if (dataGridView1.Columns[e.ColumnIndex].Name == "Edit")
                {
                    var form = new AddMeter(id);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadFormApi();
                    }
                }
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var meteer = meterlist.FirstOrDefault(x => x.meterId == id);

                    if (meteer.anyLogs)
                    {
                        var form = new MeterDeleteWindow(id);

                        AppData.selectedserail = meteer.meterSerialNumber;
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadFormApi();
                        }
                    }
                    else
                    {
                        var msd = MessageBox.Show("Are you sure?", $"No consumption logs found. Permanently delete Smart Meter {meteer.meterSerialNumber}?", MessageBoxButtons.YesNo);

                        if (msd == DialogResult.Yes)
                        {
                            var client = new HttpClient();
                            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7290/api/meters/{id}");
                            request.Headers.Add("accept", "*/*");
                            var response = await client.SendAsync(request);
                            response.EnsureSuccessStatusCode();
                            Console.WriteLine(await response.Content.ReadAsStringAsync());

                            MessageBox.Show("Deleted Successfully");

                            LoadFormApi();

                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new AddMeter();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadFormApi();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new ConsumptionLogViewer().Show();
            this.Hide();
        }
    }
}
