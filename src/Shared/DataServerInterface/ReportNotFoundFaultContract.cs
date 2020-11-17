//  Copyright (c) RXD Solutions. All rights reserved.
using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class ReportNotFoundFaultContract
    {
        [DataMember]
        public string Report { get; set; }
        
        [DataMember]
        public string Source { get; set; }
    }
}