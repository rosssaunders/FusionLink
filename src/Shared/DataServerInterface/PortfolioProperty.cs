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
        ParentId
    }
}
