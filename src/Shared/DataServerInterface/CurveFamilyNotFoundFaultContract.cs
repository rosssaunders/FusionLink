//  Copyright (c) RXD Solutions. All rights reserved.


using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class CurveFamilyNotFoundFaultContract
    {
        [DataMember]
        public string CurveFamily { get; set; }
    }
}