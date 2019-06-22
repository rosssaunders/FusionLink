//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System.Runtime.Serialization;

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
