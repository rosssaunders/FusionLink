//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RxdSolutions.FusionLink.Interface
{
    [ServiceContract(Namespace = "http://schemas.rxdsolutions.co.uk/fusionlink")]
    public interface IOnDemandServer
    {
        [OperationContract]
        [FaultContract(typeof(PortfolioNotLoadedFaultContract))]
        [FaultContract(typeof(PortfolioNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<int> GetPositions(int portfolioId, PositionsToRequest position);

        [OperationContract]
        [FaultContract(typeof(PortfolioNotLoadedFaultContract))]
        [FaultContract(typeof(PortfolioNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<int> GetFlatPositions(int portfolioId, PositionsToRequest position);

        [OperationContract(Name = "GetInstrumentPriceHistoryById")]
        [FaultContract(typeof(InstrumentNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<PriceHistory> GetInstrumentPriceHistory(int instrumentId, DateTime startDate, DateTime endDate);

        [OperationContract(Name = "GetInstrumentPriceHistoryByReference")]
        [FaultContract(typeof(InstrumentNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<PriceHistory> GetInstrumentPriceHistory(string reference, DateTime startDate, DateTime endDate);

        [OperationContract(Name = "GetCurrencyPriceHistoryById")]
        [FaultContract(typeof(CurrencyNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<PriceHistory> GetCurrencyPriceHistory(int currencyId, DateTime startDate, DateTime endDate);

        [OperationContract(Name = "GetCurrencyPriceHistoryByReference")]
        [FaultContract(typeof(CurrencyNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<PriceHistory> GetCurrencyPriceHistory(string reference, DateTime startDate, DateTime endDate);

        [OperationContract(Name = "GetCurvePointsByReference")]
        [FaultContract(typeof(CurrencyNotFoundFaultContract))]
        [FaultContract(typeof(CurveFamilyNotFoundFaultContract))]
        [FaultContract(typeof(CurveNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<CurvePoint> GetCurvePoints(string curency, string family, string reference);

        [OperationContract(Name = "GetPositionTransactions")]
        [FaultContract(typeof(PositionNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<Transaction> GetPositionTransactions(int positionId, DateTime startDate, DateTime endDate);

        [OperationContract(Name = "GetPortfolioTransactions")]
        [FaultContract(typeof(PortfolioNotFoundFaultContract))]
        [FaultContract(typeof(PortfolioNotLoadedFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<Transaction> GetPortfolioTransactions(int portfolioId, DateTime startDate, DateTime endDate);

        [OperationContract(Name = "AddBusinessDays")]
        [FaultContract(typeof(CalendarNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        DateTime AddBusinessDays(DateTime baseDate, int daysToAdd, string currency, CalendarType calendarType, string name);
    }
}
