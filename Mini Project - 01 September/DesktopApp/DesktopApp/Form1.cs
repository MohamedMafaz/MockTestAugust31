using System.Text.Json;

namespace DesktopApp
{
    public partial class Form1 : Form
    {
        TimeSpan timeElapsed = TimeSpan.Zero;
        bool isRunning = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var endTime = new DateTime(2026, 9, 15);

            TimeSpan remaining = endTime - DateTime.Now;

            label1.Text = $"{remaining.Days} Days, {remaining.Hours} Hours, {remaining.Minutes} Minutes, {remaining.Seconds} seconds to competition";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();

            LoadTheData();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timeElapsed = timeElapsed.Add(TimeSpan.FromSeconds(1));
            label2.Text = $"{timeElapsed.Hours} : {timeElapsed.Minutes} : {timeElapsed.Seconds}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isRunning = !isRunning;
            if (isRunning)
            {
                timer3.Start();
                button1.Text = "Pause";
            }
            else
            {
                timer3.Stop();
                button1.Text = "Start";
            }
        }

        public class DataDTO
        {
            public string Name { get; set; }

            public TimeSpan time { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer3.Stop();
            button1.Text = "Start";

            var msd = MessageBox.Show("Do you want to save this locally?", "Save?", MessageBoxButtons.YesNo);

            if(msd == DialogResult.Yes)
            {
                var file = Path.Combine(Application.StartupPath, "data.json");



                var text = File.ReadAllText(file);

          

                var list = JsonSerializer.Deserialize<List<DataDTO>>(text);

                list.Add(new DataDTO
                {
                    Name = Guid.NewGuid().ToString(),
                    time = timeElapsed
                });

                
                File.WriteAllText(file,JsonSerializer.Serialize(list));

               

                MessageBox.Show("Saved Successfully");
                timeElapsed = TimeSpan.Zero;

                label2.Text = "";

                LoadTheData();
            }
        }

        private void LoadTheData()
        {
            var file = Path.Combine(Application.StartupPath, "data.json");


            var text = File.ReadAllText(file);

            var list = JsonSerializer.Deserialize<List<DataDTO>>(text);

            dataGridView1.DataSource = list;
        }
    }
}
