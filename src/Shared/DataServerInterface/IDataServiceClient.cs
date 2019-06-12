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
        [OperationContract(IsOneWay = true)]
        void SendServiceStaus(ServiceStatus status);

        [OperationContract(IsOneWay = true)]
        void SendPositionValue(int positionId, string column, object value);

        [OperationContract(IsOneWay = true)]
        void SendPortfolioValue(int portfolioId, string column, object value);

        [OperationContract(IsOneWay = true)]
        void SendPortfolioProperty(int id, PortfolioProperty property, object value);

        [OperationContract(IsOneWay = true)]
        void SendSystemValue(SystemProperty property, object value);

        [OperationContract(IsOneWay = true)]
        void Heartbeat();
    }
}
