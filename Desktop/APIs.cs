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
            return 50;
        }



    }
}
