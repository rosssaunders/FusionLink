//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    // Select an ExcelReference (perhaps on another sheet) allowing changes to be made there.
    // On clean-up, resets all the selections and the active sheet.
    // Should not be used if the work you are going to do will switch sheets, amke new sheets etc.
    public sealed class ExcelSelectionHelper : XlCall, IDisposable
    {
        readonly object oldSelectionOnActiveSheet;
        readonly object oldActiveCellOnActiveSheet;
        readonly object oldSelectionOnRefSheet;
        readonly object oldActiveCellOnRefSheet;

        public ExcelSelectionHelper(ExcelReference refToSelect)
        {
            // Remember old selection state on the active sheet
            oldSelectionOnActiveSheet = Excel(xlfSelection);
            oldActiveCellOnActiveSheet = Excel(xlfActiveCell);

            // Switch to the sheet we want to select
            string refSheet = (string)Excel(xlSheetNm, refToSelect);
            Excel(xlcWorkbookSelect, new object[] { refSheet });

            // record selection and active cell on the sheet we want to select
            oldSelectionOnRefSheet = Excel(xlfSelection);
            oldActiveCellOnRefSheet = Excel(xlfActiveCell);

            // make the selection
            Excel(xlcFormulaGoto, refToSelect);
        }

        public void Dispose()
        {
            // Reset the selection on the target sheet
            Excel(xlcSelect, oldSelectionOnRefSheet, oldActiveCellOnRefSheet);

            // Reset the sheet originally selected
            string oldActiveSheet = (string)Excel(xlSheetNm, oldSelectionOnActiveSheet);
            Excel(xlcWorkbookSelect, new object[] { oldActiveSheet });

            // Reset the selection in the active sheet (some bugs make this change sometimes too)
            Excel(xlcSelect, oldSelectionOnActiveSheet, oldActiveCellOnActiveSheet);
        }
    }
}