//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public enum ServiceStatus
    {
        [EnumMember]
        Started,

        [EnumMember]
        Stopped,

        [EnumMember]
        NotConnected
    }
}
