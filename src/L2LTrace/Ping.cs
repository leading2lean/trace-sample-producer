//
// Ping.cs: pings the API and returns the response
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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace L2L
{
    namespace Trace
    {
        class Ping
        {
            private string _route = "_/status";

            public async Task<Result> RunAsync(string baseurl)
            {
                var result = new Result();
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseurl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // HTTP GET
                        HttpResponseMessage response = await client.GetAsync(_route);
                        result.HttpCode = response.StatusCode;
                        response.EnsureSuccessStatusCode();
                        result.Success = true;
                        result.Message = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase);
                        var stream = await response.Content.ReadAsStringAsync();
                        result.Response = JsonConvert.DeserializeObject<PingResponse>(stream);
                    }
                } catch (HttpRequestException e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                }
                return result;
            }
        }

        public class PingResponse : IResponse
        {
            [JsonProperty("status")]
            public string Status { get; set; }
        }
    }
}
