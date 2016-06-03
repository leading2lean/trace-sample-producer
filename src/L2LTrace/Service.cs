//
// Service.cs: Implementation of API calls
// 
// Authors:
//   Corey Olsen <corey@leading2lean.com>
// 
// Copyright 2016 Leading2Lean, Inc. (www.leading2lean.com)
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace L2L
{
    namespace Trace
    {
        public class Service
        {
            public static HttpClientHandler ClientHandler { get; set; }

            public static async Task<Result> PingAsync(Config conf)
            {
                var ping = new Ping();
                var result = await ping.RunAsync(conf.Apiurl);
                return result;
            }

            public static async Task<Result> ReserveAsync(Config conf)
            {
                var reserve = new Reserve(Service.ClientHandler);
                var result = await reserve.RunAsync(conf.Apiurl, conf.Token, conf.Reservequantity);
                return result;
            }

            public static async Task<Result> CreateAsync(Config conf, TraceRecord tr)
            {
                var result = await tr.CreateAsync(conf.Apiurl, conf.Token);
                return result;
            }

            public static async Task<Result> UpdateAsync(Config conf, TraceRecord tr, String gtid)
            {
                var result = await tr.UpdateAsync(gtid, conf.Apiurl, conf.Token);
                return result;
            }
        }
    }
}
