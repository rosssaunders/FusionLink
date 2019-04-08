//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System.Runtime.Serialization;

namespace RxdSolutions.Sophis2Excel.Interface
{
    [DataContract(Name = "DataType")]
    public enum DataTypeEnum
    {
        [EnumMember(Value = "String")]
        String = 0,

        [EnumMember(Value = "Int64")]
        Int64 = 1,

        [EnumMember(Value = "DateTime")]
        DateTime = 2,

        [EnumMember(Value = "Double")]
        Double = 3,

        [EnumMember(Value = "Boolean")]
        Boolean = 4
    }
}
