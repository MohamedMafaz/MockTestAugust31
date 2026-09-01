using Microsoft.EntityFrameworkCore;
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
    public partial class AddMeter : Form
    {
        int id = 0;
        EnergyContext db = new EnergyContext();
        public AddMeter(int id = 0)
        {
            this.id = id;
            InitializeComponent();
        }

        private void AddMeter_Load(object sender, EventArgs e)
        {
            this.Text = id == 0 ? "Add Meter Form" : "Edit Meter Form"; 

            comboBox1.DataSource = db.Facilities.ToList();
            comboBox1.DisplayMember = "FacilityName";
            comboBox1.ValueMember = "FacilityID";

            if(id != 0)
            {
                PrefillTheForm();
            }
        }

        private void PrefillTheForm()
        {
            var rprev = db.EnergyMeters.AsNoTracking()
                .FirstOrDefault(x => x.MeterId == id);


            comboBox1.SelectedValue = rprev.FacilityId;
            textBox1.Text = rprev.MeterSerialNumber;
            numericUpDown1.Value = rprev.MaxVoltageCapacity;
            numericUpDown2.Value = rprev.BaseTariffRate;
            dateTimePicker1.Value = rprev.InstallationDate.ToDateTime(TimeOnly.MinValue);
            checkBox1.Checked = rprev.IsActive;
            checkBox2.Checked = rprev.IsIndustrial;
            textBox2.Text = rprev.Description;
            textBox3.Text = rprev.LocationZone;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if(comboBox1.SelectedIndex == -1 || string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox3.Text) || !dateTimePicker1.Checked)
            {
                MessageBox.Show("All fields are required");
                return;
            }

            var meter = new EnergyMeter
            {
                MeterId = id,
                MeterSerialNumber = textBox1.Text,
                FacilityId = (int)comboBox1.SelectedValue,
                LocationZone = textBox3.Text,
                MaxVoltageCapacity = numericUpDown1.Value,
                BaseTariffRate = numericUpDown2.Value,
                InstallationDate = DateOnly.FromDateTime(dateTimePicker1.Value),
                IsActive = checkBox1.Checked,
                IsIndustrial = checkBox2.Checked,
                Description = textBox2.Text
            };


            if(id == 0)
            {
                db.EnergyMeters.Add(meter);
            }
            else
            {
                db.EnergyMeters.Update(meter);
            }

            db.SaveChanges();

            MessageBox.Show("Saved Successfully");

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
