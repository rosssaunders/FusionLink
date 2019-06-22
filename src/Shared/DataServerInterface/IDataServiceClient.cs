//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace RxdSolutions.FusionLink.Interface
{
    [ServiceContract]
    public interface IDataServiceClient
    {
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(ErrorFaultContract))]
        void SendServiceStaus(ServiceStatus status);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(ErrorFaultContract))]
        void SendPositionValue(int positionId, string column, object value);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(ErrorFaultContract))]
        void SendPortfolioValue(int portfolioId, string column, object value);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(ErrorFaultContract))]
        void SendPortfolioProperty(int id, PortfolioProperty property, object value);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(ErrorFaultContract))]
        void SendSystemValue(SystemProperty property, object value);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(ErrorFaultContract))]
        void Heartbeat();
    }
}
