//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using System.Linq;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public class TestDataServiceProvider : IRealTimeProvider
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

        public event EventHandler<DataAvailableEventArgs> DataAvailable;

        public bool IsBusy => false;

        public TimeSpan ElapsedTimeOfLastCall => throw new NotImplementedException();

        public bool IsRunning => throw new NotImplementedException();

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

        public void GetPositionValues(IDictionary<(int positionId, string column), object> values)
        {
            throw new NotImplementedException();
        }

        public void GetPortfolioValues(IDictionary<(int positionId, string column), object> values)
        {
            throw new NotImplementedException();
        }

        public object GetSystemValue(SystemProperty property)
        {
            throw new NotImplementedException();
        }

        public void GetSystemValues(IDictionary<SystemProperty, object> values)
        {
            throw new NotImplementedException();
        }

        public List<int> GetPositions(int folioId, PositionsToRequest positions)
        {
            return new List<int>() { 1, 2, 3, 4, 5 };
        }

        public void SubscribeToPortfolio(int portfolioId, string column)
        {
            throw new NotImplementedException();
        }

        public void SubscribeToPosition(int positionId, string column)
        {
            throw new NotImplementedException();
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromPortfolio(int portfolioId, string column)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromPosition(int positionId, string column)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromSystemValue(SystemProperty property)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool GetPositions(int folioId, PositionsToRequest positions, out List<int> results)
        {
            throw new NotImplementedException();
        }

        public void ComputePortfolios(int skipPortfolio)
        {
            throw new NotImplementedException();
        }

        public bool GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate, out List<PriceHistory> results)
        {
            throw new NotImplementedException();
        }

        public void RequestCalculate()
        {
            throw new NotImplementedException();
        }

        public void LoadPositions()
        {
            throw new NotImplementedException();
        }

        public List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public List<PriceHistory> GetPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public List<CurvePoint> GetCurvePoints(string currency, string family, string reference)
        {
            throw new NotImplementedException();
        }

        public void SubscribeToPortfolioProperty(int id, PortfolioProperty property)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromPortfolioProperty(int id, PortfolioProperty property)
        {
            throw new NotImplementedException();
        }

        public List<Transaction> GetPositionTransactions(int positionId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public List<Transaction> GetPortfolioTransactions(int portfolioId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public DateTime AddBusinessDays(DateTime date, int daysToAdd, string calendar, CalendarType calendarType, string currency)
        {
            throw new NotImplementedException();
        }

        public void SubscribeToInstrumentProperty(object id, string property)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromInstrumentProperty(object id, string property)
        {
            throw new NotImplementedException();
        }

        public void SubscribeToFlatPosition(int portfolioId, int instrumentId, string column)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromFlatPosition(int portfolioId, int instrumentId, string column)
        {
            throw new NotImplementedException();
        }
    }
}
