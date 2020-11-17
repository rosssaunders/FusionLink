//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetInstrumentPropertyFunctions
    {
        [ExcelFunction(Name = "GETINSTRUMENTPROPERTY",
                       Description = "Returns an Instrument Property",
                       HelpTopic = "Get-Instrument-Property")]
        public static object GetInstrumentProperty(
            [ExcelArgument(Name = "instrument", Description = "The instrument id or reference")]object instrument,
            [ExcelArgument(Name = "property", Description = "The instrument property to subscribe to")]string property)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            return ExcelAsyncUtil.Observe(nameof(GetInstrumentProperty), new object[] { instrument, property }, () => new InstrumentPropertyExcelObservable(instrument, property, AddIn.Client));
        }

        [ExcelFunction(Name = "GETINSTRUMENTSET",
               Description = "Returns an Instrument Set",
               HelpTopic = "Get-Instrument-Set")]
        public static object GetInstrumentSet(
            [ExcelArgument(Name = "instrument", Description = "The instrument id or reference")] object instrument,
            [ExcelArgument(Name = "property", Description = "The instrument property to request")] string property)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (AddIn.Client.State != System.ServiceModel.CommunicationState.Opened)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.NotConnectedMessage));
            }

            try
            {
                DataTable results;
                if (instrument is double)
                {
                    results = AddIn.Client.GetInstrumentSet(Convert.ToInt32(instrument), property);
                }
                else
                {
                    results = AddIn.Client.GetInstrumentSet((string)instrument, property);
                }

                if (results.Rows.Count == 0)
                {
                    return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.ExcelEmptyRange);
                }
                else
                {
                    object[,] array = DataHelper.ConvertDataTableToExcel(results);

                    return ExcelRangeResizer.TransformToExcelRange(array);
                }
            }
            catch (Exception ex)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(ex.Message));
            }
        }
    }
}
