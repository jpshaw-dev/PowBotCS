using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBot1.config
{
    internal class JSONReader
    {
        public string token {  get; set; }
        public string prefix { get; set; }

        public string connectionStringDB { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStruc data = JsonConvert.DeserializeObject<JSONStruc>(json);

                this.token = data.token; 
                this.prefix = data.prefix;
                this.connectionStringDB = data.connectionStringDB;
            }
        }
    }

    internal sealed class JSONStruc
    {
        public string token { get; set; }
        public string prefix { get; set; }

        public string connectionStringDB { get; set; }
    }
}
