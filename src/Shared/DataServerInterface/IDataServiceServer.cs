//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RxdSolutions.FusionLink.Interface
{
    [ServiceContract(CallbackContract = typeof(IDataServiceClient), Namespace = "http://schemas.rxdsolutions.co.uk/fusionlink")]
    public interface IDataServiceServer
    {
        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void Register();

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void Unregister();

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        ServiceStatus GetServiceStatus();

        [OperationContract(IsOneWay = true)]
        void SubscribeToPositionValue(int positionId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPositionValues(List<(int positionId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioValue(int portfolioId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioValues(List<(int portfolioId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void SubscribeToSystemValue(SystemProperty systemValue);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioProperty(int portfolioId, PortfolioProperty property);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioProperties(List<(int portfolioId, PortfolioProperty property)> items);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPositionValue(int positionId, string column);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPositionValues(List<(int positionId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPortfolioValue(int portfolioId, string column);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPortfolioValues(List<(int portfolioId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromSystemValue(SystemProperty systemValue);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPortfolioProperty(int portfolioId, PortfolioProperty property);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPortfolioProperties(List<(int portfolioId, PortfolioProperty property)> items);

        [OperationContract]
        [FaultContract(typeof(PortfolioNotLoadedFaultContract))]
        [FaultContract(typeof(PortfolioNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<int> GetPositions(int portfolioId, PositionsToRequest position);

        [OperationContract(Name = "GetPriceHistoryById")]
        [FaultContract(typeof(InstrumentNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate);

        [OperationContract(Name = "GetPriceHistoryByReference")]
        [FaultContract(typeof(InstrumentNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<PriceHistory> GetPriceHistory(string reference, DateTime startDate, DateTime endDate);

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void RequestCalculate();

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void LoadPositions();

        [OperationContract(Name = "GetCurvePointsByReference")]
        [FaultContract(typeof(CurrencyNotFoundFaultContract))]
        [FaultContract(typeof(CurveFamilyNotFoundFaultContract))]
        [FaultContract(typeof(CurveNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<CurvePoint> GetCurvePoints(string curency, string family, string reference);

        [OperationContract(Name = "GetTransactionsByPosition")]
        [FaultContract(typeof(PositionNotFoundFaultContract))]
        [FaultContract(typeof(ErrorFaultContract))]
        List<Transaction> GetTransactions(int positionId, DateTime startDate, DateTime endDate);
    }
}
