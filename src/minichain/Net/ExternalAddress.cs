using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace minichain
{
    class ExternalAddress
    {
        public static string GetMyExternalIp()
        {
            var web = new HttpClient();
            return web.GetStringAsync("https://api.ipify.org?format=json").Result
                .Split(new string[] { "ip\":\"" }, StringSplitOptions.None)[1]
                .Split(new string[] { "\"" }, StringSplitOptions.None)[0];
        }
    }
}
