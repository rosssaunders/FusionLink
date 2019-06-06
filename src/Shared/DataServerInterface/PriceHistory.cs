//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class PriceHistory
    {
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public double ? Ask { get; set; }

        [DataMember]
        public double ? Bid { get; set; }

        [DataMember]
        public double ? First { get; set; }

        [DataMember]
        public double ? High { get; set; }

        [DataMember]
        public double ? Last { get; set; }

        [DataMember]
        public double ? Low { get; set; }

        [DataMember]
        public double ? Volume { get; set; }

        [DataMember]
        public double ? Theoretical { get; set; }
    }
}
