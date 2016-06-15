using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using L2L.Trace;

namespace L2LProducer
{
    class Producer
    {
        private Config Config { get; set; }
        private Dictionary<String, Product> Products { get; set; }
        private Dictionary<String, Queue<IRecord>> Spares { get; set; }
        private TraceWrapper TraceRunner { get; set; }
        private L2L.Trace.Config TraceConf { get; set; }
        public Producer(Config conf)
        {
            this.Config = conf;
            this.Products = new Dictionary<string, Product>();
            this.Spares = new Dictionary<string, Queue<IRecord>>();
            this.TraceRunner = new TraceWrapper();
            this.TraceConf = new L2L.Trace.Config();
            this.TraceConf.Apiurl = conf.Apiurl;
            this.TraceConf.Token = conf.Token;
            this.TraceConf.Reservequantity = conf.Reservequantity;
        }

        public void run()
        {
            setup_products();
            produce_products();
        }

        private void setup_products()
        {
            foreach (Product prod in Config.Products)
            {
                Products[prod.Code] = prod;
                prod.get_next_serial_no();
                Spares[prod.Code] = new Queue<IRecord>();
            }
        }

        private void produce_products()
        {
            Product starting_product = null;
            foreach (KeyValuePair<String, Product> entry in Products)
            {
                if (entry.Value.Root)
                {
                    starting_product = entry.Value;
                    break;
                }
            }

            if (starting_product == null)
            {
                return;
            }

            produce_product(starting_product, starting_product.Quota, true);
            print_serial_numbers();
        }

        private void print_serial_numbers()
        {
            foreach (KeyValuePair<String, Product> entry in Products)
            {
                Console.WriteLine("{0}: {1}", entry.Value.Name, entry.Value.SerialNo);
            }
        }

        private void produce_product(Product product, Int16 quantity, bool initial_product = false)
        {
            for (var counter = 0; counter < quantity; counter++)
            {
                // Produce dependencies
                foreach (Recipe recipe in product.Recipe)
                {
                    produce_product(Products[recipe.Code], recipe.Quantity);
                }

                var subs = new List<IRecord>();
                // Consume dependencies
                foreach (Recipe recipe in product.Recipe)
                {
                    for (var idx = 0; idx < recipe.Quantity; idx++)
                    {
                        subs.Add(Spares[recipe.Code].Dequeue());
                    }
                }

                // Produce the requisite number of items
                var item = create_record_from_product(product, subs) as TraceRecord; // new Item();

                // Produce the item
                Spares[product.Code].Enqueue(item);
                Console.WriteLine("Producing record with typecode: {0}", item.Typecode);

                if (initial_product)
                {
                    Console.WriteLine("\nPausing production momentarily..\n");
                    Thread.Sleep(Config.ProductPause * 1000);
                }
                else
                {
                    Thread.Sleep(Config.SubcomponentPause * 1000);
                }
            }
        }

        private IRecord create_record_from_product(Product product, List<IRecord> subs)
        {
            return TraceRunner.record_from_product(product, subs, TraceConf);
        }
    }
}
