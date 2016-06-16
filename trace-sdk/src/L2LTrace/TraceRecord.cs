//
// TraceRecord.cs: default representation of a Trace Record data structure
// 
// Authors:
//   Corey Olsen <corey@leading2lean.com>
// 
// Copyright 2016 Leading2Lean, LLC. (www.leading2lean.com)
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace L2L
{
    namespace Trace
    {
        public class IRecord
        {
            public async Task<IResponse> JsonDeserializeAsync<T>(HttpResponseMessage response)
            {
                var stream = await response.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<T>(stream);
                return r as IResponse;
            }
        }

        public class Subcomponent : IRecord
        {
            [JsonProperty("uom")]
            public string UOM { get; set; }

            [JsonProperty("gtid")]
            public string GTID { get; set; }

            [JsonProperty("quantity")]
            public int Quantity { get; set; }

            public Subcomponent()
            {
                UOM = "ns";
                Quantity = 0;
                GTID = "";
            }
        }

        public class TraceRecord : IRecord
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("typecode")]
            public string Typecode { get; set; }

            [JsonProperty("links")]
            public List<Subcomponent> Links { get; set; }

            [JsonProperty("attributes")]
            public List<string> Attributes { get; set; }

            [JsonProperty("subcomponents")]
            public List<Subcomponent> Subcomponents { get; set; }

            [JsonProperty("externalid")]
            public List<IRecord> ExternalIds { get; set; }

            [JsonProperty("privatedata")]
            public IRecord Private { get; set; }

            [JsonProperty("publicdata")]
            public IRecord Public { get; set; }

            [JsonIgnore()]
            public string GTID { get; set; }
            private string Route { get; set; }

            public TraceRecord()
            {
                Status = "";
                Typecode = "";
                Links = new List<Subcomponent>();
                Attributes = new List<string>();
                Subcomponents = new List<Subcomponent>();
                ExternalIds = new List<IRecord>();
            }

            public async Task<Result> CreateAsync(string baseurl, string token)
            {
                Route = "_/create";
                return await RunAsync(baseurl, token);
            }

            public async Task<Result> UpdateAsync(string gtid, string baseurl, string token)
            {
                GTID = gtid;
                Route = gtid;
                return await RunAsync(baseurl, token);
            }

            protected async Task<Result> RunAsync(string baseurl, string token)
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
                        var route = string.Format("{0}?_auth={1}", Route, token);

                        var serialized_data = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});
                        var content = new StringContent(serialized_data, System.Text.Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(route, content);
                        result.HttpCode = response.StatusCode;
                        result.Message = string.Format("{0}", response.StatusCode);
                        IResponse r = null;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            result.Success = true;
                            r = await JsonDeserializeAsync<TraceRecordResponse>(response);
                            result.Response = r as TraceRecordResponse;
                        }
                        else
                        {
                            result.Success = false;
                            r = await JsonDeserializeAsync<ErrorResponse>(response);
                            result.Response = r as ErrorResponse;
                        }
                        
                    }
                }
                catch (HttpRequestException e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                }
                return result;
            }
        }

        public class TraceRecordResponse : IResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("gtid")]
            public string GTID { get; set; }
        }
    }
}
