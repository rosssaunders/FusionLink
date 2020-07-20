//  Copyright (c) RXD Solutions. All rights reserved.
using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class CalendarNotFoundFaultContract
    {
        [DataMember]
        public string Calendar { get; set; }
    }
}