//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

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
        List<int> GetPositions(int folioId);
    }
}
