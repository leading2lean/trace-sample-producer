using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using Newtonsoft.Json;

using L2L.Trace;

namespace L2LProducer
{
    class Config
    {
        [YamlMember(Alias = "token")]
        public string Token { get; set; }
        public string Apiurl { get; set; }
        public Int16 Reservequantity { get; set; }
        [YamlMember(Alias = "product_pause_time")]
        public Int16 ProductPause { get; set; }
        [YamlMember(Alias = "subcomponent_pause_time")]
        public Int16 SubcomponentPause { get; set; }

        public List<Product> Products { get; set; }
    }

    class Product
    {
        public String Code { get; set; }
        public String Facility { get; set; }
        public String Name { get; set; }
        [YamlMember(Alias = "part_no")]
        public String PartNo { get; set; }

        [YamlMember(Alias = "is_lot")]
        public Boolean IsLot { get; set; }
        [YamlMember(Alias = "registered")]
        public Boolean Registered { get; set; }
        [YamlMember(Alias = "is_root")]
        public Boolean Root { get; set; }
        [YamlMember(Alias = "gtid")]
        public String GTID { get; set; }

        [YamlIgnore]
        private String _serial_no;
        [YamlMember(Alias = "serial_no")]
        public String SerialNo {
            get
            {
                return _serial_no;
            }
            set
            {
                _serial_no = value;
                set_serial_no(value);
            }
        }
        [YamlMember(Alias = "items_in_container")]
        public Int16 ItemsInContainer { get; set; }
        [YamlMember(Alias = "containers_on_pallett")]
        public Int16 ContainersOnPallet { get; set; }
        public Int16 Quota { get; set; }
        public List<Recipe> Recipe { get; set; }
        [YamlIgnore]
        public IRecord TraceRecord { get; set; }

        public Product()
        {
            Recipe = new List<Recipe>();
            Registered = false;
        }

        [YamlIgnore]
        private UInt32 RealSerial { get; set; }

        private void set_serial_no(String value)
        {
            RealSerial = Convert.ToUInt32(value, 16);
        }

        public String get_next_serial_no()
        {
            var retval = String.Format("{0:X}", RealSerial);
            _serial_no = retval;
            RealSerial++;
            return retval;
        }

        [YamlMember(Alias = "private_data")]
        public CustomData PrivateData { get; set; }
    }

    class CustomData : IRecord
    {
        [JsonProperty("_external")]
        [YamlMember(Alias = "external")]
        public ExternalLink External { get; set; }

        [JsonProperty("data")]
        [YamlMember(Alias = "data")]
        public KeyValues Data { get; set; }

    }

    class ExternalLink
    {
        [JsonProperty("link")]
        public String Link { get; set; }
        [JsonProperty("title")]
        public String Title { get; set; }
    }

    class KeyValues
    {
        [JsonProperty("key")]
        public String Key { get; set; }
        [JsonProperty("value")]
        public String Value { get; set; }
        [JsonProperty("_external")]
        [YamlMember(Alias = "external")]
        public ExternalLink External { get; set; }
    }

    class Recipe
    {
        public String Code { get; set; }
        public Int16 Quantity { get; set; }
    }

    class Item
    {
        public String Code { get; set; }
        public String Serial { get; set; }
    }
}
