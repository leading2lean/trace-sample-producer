//
// ConfigTests.cs: tests config file loading
// 
// Authors:
//   Corey Olsen <corey@leading2lean.com>
// 
// Copyright 2016 Leading2Lean, Inc. (www.leading2lean.com)
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using L2L.Trace;

namespace L2LTraceUnitTests
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void TestLoadConfig()
        {
            Config c = Utility.load_config_file("./config.yaml");
            Assert.AreNotEqual(c.Token, "");
            Assert.AreEqual(c.Token, "bba0c178-0210-4c6b-b23a-6d46431d67ca");
        }
    }
}
