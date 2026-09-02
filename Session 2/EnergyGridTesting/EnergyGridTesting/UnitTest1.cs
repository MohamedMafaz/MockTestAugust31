using System.Runtime.CompilerServices;

namespace EnergyGridTesting
{
    public class Tests
    {
        [TestCase("1001",12.9,34.1, 10)]
        [TestCase("1002", 0,0, 0)]
        [TestCase("", 0, 0, 0)]
        [TestCase(null, 0, 0, 0)]
        public void TestingHappyPath(string? id, decimal voltage, decimal current, decimal thresholdLimit)
        {
            var meter = new MeterReading
            {
                MeterId = id,
                Voltage = voltage,
                Current = current,
                Timestamp = DateTime.Now,
                ThresholdLimit = thresholdLimit
            };

            EnergyGridManager manager = new EnergyGridManager();


            if (string.IsNullOrEmpty(id))
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    manager.AddReading(meter);

                });
            }
            else
            {
                manager.AddReading(meter);


                Assert.That(manager.list.Any(x => x.MeterId == id));
            }

        }

        [TestCase(10, 5, 10, false)]
        [TestCase(10, 11, 10, true)]
        [TestCase(10, 10, 10, false)]
        [TestCase(5, 0, 10, false)]
        [TestCase(10, 100, 50, true)]
        public void IsOverloaded_ShouldReturnExpectedresult(
            decimal voltage,
            decimal current,
            decimal threshold,
            bool expected)
        {
            var reading = new MeterReading
            {
                MeterId = "M001",
                Voltage = voltage,
                Current = current,
                ThresholdLimit = threshold,
                Timestamp = DateTime.Now
            };

            
            var result = reading.IsOverloaded();

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("")]
        [TestCase("1001")]
        [TestCase("1002")]
        public void Delete_Testing(string meterif)
        {
            EnergyGridManager manager = new EnergyGridManager();


            var meter = new MeterReading
            {
                MeterId = "1001",
                Voltage = 0,
                Current = 0,
                Timestamp = DateTime.Now,
                ThresholdLimit = 0
            };

            var meter2 = new MeterReading
            {
                MeterId = "1001",
                Voltage = 0,
                Current = 0,
                Timestamp = DateTime.Now.AddMinutes(10),
                ThresholdLimit = 0
            };

            manager.AddReading(meter);
            manager.AddReading(meter2);

            if (string.IsNullOrWhiteSpace(meterif))
            {
                Assert.Throws<ArgumentException>(() => manager.RemoveReadingsByMeter(meterif));
            }
            else
            {
                if(meterif == "1001")
                {
                    manager.RemoveReadingsByMeter(meterif);
                    Assert.That(manager.list.Any(x => x.MeterId == meterif), Is.False);
                }
                else
                {
                    Assert.Throws<ArgumentException>(() =>
                    {
                        manager.RemoveReadingsByMeter(meterif);
                    });
                }
            }


        }

        [Test]
        public void GetTotalConsumption_HappyPath()
        {
            EnergyGridManager manager = new EnergyGridManager();

            manager.AddReading(new MeterReading
            {
                MeterId = "M001",
                Voltage = 230,
                Current = 10,
                ThresholdLimit = 15,
                Timestamp = new DateTime(2026, 9, 1, 10, 0, 0)
            });

            manager.AddReading(new MeterReading
            {
                MeterId = "M002",
                Voltage = 240,
                Current = 5,
                ThresholdLimit = 10,
                Timestamp = new DateTime(2026, 9, 1, 11, 0, 0)
            });

            manager.AddReading(new MeterReading
            {
                MeterId = "M003",
                Voltage = 220,
                Current = 2,
                ThresholdLimit = 5,
                Timestamp = new DateTime(2026, 9, 1, 12, 0, 0)
            });

           
            var result = manager.GetTotalConsumption();


            Assert.That(result, Is.EqualTo(3940));
        }


        [Test]
        public void GetTotalConsumption_returnZero()
        {
            EnergyGridManager manager = new EnergyGridManager();

            var result = manager.GetTotalConsumption();

   
            Assert.That(result, Is.EqualTo(0));
        }


        [TestCase("01", "01", "2026-09-01 10:00", "2026-09-01 10:00")]
        [TestCase("01", "01", "2026-09-01 10:00", "2026-09-01 11:00")]
        [TestCase("01", "02", "2026-09-01 10:00", "2026-09-01 10:00")]
        [TestCase("01", "02", "2026-09-01 10:00", "2026-09-01 11:00")]

        public void AddReading_ShouldHandleDuplicateConditions(
            string meterid1,
            string meterid2,
            string timestampString1,
            string timestampString2)
        {
            var manager = new EnergyGridManager();

            DateTime timestamp1 = DateTime.Parse(timestampString1);
            DateTime timestamp2 = DateTime.Parse(timestampString2);

            var reading1 = new MeterReading
            {
                MeterId = meterid1,
                Voltage = 230,
                Current = 10,
                ThresholdLimit = 15,
                Timestamp = timestamp1
            };

            var reading2 = new MeterReading
            {
                MeterId = meterid2,
                Voltage = 240,
                Current = 20,
                ThresholdLimit = 25,
                Timestamp = timestamp2
            };

            manager.AddReading(reading1);

            if (meterid1 == meterid2 && timestamp1 == timestamp2)
            {
                Assert.Throws<ArgumentException>(
                    () => manager.AddReading(reading2));

                Assert.That(manager.list.Count, Is.EqualTo(1));
            }
            else
            {
                manager.AddReading(reading2);

                Assert.That(manager.list.Count, Is.EqualTo(2));
            }
        }


    }
}

