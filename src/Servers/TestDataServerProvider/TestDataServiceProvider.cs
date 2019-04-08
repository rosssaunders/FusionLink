//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using RxdSolutions;
using RxdSolutions.Sophis2Excel.Interface;

namespace TestDataServer
{
    public class TestDataServiceProvider : IDataServiceProvider
    {
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static double RandomDouble()
        {
            return random.NextDouble();
        }

        public static int RandomInt()
        {
            return random.Next();
        }

        readonly Dictionary<string, Func<string>> stringColumns = new Dictionary<string, Func<string>>();
        readonly Dictionary<string, Func<double>> doubleColumns = new Dictionary<string, Func<double>>();
        readonly Dictionary<string, Func<int>> intColumns = new Dictionary<string, Func<int>>();

        public bool IsBusy => false;

        public TestDataServiceProvider()
        {
            stringColumns.Add("Reference", () => RandomString(23));

            doubleColumns.Add("Theoretical", RandomDouble);

            intColumns.Add("Number Of Securities", RandomInt);
        }

        public (DataTypeEnum dataType, object value) GetPortfolioValue(int portfolioId, string column)
        {
            if(stringColumns.ContainsKey(column))
            {
                return (DataTypeEnum.String, stringColumns[column]());
            }

            if (doubleColumns.ContainsKey(column))
            {
                return (DataTypeEnum.Double, doubleColumns[column]());
            }

            if (intColumns.ContainsKey(column))
            {
                return (DataTypeEnum.Int64, intColumns[column]());
            }

            return (DataTypeEnum.Double, RandomDouble());
        }

        public (DataTypeEnum dataType, object value) GetPositionValue(int positionId, string column)
        {
            if (stringColumns.ContainsKey(column))
            {
                return (DataTypeEnum.String, stringColumns[column]());
            }

            if (doubleColumns.ContainsKey(column))
            {
                return (DataTypeEnum.Double, doubleColumns[column]());
            }

            if (intColumns.ContainsKey(column))
            {
                return (DataTypeEnum.Int64, intColumns[column]());
            }

            return (DataTypeEnum.Double, RandomDouble());
        }
    }
}
