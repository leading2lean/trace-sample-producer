﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using L2L.Trace;

namespace L2LProducer
{
    class TraceWrapper
    {
        private Queue<String> ReserveIDs { get; set; }

        public TraceWrapper()
        {
            ReserveIDs = new Queue<string>();
        }

        public IRecord record_from_product(Product product, List<IRecord> subs, L2L.Trace.Config conf)
        {
            if (ReserveIDs.Count < 1)
            {
                refill_reserve_ids(conf);
            }

            if (!product.IsLot || (product.IsLot && !product.Registered)) {
                return record_from_serialized_product(product, subs, conf);
            } else
            {
                return record_from_lot_product(product);
            }
        }

        public IRecord record_from_lot_product(Product product)
        {
            if (product.TraceRecord != null)
            {
                return product.TraceRecord as IRecord;
            }

            var record = new TraceRecord();
            record.Typecode = product.Name + " -- " + product.PartNo;
            var exid = new ExternalID();
            exid.PartNumber = product.PartNo;
            exid.SerialNumber = product.SerialNo;
            record.ExternalIds.Add(exid);
            record.GTID = product.GTID;
            product.TraceRecord = record;
            return record as IRecord;
        }

        public IRecord record_from_serialized_product(Product product, List<IRecord> subs, L2L.Trace.Config conf)
        {
            var record = new TraceRecord();
            record.Typecode = product.Name + " -- " + product.PartNo;
            var exid = new ExternalID();
            exid.PartNumber = product.PartNo;
            exid.SerialNumber = product.SerialNo;
            record.ExternalIds.Add(exid);

            foreach (TraceRecord sub_record in subs)
            {
                var subcomp = new Subcomponent();
                subcomp.GTID = sub_record.GTID;
                record.Subcomponents.Add(subcomp);
            }

            var attribute = string.Format("P{0}@{1}", product.PartNo, DateTime.Now.ToString("yyyy-MM-dd"));
            record.Attributes.Add(attribute);

            record.Status = "Good";

            var gtid = ReserveIDs.Dequeue();
            var result = Service.UpdateAsync(conf, record, gtid);
            result.Wait();

            // retry. TODO: look at result code and act appropriately. Actually put the retry logic in the SDK..
            if (!result.Result.Success)
            {
                result = Service.UpdateAsync(conf, record, gtid);
                result.Wait();
            }

            if (product.IsLot)
            {
                product.Registered = true;
                product.TraceRecord = record as IRecord;
            }
            return record as IRecord;
        }

        public void refill_reserve_ids(L2L.Trace.Config conf)
        {
            var result = Service.ReserveAsync(conf);
            result.Wait();
            var r = result.Result;
            
            var reserve_response = r.Response as ReserveResponse;
            ReserveIDs = new Queue<String>(reserve_response.IDs);
        }
    }

    public class ExternalID : IRecord
    {
        public string PartNumber { get; set; }
        public string SerialNumber { get; set; }
    }
}
