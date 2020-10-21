//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DataServers : IRealTimeServer, IOnDemandServer
    {
        private readonly IRealTimeServer _realTimeServer;
        private readonly IOnDemandServer _onDemandServer;

        public DataServers(IRealTimeServer realTimeServer, IOnDemandServer onDemandServer)
        {
            _realTimeServer = realTimeServer;
            _onDemandServer = onDemandServer;
        }

        public DateTime AddBusinessDays(DateTime baseDate, int daysToAdd, string currency, CalendarType calendarType, string name)
        {
            return _onDemandServer.AddBusinessDays(baseDate, daysToAdd, currency, calendarType, name);
        }

        public List<PriceHistory> GetCurrencyPriceHistory(int currencyId, DateTime startDate, DateTime endDate)
        {
            return _onDemandServer.GetCurrencyPriceHistory(currencyId, startDate, endDate);
        }

        public List<PriceHistory> GetCurrencyPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            return _onDemandServer.GetCurrencyPriceHistory(reference, startDate, endDate);
        }

        public List<CurvePoint> GetCurvePoints(string curency, string family, string reference)
        {
            return _onDemandServer.GetCurvePoints(curency, family, reference);
        }

        public List<int> GetFlatPositions(int portfolioId, PositionsToRequest position)
        {
            return _onDemandServer.GetFlatPositions(portfolioId, position);
        }

        public List<PriceHistory> GetInstrumentPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            return _onDemandServer.GetInstrumentPriceHistory(instrumentId, startDate, endDate);
        }

        public List<PriceHistory> GetInstrumentPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            return _onDemandServer.GetInstrumentPriceHistory(reference, startDate, endDate);
        }

        public DataTable GetInstrumentSet(int instrumentId, string property)
        {
            return _onDemandServer.GetInstrumentSet(instrumentId, property);
        }

        public DataTable GetInstrumentSet(string reference, string property)
        {
            return _onDemandServer.GetInstrumentSet(reference, property);
        }

        public List<Transaction> GetPortfolioTransactions(int portfolioId, DateTime startDate, DateTime endDate)
        {
            return _onDemandServer.GetPortfolioTransactions(portfolioId, startDate, endDate);
        }

        public List<int> GetPositions(int portfolioId, PositionsToRequest position)
        {
            return _onDemandServer.GetPositions(portfolioId, position);
        }

        public List<Transaction> GetPositionTransactions(int positionId, DateTime startDate, DateTime endDate)
        {
            return _onDemandServer.GetPositionTransactions(positionId, startDate, endDate);
        }

        public ServiceStatus GetServiceStatus()
        {
            return _realTimeServer.GetServiceStatus();
        }

        public void LoadPositions()
        {
            _realTimeServer.LoadPositions();
        }

        public void Register()
        {
            _realTimeServer.Register();
        }

        public void RequestCalculate()
        {
            _realTimeServer.RequestCalculate();
        }

        public void SubscribeToFlatPositionValue(int portgolioId, int instrumentId, string column)
        {
            _realTimeServer.SubscribeToFlatPositionValue(portgolioId, instrumentId, column);
        }

        public void SubscribeToFlatPositionValues(List<(int portfolioId, int instrumentId, string column)> items)
        {
            _realTimeServer.SubscribeToFlatPositionValues(items);
        }

        public void SubscribeToInstrumentProperties(List<(object Id, string Property)> list)
        {
            _realTimeServer.SubscribeToInstrumentProperties(list);
        }

        public void SubscribeToInstrumentProperty(object instrument, string propertyName)
        {
            _realTimeServer.SubscribeToInstrumentProperty(instrument, propertyName);
        }

        public void SubscribeToPortfolioProperties(List<(int portfolioId, PortfolioProperty property)> items)
        {
            _realTimeServer.SubscribeToPortfolioProperties(items);
        }

        public void SubscribeToPortfolioProperty(int portfolioId, PortfolioProperty propertyName)
        {
            _realTimeServer.SubscribeToPortfolioProperty(portfolioId, propertyName);
        }

        public void SubscribeToPortfolioValue(int portfolioId, string column)
        {
            _realTimeServer.SubscribeToPortfolioValue(portfolioId, column);
        }

        public void SubscribeToPortfolioValues(List<(int portfolioId, string column)> items)
        {
            _realTimeServer.SubscribeToPortfolioValues(items);
        }

        public void SubscribeToPositionValue(int positionId, string column)
        {
            _realTimeServer.SubscribeToPositionValue(positionId, column);
        }

        public void SubscribeToPositionValues(List<(int positionId, string column)> items)
        {
            _realTimeServer.SubscribeToPositionValues(items);
        }

        public void SubscribeToSystemValue(SystemProperty systemValue)
        {
            _realTimeServer.SubscribeToSystemValue(systemValue);
        }

        public void Unregister()
        {
            _realTimeServer.Unregister();
        }

        public void UnsubscribeFromFlatPositionValue(int portfolioId, int instrumentId, string column)
        {
            _realTimeServer.UnsubscribeFromFlatPositionValue(portfolioId, instrumentId, column);
        }

        public void UnsubscribeFromFlatPositionValues(List<(int portfolioId, int instrumentId, string column)> items)
        {
            _realTimeServer.UnsubscribeFromFlatPositionValues(items);
        }

        public void UnsubscribeFromInstrumentProperties(List<(object Id, string Property)> list)
        {
            _realTimeServer.UnsubscribeFromInstrumentProperties(list);
        }

        public void UnsubscribeFromInstrumentProperty(object instrument, string propertyName)
        {
            _realTimeServer.UnsubscribeFromInstrumentProperty(instrument, propertyName);
        }

        public void UnsubscribeFromPortfolioProperties(List<(int portfolioId, PortfolioProperty property)> items)
        {
            _realTimeServer.UnsubscribeFromPortfolioProperties(items);
        }

        public void UnsubscribeFromPortfolioProperty(int portfolioId, PortfolioProperty propertyName)
        {
            _realTimeServer.UnsubscribeFromPortfolioProperty(portfolioId, propertyName);
        }

        public void UnsubscribeFromPortfolioValue(int portfolioId, string column)
        {
            _realTimeServer.UnsubscribeFromPortfolioValue(portfolioId, column);
        }

        public void UnsubscribeFromPortfolioValues(List<(int portfolioId, string column)> items)
        {
            _realTimeServer.UnsubscribeFromPortfolioValues(items);
        }

        public void UnsubscribeFromPositionValue(int positionId, string column)
        {
            _realTimeServer.UnsubscribeFromPositionValue(positionId, column);
        }

        public void UnsubscribeFromPositionValues(List<(int positionId, string column)> items)
        {
            _realTimeServer.UnsubscribeFromPositionValues(items);
        }

        public void UnsubscribeFromSystemValue(SystemProperty systemValue)
        {
            _realTimeServer.UnsubscribeFromSystemValue(systemValue);
        }
    }
}