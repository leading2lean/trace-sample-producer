//
// Reserve.cs: reserves a block of gtids
// 
// Authors:
//   Corey Olsen <corey@leading2lean.com>
// 
// Copyright 2016 Leading2Lean, LLC. (www.leading2lean.com)
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
        class Reserve
        {
            private HttpClientHandler _handler = null;
            private string _route = "_/reserve";

            public Reserve(HttpClientHandler httpClientHandler = null)
            {
                _handler = httpClientHandler;
            }

            public async Task<Result> RunAsync(string baseurl, string token, Int16 quantity = 1000)
            {
                var result = new Result();
                try
                {
                    using (var client = _handler == null
                                  ? new HttpClient()
                                  : new HttpClient(_handler))
                    {
                        client.BaseAddress = new Uri(baseurl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // HTTP GET
                        var route = string.Format("{0}?_auth={1}&quantity={2}", _route, token, quantity);
                        HttpResponseMessage response = await client.PostAsync(route, new StringContent(string.Empty));
                        result.HttpCode = response.StatusCode;
                        result.Message = string.Format("{0}", response.StatusCode);
                        IResponse r = null;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            result.Success = true;
                            r = await JsonDeserializeAsync<ReserveResponse>(response);
                            result.Response = r as ReserveResponse;
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

            public async Task<IResponse> JsonDeserializeAsync<T>(HttpResponseMessage response)
            {
                var stream = await response.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<T>(stream.ToString());
                return r as IResponse;
            }
        }

        public class ReserveResponse : IResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("reserved_ids")]
            public string[] IDs { get; set; }

            [JsonProperty("domain")]
            public string Domain { get; set; }
        }
    }
}
