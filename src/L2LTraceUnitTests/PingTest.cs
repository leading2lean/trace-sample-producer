//
// PingTest.cs
// 
// Authors:
//   Corey Olsen <corey@leading2lean.com>
// 
// Copyright 2016 Leading2Lean, Inc. (www.leading2lean.com)
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using L2L.Trace;

namespace L2LTraceUnitTests
{
    /// <summary>
    /// This is the most basic test of the API SDK. In this test we ensure that there is a connection
    /// that can be made to the server setup in the config.yaml file and that everything is responding
    /// correctly. If in your use of the SDK this call doesn't work something basic is missing.
    /// </summary>
    [TestClass]
    public class PingTest
    {
        [TestMethod]
        public void TestPing()
        {
            Config c = Utility.load_config_file("./config.yaml");
            var result = Service.PingAsync(c);
            result.Wait();
            var r = result.Result;
            Assert.AreEqual(true, r.Success);

            var ping_response = r.Response as PingResponse;
            Assert.AreEqual("OK", ping_response.Status);
        }

        [TestMethod]
        public void TestPingNoServer()
        {
            Config c = Utility.load_config_file("./config.yaml");

            // Intentionally mess with the url to make bad things happen and ensure the sdk keeps on keeping on
            c.Apiurl = "http://bluesuns.trcld.com";
            var result = Service.PingAsync(c);
            result.Wait();
            var r = result.Result;
            Assert.AreEqual(false, r.Success);
            Assert.AreEqual((System.Net.HttpStatusCode)0, r.HttpCode);
        }
    }
}
