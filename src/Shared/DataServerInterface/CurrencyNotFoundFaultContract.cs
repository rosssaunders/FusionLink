//  Copyright (c) RXD Solutions. All rights reserved.


using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public class CurrencyNotFoundFaultContract
    {
        [DataMember]
        public string Currency { get; set; }
    }
}