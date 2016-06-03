//
// TraceRecordTest.cs
// 
// Authors:
//   Corey Olsen <corey@leading2lean.com>
// 
// Copyright 2016 Leading2Lean, Inc. (www.leading2lean.com)
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using L2L.Trace;


namespace L2LTraceUnitTests
{
    // [DataContract]
    public class ExternalID : IRecord
    {
        // [DataMember(Name = "PartNumber")]
        public string PartNumber { get; set; }
        // [DataMember(Name = "SerialNumber")]
        public string SerialNumber { get; set; }

        // static constructor
        /*static ExternalID()
        {
            IRecord.DerivedTypes.Add(typeof(ExternalID)); 
        }*/
    }

    [TestClass]
    public class TraceRecordTest
    {
        [TestMethod]
        public void TestBasicCreate()
        {
            Config c = Utility.load_config_file("./config.yaml");
            var tr = new TraceRecord();
            tr.Typecode = "asdf1234";

            var exid = new ExternalID();
            exid.PartNumber = "asdf";
            exid.SerialNumber = "1234";
            tr.ExternalIds.Add(exid as IRecord);

            var result = tr.CreateAsync(c.Apiurl, c.Token);
            result.Wait();

            var r = result.Result;
            Assert.AreEqual(true, r.Success);
        }
    }
}
