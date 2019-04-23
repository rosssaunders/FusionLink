//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using RxdSolutions;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public class TestDataServiceProvider : IDataServerProvider
    {
        private static readonly Random random = new Random();

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

        public object GetPortfolioValue(int portfolioId, string column)
        {
            if(stringColumns.ContainsKey(column))
            {
                return stringColumns[column]();
            }

            if (doubleColumns.ContainsKey(column))
            {
                return doubleColumns[column]();
            }

            if (intColumns.ContainsKey(column))
            {
                return intColumns[column]();
            }

            return RandomDouble();
        }

        public object GetPositionValue(int positionId, string column)
        {
            if (stringColumns.ContainsKey(column))
            {
                return stringColumns[column]();
            }

            if (doubleColumns.ContainsKey(column))
            {
                return doubleColumns[column]();
            }

            if (intColumns.ContainsKey(column))
            {
                return intColumns[column]();
            }

            return RandomDouble();
        }

        public DateTime GetPortfolioDate()
        {
            return DateTime.Today;
        }

        public List<int> GetPositions(int folioId)
        {
            return new List<int>() { 1, 2, 3, 4, 5 };
        }

        public void GetPositionValues(IDictionary<(int positionId, string column), object> values)
        {
            throw new NotImplementedException();
        }

        public void GetPortfolioValues(IDictionary<(int positionId, string column), object> values)
        {
            throw new NotImplementedException();
        }
    }
}
