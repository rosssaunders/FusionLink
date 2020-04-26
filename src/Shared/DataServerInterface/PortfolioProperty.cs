//  Copyright (c) RXD Solutions. All rights reserved.


using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public enum PortfolioProperty
    {
        [EnumMember]
        Name,

        [EnumMember]
        FullPath,

        [EnumMember]
        ParentId,

        [EnumMember]
        Locked,

        [EnumMember]
        Comment,

        [EnumMember]
        Closed,

        [EnumMember]
        Entity,

        [EnumMember]
        Currency,

        [EnumMember]
        Security
    }
}
