//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System.ServiceModel;

namespace RxdSolutions.Sophis2Excel.Interface
{
    [ServiceContract(CallbackContract = typeof(IDataServiceClient))]
    public interface IDataServiceServer
    {
        [OperationContract(IsOneWay = true)]
        void Register();

        [OperationContract(IsOneWay = true)]
        void UnRegister();

        [OperationContract(IsOneWay = true)]
        void SubscribeToPosition(int positionId, string column);

        [OperationContract(IsOneWay = true)]
        void SubscribeToPortfolio(int folioId, string column);
    }
}
