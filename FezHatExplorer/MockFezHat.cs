using System;
using Windows.UI.Xaml;
using FezHatExplorer.Model;

namespace FezHatExplorer
{
    public class MockFezHat : FezHatItem
    {
        public MockFezHat(int seed) 
        {
            UniqueName = (Guid.NewGuid()).ToString();
            DefaultAppName = "Mock FEZ HAT";
            DateOfManufacture = DateTime.Now;
            ModelNumber = "1.0";
            Consumer = null;

            var t = new DispatcherTimer
            {
                Interval = new TimeSpan(TimeSpan.TicksPerSecond/4)
            };
            t.Tick += (sender, o) =>
                        {
                            var random = new Random(DateTime.Now.Second + seed);
                            Temperature = random.NextDouble() + 86.0;
                            LightLevel = random.NextDouble();
                        };
            t.Start();
        }
    }
}
