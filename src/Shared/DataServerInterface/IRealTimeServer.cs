//  Copyright (c) RXD Solutions. All rights reserved.
using System.Collections.Generic;
using System.ServiceModel;

namespace RxdSolutions.FusionLink.Interface
{
    [ServiceContract(CallbackContract = typeof(IRealTimeCallbackClient), Namespace = "http://schemas.rxdsolutions.co.uk/fusionlink")]
    public interface IRealTimeServer
    {
        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void Register();

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void Unregister();

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void RequestCalculate();

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void LoadPositions();

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        ServiceStatus GetServiceStatus();

        [OperationContract(IsOneWay = true)]
        void SubscribeToPositionValue(int positionId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPositionValues(List<(int positionId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void SubscribeToFlatPositionValue(int portgolioId, int instrumentId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToFlatPositionValues(List<(int portfolioId, int instrumentId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioValue(int portfolioId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioValues(List<(int portfolioId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void SubscribeToSystemValue(SystemProperty systemValue);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioProperty(int portfolioId, PortfolioProperty propertyName);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioProperties(List<(int portfolioId, PortfolioProperty property)> items);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPositionValue(int positionId, string column);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPositionValues(List<(int positionId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromFlatPositionValue(int portfolioId, int instrumentId, string column);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromFlatPositionValues(List<(int portfolioId, int instrumentId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPortfolioValue(int portfolioId, string column);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPortfolioValues(List<(int portfolioId, string column)> items);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromSystemValue(SystemProperty systemValue);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPortfolioProperty(int portfolioId, PortfolioProperty propertyName);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromPortfolioProperties(List<(int portfolioId, PortfolioProperty property)> items);

        [OperationContract(IsOneWay = true)]
        void SubscribeToInstrumentProperty(object instrument, string propertyName);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromInstrumentProperty(object instrument, string propertyName);

        [OperationContract(IsOneWay = true)]
        void SubscribeToInstrumentProperties(List<(object Id, string Property)> list);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromInstrumentProperties(List<(object Id, string Property)> list);

        [OperationContract(IsOneWay = true)]
        void SubscribeToCurrencyProperty(object instrument, string propertyName);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromCurrencyProperty(object instrument, string propertyName);

        [OperationContract(IsOneWay = true)]
        void SubscribeToCurrencyProperties(List<(object Id, string Property)> list);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromCurrencyProperties(List<(object Id, string Property)> list);
    }
}
