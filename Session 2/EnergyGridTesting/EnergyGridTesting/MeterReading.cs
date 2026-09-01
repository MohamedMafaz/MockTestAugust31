using System;
using System.Collections.Generic;
using System.Text;

namespace EnergyGridTesting
{
    public class MeterReading
    {
        public string MeterId { get; set; }

        public decimal Voltage { get; set;}

        public decimal Current { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal ThresholdLimit { get; set; }

        public bool IsOverloaded()
        {
            return Current > ThresholdLimit;
        }

    }
}
