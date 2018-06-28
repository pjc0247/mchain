using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace minichain
{
    class ExternalAddress
    {
        public static string[] GetMyLocalIps()
        {
            NetworkInterfaceType _type = NetworkInterfaceType.Ethernet;
            List<string> outputs = new List<string>();

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            outputs.Add(ip.Address.ToString());
                        }
                    }
                }
            }

            return outputs.ToArray();
        }
        public static string GetMyExternalIp()
        {
            var web = new HttpClient();
            return web.GetStringAsync("https://api.ipify.org?format=json").Result
                .Split(new string[] { "ip\":\"" }, StringSplitOptions.None)[1]
                .Split(new string[] { "\"" }, StringSplitOptions.None)[0];
        }
    }
}
