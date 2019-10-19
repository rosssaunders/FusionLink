//  Copyright (c) RXD Solutions. All rights reserved.


using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class InstrumentNotFoundFaultContract
    {
        [DataMember]
        public string Instrument { get; set; }
    }
}