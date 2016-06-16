//
// Config.cs: configuration data structure
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
using System.Text;
using System.Threading.Tasks;

namespace L2L
{
    namespace Trace
    {
        public class Config
        {
            public string Token { get; set; }
            public string Apiurl { get; set; }
            public Int16 Reservequantity { get; set; }
        }
    }
}
