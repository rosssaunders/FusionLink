//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.ExcelClient.Properties;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetReportFunction
    {
        [ExcelFunction(Name = "GETREPORTSQLSOURCERESULTS", 
                       Description = "Returns the results from a SQL source defined in a Report.", 
                       HelpTopic = "Get-Report-Sql-Source-Results")]
        public static object GetReportSqlSourceResults([ExcelArgument(Name = "report_name", Description = "Report name")]string reportName,
                                                       [ExcelArgument(Name = "source_name", Description = "Source name")]string sourceName)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (string.IsNullOrWhiteSpace(reportName))
                return Resources.InvalidReportName;
            
            if (string.IsNullOrWhiteSpace(sourceName))
                return Resources.InvalidSourceName;
            
            if (AddIn.Client.State != System.ServiceModel.CommunicationState.Opened)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.NotConnectedMessage));
            }

            try
            {
                var results = AddIn.Client.GetReportSqlSourceResults(reportName, sourceName);

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
