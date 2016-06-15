using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace L2LProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var conf = load_config_file("./config.yaml");
            var producer = new Producer(conf);
            producer.run();
            var new_conf = producer.create_config_file();
            inc_serial_nos(conf);
            save_config_file(conf, "./new_config.yaml");
            Console.Read();
        }

        
        public static Config load_config_file(string filename)
        {
            Config conf = null;
            try
            {
                var stream = new StreamReader(filename);
                var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                conf = deserializer.Deserialize<Config>(stream);
            }
            catch (YamlException except)
            {
                Console.WriteLine("Yaml Configuration exception: {0}", except.InnerException.Message);
                Console.WriteLine("Terminating due to exception");
                System.Environment.Exit(1);
            }
            return conf;
        }

        public static void inc_serial_nos(Config conf)
        {
            foreach (Product prod in conf.Products)
            {
                prod.get_next_serial_no();
            }
        }

        public static void save_config_file(Config conf, string filename)
        {
            try
            {

                var stream = new StreamWriter(filename);
                var serializer = new Serializer(namingConvention: new CamelCaseNamingConvention());
                serializer.Serialize(stream, conf);
                stream.Close();
            }
            catch (YamlException except)
            {
                Console.WriteLine("Yaml Configuration exception: {0}", except.InnerException.Message);
                Console.WriteLine("Terminating due to exception");
                System.Environment.Exit(1);
            }
        }
    }
}
