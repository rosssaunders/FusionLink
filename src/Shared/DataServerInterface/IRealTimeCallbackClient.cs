//  Copyright (c) RXD Solutions. All rights reserved.
using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace RxdSolutions.FusionLink.Interface
{
    [ServiceContract]
    public interface IRealTimeCallbackClient
    {
        [OperationContract(IsOneWay = true)]
        void SendServiceStaus(ServiceStatus status);

        [OperationContract(IsOneWay = true)]
        void SendPositionValue(int positionId, string column, object value);

        [OperationContract(IsOneWay = true)]
        void SendFlatPositionValue(int portfolioId, int instrumentId, string column, object value);

        [OperationContract(IsOneWay = true)]
        void SendPortfolioValue(int portfolioId, string column, object value);

        [OperationContract(IsOneWay = true)]
        void SendPortfolioProperty(int id, PortfolioProperty propertyName, object value);

        [OperationContract(IsOneWay = true)]
        void SendInstrumentProperty(object id, string propertyName, object value);

        [OperationContract(IsOneWay = true)]
        void SendSystemValue(SystemProperty propertyName, object value);

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void Heartbeat();
    }
}
