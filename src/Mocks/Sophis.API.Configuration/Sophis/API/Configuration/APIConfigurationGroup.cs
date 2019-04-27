//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sophis.API.Configuration
{
    public class APIConfigurationGroup : ConfigurationSectionGroup
    {
        public APISection API { get; set; }

        public ParametersSection Parameters { get; set; }
    }
}
