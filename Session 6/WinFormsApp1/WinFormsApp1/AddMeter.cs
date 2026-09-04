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
    public partial class AddMeter : Form
    {

        int id = 0;
        public class DropDownDTO
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public AddMeter(int id = 0)
        {
            this.id = id;
            InitializeComponent();
        }

        private async void AddMeter_Load(object sender, EventArgs e)
        {
            await LoadTransformers();
            await LoadCustoemrs();
            await LoadTechnicians();
            await LoadPlans();

            if (id != 0)
            {
                await LoadTheContent();
            }
        }

        private async Task LoadTheContent()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7290/api/singleMeter?id={id}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var single  = JsonSerializer.Deserialize<GetSingleDTO>(await response.Content.ReadAsStringAsync());

            textBox1.Text = single.meterSerialNumber;
            comboBox1.SelectedValue = single.transformerId;
            comboBox2.SelectedValue = single.userId;
            comboBox3.SelectedValue = single.assignedTechnicianId;
            comboBox4.SelectedValue = single.tariffPlanId;
            numericUpDown1.Value = (decimal)single.maxVoltageCapacity;
            numericUpDown2.Value = (decimal)single.dailyUsageLimitKw;

        }

        public class GetSingleDTO
        {
            public int meterId { get; set; }
            public string meterSerialNumber { get; set; }
            public int transformerId { get; set; }
            public int userId { get; set; }
            public int assignedTechnicianId { get; set; }
            public int tariffPlanId { get; set; }
            public float latitude { get; set; }
            public float longitude { get; set; }
            public float maxVoltageCapacity { get; set; }
            public float dailyUsageLimitKw { get; set; }
            public string installationDate { get; set; }
            public bool isActive { get; set; }
            public bool isIndustrial { get; set; }
            public object description { get; set; }
        }


        private async Task LoadTransformers()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7290/api/transformers");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var transformers = JsonSerializer.Deserialize<List<DropDownDTO>>(await response.Content.ReadAsStringAsync());
            comboBox1.DataSource = transformers;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Id";
            comboBox1.SelectedIndex = -1;
        }

        private async Task LoadCustoemrs()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7290/api/customers");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var transformers = JsonSerializer.Deserialize<List<DropDownDTO>>(await response.Content.ReadAsStringAsync());
            comboBox2.DataSource = transformers;
            comboBox2.DisplayMember = "Name";
            comboBox2.ValueMember = "Id";
            comboBox2.SelectedIndex = -1;
        }


        private async Task LoadTechnicians()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7290/api/technicians");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var transformers = JsonSerializer.Deserialize<List<DropDownDTO>>(await response.Content.ReadAsStringAsync());
            comboBox3.DataSource = transformers;
            comboBox3.DisplayMember = "Name";
            comboBox3.ValueMember = "Id";
            comboBox3.SelectedIndex = -1;
        }
        private async Task LoadPlans()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7290/api/plans");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var transformers = JsonSerializer.Deserialize<List<DropDownDTO>>(await response.Content.ReadAsStringAsync());
            comboBox4.DataSource = transformers;
            comboBox4.DisplayMember = "Name";
            comboBox4.ValueMember = "Id";
            comboBox4.SelectedIndex = -1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }



        public class CreateMeterDTO
        {
            public int meterId { get; set; }
            public string meterSerialNumber { get; set; }
            public int transformerId { get; set; }
            public int userId { get; set; }
            public int assignedTechnicianId { get; set; }
            public int tariffPlanId { get; set; }
            public int latitude { get; set; }
            public int longitude { get; set; }
            public decimal maxVoltageCapacity { get; set; }
            public decimal dailyUsageLimitKw { get; set; }
            public string installationDate { get; set; }
            public bool isActive { get; set; }
            public bool isIndustrial { get; set; }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 || comboBox3.SelectedIndex == -1 || comboBox4.SelectedIndex == -1)
            {
                MessageBox.Show("All fields are required");
                return;
            }

            string text = textBox1.Text.Trim();

            string[] parts = text.Split('-');

            bool isInvalid = false;

            if (parts.Length != 3)
            {
                isInvalid = true;
            }
            else
            {
                if (parts[0] != "EM")
                {
                    isInvalid = true;
                }

                if (string.IsNullOrEmpty(parts[1]))
                {
                    isInvalid = true;
                }
                else
                {
                    foreach (char c in parts[1])
                    {
                        if (!char.IsLetter(c) || !char.IsUpper(c))
                        {
                            isInvalid = true;
                            break;
                        }
                    }
                }
                if (!int.TryParse(parts[2], out int number))
                {
                    isInvalid = true;
                }
                else
                {
                    if (id != 0)
                    {
                        if(number > AppData.maxid)
                        {
                            isInvalid = true;
                        }
                    }
                    else
                    {
                        if (number != AppData.maxid + 1)
                        {
                            isInvalid = true;
                        }

                    }
                 
                }
            }

            if (isInvalid)
            {
                MessageBox.Show(
                    "Must follow the pattern\r\n" +
                    "EM-{LETTERS}-{NUMBER+1} (e.g., EM-RES-1002).\r\n" +
                    "■ EM-: Mandatory prefix.\r\n" +
                    "■ {LETTERS}: Uppercase alphabetic category code (e.g., RES, IND, COMM).\r\n" +
                    "■ {NUMBER+1}: Auto-generated integer sequence incremented by +1 based on the highest existing meter ID."
                );

                return;
            }


            var a = new CreateMeterDTO
            {
                meterId = id,
                meterSerialNumber = textBox1.Text,
                transformerId = (int)comboBox1.SelectedValue,
                userId = (int)comboBox2.SelectedValue,
                assignedTechnicianId = (int)comboBox3.SelectedValue,
                tariffPlanId = (int)comboBox4.SelectedValue,
                latitude = 0,
                longitude = 0,
                maxVoltageCapacity = (decimal)numericUpDown1.Value,
                dailyUsageLimitKw = (decimal)numericUpDown2.Value,
                installationDate = DateTime.Now.ToString("yyyy-MM-dd"),
                isActive = true,
                isIndustrial = false
            };

            if (id == 0)
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7290/api/meters");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(JsonSerializer.Serialize(a), null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var output = await response.Content.ReadAsStringAsync();

                MessageBox.Show(output);

                if (response.IsSuccessStatusCode)
                {
                    DialogResult = DialogResult.OK;
                }
            }
            else
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7290/api/meters/{id}");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(JsonSerializer.Serialize(a), null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var output = await response.Content.ReadAsStringAsync();

                MessageBox.Show(output);

                if (response.IsSuccessStatusCode)
                {
                    DialogResult = DialogResult.OK;
                }

            }



        }
    }
}
