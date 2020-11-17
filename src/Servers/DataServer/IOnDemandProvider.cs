//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Data;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public interface IOnDemandProvider
    {
        List<int> GetPositions(int portfolioId, PositionsToRequest positions);

        List<int> GetFlatPositions(int portfolioId, PositionsToRequest positions);

        List<PriceHistory> GetInstrumentPriceHistory(int instrumentId, DateTime startDate, DateTime endDate);

        List<PriceHistory> GetInstrumentPriceHistory(string reference, DateTime startDate, DateTime endDate);

        List<PriceHistory> GetCurrencyPriceHistory(int currencyId, DateTime startDate, DateTime endDate);

        List<PriceHistory> GetCurrencyPriceHistory(string currency, DateTime startDate, DateTime endDate);

        List<CurvePoint> GetCurvePoints(string currency, string family, string reference);

        List<Transaction> GetPositionTransactions(int positionId, DateTime startDate, DateTime endDate);

        List<Transaction> GetPortfolioTransactions(int portfolioId, DateTime startDate, DateTime endDate);

        DateTime AddBusinessDays(DateTime date, int daysToAdd, string calendar, CalendarType calendarType, string currency);
        
        DataTable GetInstrumentSet(int instrumentId, string property);
     
        DataTable GetInstrumentSet(string reference, string property);
        
        DataTable GetReportSqlSourceResults(string reportName, string sourceName);
        
        DataTable GetCurrencySet(int currencyId, string property);
        
        DataTable GetCurrencySet(string reference, string property);
    }
}
