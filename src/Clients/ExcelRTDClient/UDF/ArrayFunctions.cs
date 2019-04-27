using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;
using static ExcelDna.Integration.XlCall;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class ArrayFunctions
    {
        [ExcelFunction(Name = "GETPOSITIONS", Description = "Returns a list of position ids of the given portfolio", Category = "FusionLink")]
        public static object GetPositions(int portfolioId, bool includeAll)
        {
            var positionIds = AddIn.Client.GetPositions(portfolioId, includeAll ? Positions.All : Positions.Open);

            double[,] array = new double[positionIds.Count, 1];
            for (var i = 0; i < positionIds.Count; i++)
            {
                array[i, 0] = positionIds[i];
            }

            var caller = Excel(xlfCaller) as ExcelReference;
            if (caller == null)
                return array;

            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            if (rows == 0 || columns == 0)
                return array;

            if ((caller.RowLast - caller.RowFirst + 1 == rows) &&
                (caller.ColumnLast - caller.ColumnFirst + 1 == columns))
            {
                // Size is already OK - just return result
                return array;
            }

            var rowLast = caller.RowFirst + rows - 1;
            var columnLast = caller.ColumnFirst + columns - 1;

            // Check for the sheet limits
            if (rowLast > ExcelDnaUtil.ExcelLimits.MaxRows - 1 ||
                columnLast > ExcelDnaUtil.ExcelLimits.MaxColumns - 1)
            {
                // Can't resize - goes beyond the end of the sheet - just return #VALUE
                // (Can't give message here, or change cells)
                return ExcelError.ExcelErrorValue;
            }

            // TODO: Add some kind of guard for ever-changing result?
            ExcelAsyncUtil.QueueAsMacro(() => {
                // Create a reference of the right size
                var target = new ExcelReference(caller.RowFirst, rowLast, caller.ColumnFirst, columnLast, caller.SheetId);
                ExcelRangeResizer.DoResize(target); // Will trigger a recalc by writing formula
            });

            // Return what we have - to prevent flashing #N/A
            return array;
        }
    }
}
