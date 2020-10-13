//  Copyright (c) RXD Solutions. All rights reserved.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink
{
    [DataContract]
    public class FlatPosition
    {
        [DataMember]
        public int Instrument { get; set; }

        [DataMember]
        public int Portfolio { get; set; }

        [DataMember]
        public Dictionary<string, object> Columns { get; set; }
    }
}
