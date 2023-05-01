using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using EmbedIO;
using Renci.SshNet;


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
        public int GetHumidity()
        {
            return HumiditySimulator.GetHumidity();
        }

        [Route(HttpVerbs.Post, "/water")]
        public async Task<int> WaterAsync()
        {
            int milliliters = 0;
            try
            {
                /*StreamReader reader = new StreamReader(Request.InputStream);
                string body = await reader.ReadToEndAsync();
                Console.WriteLine();*/
                var requestData = await HttpContext.GetRequestDataAsync<SliderData>();
                int seconds = requestData.SliderValue;
                milliliters = seconds * 10;
            }
            catch (Exception ex)
            {
                throw;
            }
            return (int) HumiditySimulator.Water(milliliters);
        }

        public class SliderData
        {
            public int SliderValue { get; set; }
            public string? User { get; set; }
        }
    }
}
