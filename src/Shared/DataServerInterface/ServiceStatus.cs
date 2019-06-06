//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

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
