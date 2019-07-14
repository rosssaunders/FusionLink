//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetCurveFunctions
    {
        [ExcelFunction(Name = "GETCURVEPOINTS",
                       Description = "Returns a list of points on a given curve.",
                       HelpTopic = "Get-Curve-Points")]
        public static object GetCurveDefinition(
            [ExcelArgument(Name = "currency", Description = "The code of the currency")]string currency,
            [ExcelArgument(Name = "family_reference", Description = "The curve family")]string family,
            [ExcelArgument(Name = "curve_reference", Description = "The curve reference")]string reference)
        {
            if (AddIn.Client.State != System.ServiceModel.CommunicationState.Opened)
            {
                return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.GetExcelRangeError(Resources.NotConnectedMessage));
            }

            try
            {
                var results = AddIn.Client.GetCurvePoints(currency, family, reference);
                CurvePoint[] points = results.ToArray();

                if (points.Length == 0)
                {
                    return ExcelRangeResizer.TransformToExcelRange(ExcelStaticData.ExcelEmptyRange);
                }
                else
                {
                    object[,] array = new object[points.Length + 1, 5];

                    array[0, 0] = "Tenor";
                    array[0, 1] = "Rate";
                    array[0, 2] = "IsEnabled";
                    array[0, 3] = "RateCode";
                    array[0, 4] = "Type";

                    for (int i = 0; i < points.Length; i++)
                    {
                        array[i + 1, 0] = points[i].Tenor;
                        array[i + 1, 1] = points[i].Rate ?? 0;
                        array[i + 1, 2] = points[i].IsEnabled;
                        array[i + 1, 3] = points[i].RateCode;
                        array[i + 1, 4] = points[i].PointType;
                    }

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
