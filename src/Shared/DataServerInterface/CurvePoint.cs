//  Copyright (c) RXD Solutions. All rights reserved.
using System.Runtime.Serialization;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    [DataContract]
    public class CurvePoint
    {
        [DataMember]
        public string Tenor { get; set; }

        [DataMember]
        public double? Rate { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public string RateCode { get; set; }

        [DataMember]
        public string PointType { get; set; }
    }
}
