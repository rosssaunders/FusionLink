//  Copyright (c) RXD Solutions. All rights reserved.
using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class ErrorFaultContract
    {
        [DataMember]
        public string Message { get; set;  }
    }
}