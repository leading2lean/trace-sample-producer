//
// ReserveTest.cs
// 
// Authors:
//   Corey Olsen <corey@leading2lean.com>
// 
// Copyright 2016 Leading2Lean, Inc. (www.leading2lean.com)
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using L2L.Trace;
using Moq;
using Moq.Protected;

namespace L2LTraceUnitTests
{

    /// <summary>
    /// This is the most basic test of the API SDK. In this test we ensure that there is a connection
    /// that can be made to the server setup in the config.yaml file and that everything is responding
    /// correctly. If in your use of the SDK this call doesn't work something basic is missing.
    /// </summary>
    [TestClass]
    public class ReserveTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            Service.ClientHandler = null;
        }

        [TestMethod]
        public void TestReserve()
        {
            Config c = Utility.load_config_file("./config.yaml");
            var route = string.Format("{0}{1}?_auth={2}&quantity={3}", c.Apiurl, "_/reserve", c.Token, 1000);
            Uri requestUri = new Uri(route);

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for(var x = 0; x < 1000; x++)
            {
                sb.Append("\"");
                sb.Append(x);
                sb.Append("\"");
                if (x < 999)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("]");
            string expectedResponse = @"{""success"": true, ""domain"": ""bluesun"", ""reserved_ids"": " + sb.ToString() + "}";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(expectedResponse) };

            Mock<HttpClientHandler> mockHandler = new Mock<HttpClientHandler>();
            // mockHandler.Setup
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(message => message.RequestUri == requestUri), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            
            // var fakeResponseHandler = new FakeResponseHandler();
            // fakeResponseHandler.AddFakeResponse(new Uri(route), responseMessage);
            // Service.ClientHandler = fakeResponseHandler;
            Service.ClientHandler = mockHandler.Object;
            var result = Service.ReserveAsync(c);
            result.Wait();
            var r = result.Result;
            Assert.AreEqual(true, r.Success);

            var reserve_response = r.Response as ReserveResponse;
            Assert.AreEqual(true, reserve_response.Success);
            Assert.AreEqual(1000, reserve_response.IDs.Length);

            Service.ClientHandler = null;
        }

        [TestMethod]
        public void TestReserveBadData()
        {
            Config c = Utility.load_config_file("./config.yaml");

            // Intentionally mess with the url to make bad things happen and ensure the sdk keeps on keeping on
            c.Reservequantity = 1;
            var result = Service.ReserveAsync(c);
            result.Wait();
            var r = result.Result;
            Assert.AreEqual(false, r.Success);
            Assert.AreEqual(HttpStatusCode.BadRequest, r.HttpCode);

            var error_response = r.Response as ErrorResponse;
            Assert.AreEqual("Reserve quantities must be a multiple of 1000", error_response.Error);
            Assert.AreEqual(false, error_response.Success);
        }
    }
}