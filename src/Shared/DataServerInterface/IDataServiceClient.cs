//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System.ServiceModel;

namespace RxdSolutions.Sophis2Excel.Interface
{
    [ServiceContract]
    public interface IDataServiceClient
    {
        [OperationContract(IsOneWay = true)]
        void SendPositionValue(int positionId, string column, DataTypeEnum type, object value);

        [OperationContract(IsOneWay = true)]
        void SendPortfolioValue(int portfolioId, string column, DataTypeEnum type, object value);
    }
}
