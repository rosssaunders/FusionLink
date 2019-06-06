//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RxdSolutions.FusionLink.Interface
{
    [ServiceContract(CallbackContract = typeof(IDataServiceClient))]
    public interface IDataServiceServer
    {
        [OperationContract(IsOneWay = true)]
        void Register();

        [OperationContract(IsOneWay = true)]
        void Unregister();

        [OperationContract]
        ServiceStatus GetServiceStatus();

        [OperationContract(IsOneWay = true)]
        void SubscribeToPositionValue(int positionId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioValue(int folioId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToSystemValue(SystemProperty systemValue);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeToPositionValue(int positionId, string column);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeToPortfolioValue(int folioId, string column);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeToSystemValue(SystemProperty systemValue);

        [OperationContract]
        [FaultContract(typeof(PortfolioNotLoadedFaultContract))]
        [FaultContract(typeof(PortfolioNotFoundFaultContract))]
        List<int> GetPositions(int folioId, PositionsToRequest position);

        [OperationContract]
        [FaultContract(typeof(InstrumentNotFoundFaultContract))]
        List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate);

        [OperationContract]
        void RequestCalculate();

        [OperationContract]
        void LoadPositions();
    }
}
