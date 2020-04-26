//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class CalendarFunctions
    {
        [ExcelFunction(Name = "ADDBUSINESSDAYS",
                       Description = "Adds the given number of days to the date using the supplied calendar",
                       HelpTopic = "Add-Business-Days")]
        public static object AddBusinessDays(
            [ExcelArgument(Name = "date", Description = "The date to use")]DateTime date,
            [ExcelArgument(Name = "number_of_days", Description = "The number of days to add")]int numberOfDays,
            [ExcelArgument(Name = "currency", Description = "The currency to use")]string currency,
            [ExcelArgument(Name = "calendar_type", Description = "The type of calendar to use. 'Currency', 'Place' or 'Market'. Defaults to Currency")]string calendarType,
            [ExcelArgument(Name = "name", Description = "If Calendar Type is 'Market' or 'Place' the name of the Place or Market")]string name)
        {
            if (ExcelDnaUtil.IsInFunctionWizard())
                return null;

            if (date == DateTime.MinValue)
                return Resources.DateNotEntered;

            if (string.IsNullOrWhiteSpace(currency))
                return Resources.CalendarCurrencyNotEntered;

            if (string.IsNullOrWhiteSpace(calendarType))
                calendarType = "Currency";

            if (!Enum.TryParse(calendarType, ignoreCase: true, out CalendarType result))
                return Resources.CalendarTypeUnknown;

            if(result == CalendarType.Market || result == CalendarType.Place)
            {
                if(string.IsNullOrWhiteSpace(name))
                {
                    return Resources.CalendarNameMissing;
                }
            }

            try
            {
                return AddIn.Client.AddBusinessDays(date, numberOfDays, currency, result, name);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
