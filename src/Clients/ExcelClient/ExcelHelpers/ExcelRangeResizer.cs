//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using ExcelDna.Integration;
using static ExcelDna.Integration.XlCall;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class ExcelRangeResizer
    {
        public static void DoResize(ExcelReference target)
        {
            // Get the current state for reset later
            using (new ExcelEchoOffHelper())
            using (new ExcelCalculationManualHelper())
            {
                ExcelReference firstCell = new ExcelReference(target.RowFirst, target.RowFirst, target.ColumnFirst, target.ColumnFirst, target.SheetId);

                // Get the formula in the first cell of the target
                string formula = (string)Excel(xlfGetCell, 41, firstCell);
                bool isFormulaArray = (bool)Excel(xlfGetCell, 49, firstCell);
                if (isFormulaArray)
                {
                    // Select the sheet and firstCell - needed because we want to use SelectSpecial.
                    using (new ExcelSelectionHelper(firstCell))
                    {
                        // Extend the selection to the whole array and clear
                        Excel(xlcSelectSpecial, 6);
                        ExcelReference oldArray = (ExcelReference)Excel(xlfSelection);

                        oldArray.SetValue(ExcelEmpty.Value);
                    }
                }
                // Get the formula and convert to R1C1 mode
                bool isR1C1Mode = (bool)Excel(xlfGetWorkspace, 4);
                string formulaR1C1 = formula;
                if (!isR1C1Mode)
                {
                    object formulaR1C1Obj;
                    XlReturn formulaR1C1Return = TryExcel(xlfFormulaConvert, out formulaR1C1Obj, formula, true, false, ExcelMissing.Value, firstCell);
                    if (formulaR1C1Return != XlReturn.XlReturnSuccess || formulaR1C1Obj is ExcelError)
                    {
                        string firstCellAddress = (string)Excel(xlfReftext, firstCell, true);
                        Excel(xlcAlert, "Cannot resize array formula at " + firstCellAddress + " - formula might be too long when converted to R1C1 format.");
                        firstCell.SetValue("'" + formula);
                        return;
                    }
                    formulaR1C1 = (string)formulaR1C1Obj;
                }
                // Must be R1C1-style references
                object ignoredResult;
                //Debug.Print("Resizing START: " + target.RowLast);
                XlReturn formulaArrayReturn = TryExcel(xlcFormulaArray, out ignoredResult, formulaR1C1, target);
                //Debug.Print("Resizing FINISH");

                // TODO: Find some dummy macro to clear the undo stack

                if (formulaArrayReturn != XlReturn.XlReturnSuccess)
                {
                    string firstCellAddress = (string)Excel(xlfReftext, firstCell, true);
                    Excel(xlcAlert, "Cannot resize array formula at " + firstCellAddress + " - result might overlap another array.");
                    // Might have failed due to array in the way.
                    firstCell.SetValue("'" + formula);
                }
            }
        }
    }
}