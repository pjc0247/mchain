using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace minichain
{
    public class PacketBase
    {
        private static JsonSerializerSettings SerializeSetting =>
            new JsonSerializerSettings()
            {
                ContractResolver = new NonPublicPropertiesResolver(),
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

        /// <summary>
        /// Deserialize the packet from JSON string.
        /// </summary>
        public static PacketBase FromJson(string json)
        {
            try
            {
                return (PacketBase)JsonConvert.DeserializeObject(json, SerializeSetting);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Uniq packet id
        /// This is used to avoid duplicated processing in p2p broadcasting.
        /// </summary>
        public string pid;

        public PacketBase()
        {
            pid = UniqID.Generate();
        }

        /// <summary>
        /// Serialize the packet to JSON string
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, SerializeSetting);
        }
    }

    public class BroadcastPacket : PacketBase
    {
        public int ttl = 10;
    }

    public class PktRequestPeers : PacketBase
    {
    }
    public class PktResponsePeers : PacketBase
    {
        public string[] addrs;
    }

    public class PktRequestBlock : PacketBase
    {
        public int blockNo;
    }
    public class PktResponseBlock : PacketBase
    {
        public Block block;
    }

    public class PktBroadcastNewBlock : BroadcastPacket
    {
        public Block block;
    }
    public class PktNewTransaction : BroadcastPacket
    {
        public int sentBlockNo;
        public Transaction tx;
    }
}
