using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Renci.SshNet;


namespace WWWatering_desktop
{
    public class HumidityLogger
    {
        private StreamWriter _logStreamWriter;
        private System.Timers.Timer _timer;

        public HumidityLogger(string logFilePath)
        {
            _logStreamWriter = new StreamWriter(logFilePath);
        }

        public void StartLogging()
        {
            _timer = new System.Timers.Timer(60000); // every 60 seconds
            _timer.Elapsed += LogHumidity;
            _timer.Start();
        }

        private void LogHumidity(Object? source, ElapsedEventArgs e)
        {
            _logStreamWriter.WriteLine(e.SignalTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + HumiditySimulator.GetHumidity());
            _logStreamWriter.Flush();
        }

        public void StopLogging()
        {
            _timer.Stop();
            _logStreamWriter.Close();
        }
    }
}
