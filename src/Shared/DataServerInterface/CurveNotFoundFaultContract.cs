//  Copyright (c) RXD Solutions. All rights reserved.


using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class CurveNotFoundFaultContract
    {
        [DataMember]
        public string Curve { get; set; }
    }
}