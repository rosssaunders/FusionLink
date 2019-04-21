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
        void UnRegister();

        [OperationContract(IsOneWay = true)]
        void SubscribeToPositionValue(int positionId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioValue(int folioId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolioDate();

        [OperationContract]
        List<int> GetPositions(int folioId);
    }
}
