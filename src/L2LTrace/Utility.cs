//
// Utility.cs: used to load configuration file from disk
// 
// Authors:
//   Corey Olsen <corey@leading2lean.com>
// 
// Copyright 2016 Leading2Lean, Inc. (www.leading2lean.com)
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using L2L.Trace;

namespace L2L
{
    namespace Trace
    {
        public class Utility
        {
            public static Config load_config_file(string filename)
            {
                Config conf = null;
                try
                {
                    var stream = new StreamReader(filename);
                    var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                    conf = deserializer.Deserialize<Config>(stream);
                }
                catch (Exception)
                {
                    conf = new Config();
                }
                return conf;
            }
        }
    }
}
