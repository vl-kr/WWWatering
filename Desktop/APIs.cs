using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using EmbedIO;
using Renci.SshNet;
using Swan.Logging;

namespace WWWatering_desktop
{
    public class APIs : WebApiController
    {
        [Route(HttpVerbs.Get, "/check-connection")]
        public bool CheckConnection()
        {
            return true;
        }

        [Route(HttpVerbs.Get, "/get-humidity")]
        public object GetHumidity()
        {
            int humdity = HumiditySimulator.GetHumidity();

            return new { Humidity = humdity };
        }

        [Route(HttpVerbs.Post, "/water")]
        public async Task<int> WaterAsync()
        {
            int milliliters = 0;
            try
            {
                var requestData = await HttpContext.GetRequestDataAsync<Dictionary<string, string>>();
                int seconds = Convert.ToInt32(requestData["SliderValue"]);
                string user = requestData["User"];
                Logger.Info($"Watering for {seconds} seconds by {user}");
            }
            catch (Exception ex)
            {
                throw;
            }
            return (int) HumiditySimulator.Water(milliliters);
        }
    }
}
