//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System.Runtime.Serialization;

namespace RxdSolutions.FusionLink.Interface
{
    [DataContract]
    public enum SystemProperty
    {
        [EnumMember]
        PortfolioDate,

        [EnumMember]
        InstrumentDate,

        [EnumMember]
        InstrumentCategory,

        [EnumMember]
        MarkPnLRuleSet,

        [EnumMember]
        SubscriptRedemptionDate,

        [EnumMember]
        CreditRiskDate,

        [EnumMember]
        Rate,

        [EnumMember]
        RepoDate,

        [EnumMember]
        Volatility,

        [EnumMember]
        Correlation,

        [EnumMember]
        Dividend,

        [EnumMember]
        Forex,

        [EnumMember]
        Spot,

        [EnumMember]
        MarketCategory,

        [EnumMember]
        TagMetaData,

        [EnumMember]
        Position,

        [EnumMember]
        Calculation
    }
}
