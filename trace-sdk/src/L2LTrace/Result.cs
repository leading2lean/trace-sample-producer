//
// Result.cs: response returned from API service calls
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
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace L2L
{
    namespace Trace
    {
        public interface IResponse
        {
        };

        public class Result
        {
            private bool _success = false;
            private string _message = "";
            private HttpStatusCode _http_code = 0;
            private IResponse _response = null;

            public bool Success
            {
                get { return _success; }
                set { _success = value; }
            }

            public string Message
            {
                get { return _message; }
                set { _message = value; }
            }

            public HttpStatusCode HttpCode
            {
                get { return _http_code; }
                set { _http_code = value; }
            }

            public IResponse Response
            {
                get { return _response; }
                set { _response = value; }
            }
        }

        public class ErrorResponse : IResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error")]
            public string Error { get; set; }
        }
    }
}
