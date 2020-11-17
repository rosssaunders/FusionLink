using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class DataHelper
    {
        public static object[,] ConvertDataTableToExcel(DataTable data)
        {
            int rowCount = data.Rows.Count;
            int columnCount = data.Columns.Count;

            object[,] arr = new object[rowCount + 1, columnCount];

            for (int i = 0; i < columnCount; i++)
            {
                arr[0, i] = data.Columns[i].ColumnName;
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (data.Rows[i][j] is DBNull)
                        arr[i + 1, j] = "";
                    else
                        arr[i + 1, j] = data.Rows[i][j];
                }
            }

            return arr;
        }

        public static object[,] ConvertPriceHistoryToExcel(List<PriceHistory> prices)
        {
            object[,] array = new object[prices.Count + 1, 9];

            array[0, 0] = "Date";
            array[0, 1] = "First";
            array[0, 2] = "High";
            array[0, 3] = "Low";
            array[0, 4] = "Last";
            array[0, 5] = "Theoretical";
            array[0, 6] = "Bid";
            array[0, 7] = "Ask";
            array[0, 8] = "Volume";

            for (int i = 0; i < prices.Count; i++)
            {
                array[i + 1, 0] = prices[i].Date;
                array[i + 1, 1] = prices[i].First ?? 0;
                array[i + 1, 2] = prices[i].High ?? 0;
                array[i + 1, 3] = prices[i].Low ?? 0;
                array[i + 1, 4] = prices[i].Last ?? 0;
                array[i + 1, 5] = prices[i].Theoretical ?? 0;
                array[i + 1, 6] = prices[i].Bid ?? 0;
                array[i + 1, 7] = prices[i].Ask ?? 0;
                array[i + 1, 8] = prices[i].Volume ?? 0;
            }

            return array;
        }
    }
}
