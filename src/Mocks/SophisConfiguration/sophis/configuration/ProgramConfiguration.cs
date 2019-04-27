//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sophis.configuration
{
    public class ProgramConfiguration
    {
        public static ProgramConfiguration Current { get; set; }

        public Configuration Configuration { get; set; }

        public void LoadConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}
