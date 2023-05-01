using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWWatering_desktop
{
    public static class HumiditySimulator
    {
        private static int _humidityAfterWatering = 100;
        private static DateTime _lastWatered = DateTime.Now;

        private static double _functionPoint = -2 + Math.Sqrt(5);

        /*public HumiditySimulator(int humidity)
        {
            _humidityAfterWatering = humidity;
            _lastWatered = DateTime.Now;
        }*/

        public static int GetHumidity()
        {
            double daysSinceLastWatered = (DateTime.Now - _lastWatered).TotalDays;
            double relativeHumidityOffset = 4 - (_humidityAfterWatering / 25.0);
            double res = 1 / (daysSinceLastWatered + relativeHumidityOffset + _functionPoint) - _functionPoint;
            res = Math.Max(0, res);
            res = Math.Min(4, res);
            res = res * 25;
            return (int) res;
        }

        public static double Water(int milliliters)
        {
            milliliters = Math.Max(1000,  milliliters + GetHumidity() * 10);
            _lastWatered = DateTime.Now;
            _humidityAfterWatering = milliliters / 10;
            return GetHumidity();
        }

    }
}
