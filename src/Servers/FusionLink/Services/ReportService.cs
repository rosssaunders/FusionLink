//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using RxdSolutions.FusionLink.Interface;
using sophis.portfolio;

namespace RxdSolutions.FusionLink.Services
{
    public class ReportService
    {
        public DataTable GetReportSqlSourceResults(string reportName, string sourceName)
        {
            using (var mgr = sophis.reporting.CSMReportTemplateManager.GetInstance())
            using (var report = mgr.GetReportTemplateWithName(reportName))
            {
                var parameters = report.GetParametersList();
                if (parameters.Count > 0)
                    throw new InvalidOperationException("Cannot run a report which requires parameters");

                report.GenerateDocument();
                report.GenerationType = sophis.reporting.eMGenerationType.M_xmlOnly;

                var results = XDocument.Load(report.GetDocumentFullPath());

                XNamespace reporting = "http://www.sophis.net/reporting";
                var sourceResults = results.Descendants(reporting + (sourceName + "Result"));

                var dt = new DataTable();
                dt.TableName = sourceName;

                foreach (var resultRow in sourceResults)
                {
                    var row = dt.Rows.Add();
                    foreach (var resultField in resultRow.Elements())
                    {
                        if(!dt.Columns.Contains(resultField.Name.LocalName))
                            dt.Columns.Add(resultField.Name.LocalName);

                        row[resultField.Name.LocalName] = resultField.Value;
                    }
                }

                return dt;
            }
        }
    }
}
