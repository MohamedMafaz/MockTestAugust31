using System;
using System.Collections.Generic;
using System.Text;

namespace EnergyGridTesting
{
    public class EnergyGridManager
    {
        public List<MeterReading> list = new List<MeterReading>();

        public void AddReading(MeterReading reading)
        {
            if (string.IsNullOrEmpty(reading.MeterId))
                throw new ArgumentException();

            if (list.Any(x => x.Timestamp == reading.Timestamp && x.MeterId == reading.MeterId))
                throw new ArgumentException();

            list.Add(reading);
        }

        public void RemoveReadingsByMeter(string meterId)
        {
            if (string.IsNullOrEmpty(meterId))
                throw new ArgumentException();

            var meter = list.Where(x => x.MeterId == meterId).ToList();

            if (!meter.Any())
                throw new ArgumentException();

            foreach (var item in meter)
            {
                list.Remove(item);
            }
        }

        public decimal GetTotalConsumption()
        {
            return list.Select(x => new { power = x.Voltage * x.Current }).Sum(x => x.power);
        }

    }
}
