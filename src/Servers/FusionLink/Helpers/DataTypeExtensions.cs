//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Linq;
using sophis.backoffice_kernel;
using sophis.finance;
using sophis.instrument;
using sophis.market_data;
using sophis.static_data;
using sophis.utils;
using sophisTools;

namespace RxdSolutions.FusionLink.Helpers
{
    public static class DataTypeExtensions
    {
        public static int SophisNull = -10_000_000;

        public static string GetName(this CSMCalendar calendar)
        {
            using var str = new CMString();
            calendar.GetName(str);
            return str.StringValue;
        }

        public static object ConvertSophisNumber(this double number)
        {
            if (number == SophisNull)
                return null;

            return Math.Round(number, 12);
        }

        public static string GetStringFromMethod(Action<CMString> func)
        {
            using var name = new CMString();
            func(name);
            return name.StringValue;
        }

        public static string GetStringFromMethod(Func<CMString, CMString> func)
        {
            using var name = new CMString();
            func(name);
            return name.StringValue;
        }

        public static string GetMetaModelName(this CSMMetaModel metaModel)
        {
            using var str = metaModel.GetName();
            return str.StringValue;
        }

        public static string GetPartialPaymentMethod(this eMPartialRedemptionType partialPaymentType)
        {
            switch (partialPaymentType)
            {
                case eMPartialRedemptionType.M_ePRTFloatingNotional:
                    return "Floating Notional";
                case eMPartialRedemptionType.M_ePRTNotional:
                    return "Notional";
                case eMPartialRedemptionType.M_ePRTSea:
                    return "Equal Series";
                case eMPartialRedemptionType.M_ePRTTea:
                    return "Equal Annuities";
            }

            return "";
        }

        public static string SophisEnumToString(this Enum value)
        {
            var sophisName = value.ToString();

            if(sophisName.Contains("_"))
            {
                var firstIndex = sophisName.IndexOf("_");

                var afterUnderscore = sophisName.Substring(firstIndex);

                //now find the first upper case letter
                var i = 0;
                foreach(var chr in afterUnderscore)
                {
                    if(Char.IsUpper(chr))
                    {
                        return afterUnderscore.Substring(i);
                    }

                    i++;
                }

                return afterUnderscore;
            }

            return sophisName;
        }

        public static string GetCurrencyCode(this int code)
        {
            using var str = new CMString();
            CSMCurrency.CurrencyToString(code, str);
            return str.StringValue;
        }

        public static string GetSeniorityName(this int code)
        {
            using var str = new CMString();
            using var sen = CSMSeniority.GetSeniority(code);
            return sen.fName;
        }

        public static string GetListedMarketName(this int code)
        {
            if (code == 0)
                return null;

            using var lm = sophis.listed_market.CSMListedMarket.GetCSRListedMarket(code);
            using var str = new CMString();
            lm.GetName(str);
            return str.StringValue;
        }

        public static string GetBusinessEventType(this int code)
        {
            if (code == 0)
                return null;

            using var lm = CSMBusinessEvent.GetBusinessEventById(code);
            using var str = lm.GetName();
            return str.StringValue;
        }

        public static string GetThirdPartyReference(this int code)
        {
            if (code == 0)
                return null;

            using var lm = CSMThirdParty.GetCSRThirdParty(code);
            using var str = new CMString();
            lm.GetReference(str);
            return str.StringValue;
        }

        public static string GetYieldCurveName(this int code)
        {
            using var yc = CSMYieldCurve.GetCSRYieldCurve(code);
            if (yc is null)
                return null;

            using var ayc = yc.GetActiveSSYieldCurve();
            return ayc.fName;
        }

        public static string GetMarketName(this CSMMarket market)
        {
            using var mkt = market;

            if (mkt is null)
                return "";

            using var str = new CMString();
            mkt.GetName(str);
            return str.StringValue;
        }

        public static string GetAllotmentName(this int code)
        {
            using var allotment = SSMAllotment.GetName(code);
            return allotment.StringValue;
        }

        public static object ToDateTime(this int day)
        {
            if(day == 99999)
            {
                return DateTime.Today;
            }
               
            if(day == 0)
            {
                return 0;
            }

            if (day == SophisNull)
            {
                return 0;
            }

            using (var dt = new CSMDay(day))
                return new DateTime(dt.fYear, dt.fMonth, dt.fDay);
        }

        public static object ToDateTime(this short day)
        {
            return ToDateTime((int)day);
        }

        public static object ExtractValueFromSophisCell(this SSMCellValue cv, SSMCellStyle cs)
        {
            switch (cs.kind)
            {
                case NSREnums.eMDataType.M_dDate:
                case NSREnums.eMDataType.M_dDateTime:
                    
                    if(cv.integerValue == 0)
                        return 0;

                    return ToDateTime(cv.integerValue);
                    

                case NSREnums.eMDataType.M_dInt:
                    
                    return ConvertLong((long)cv.integerValue, cs.@null);

#if !V72
                case NSREnums.eMDataType.M_dPascalString:
#endif
                case NSREnums.eMDataType.M_dUnicodeString:
                case NSREnums.eMDataType.M_dNullTerminatedString:

                    var stringValue = ConvertString(cv.GetString());
                    return stringValue;

                case NSREnums.eMDataType.M_dLong:
                case NSREnums.eMDataType.M_dLongLong:
                    return cv.longlongValue;

                case NSREnums.eMDataType.M_dArray:
                    return cv.GetString();

                case NSREnums.eMDataType.M_dSlidingDate:
                    return cv.GetString();

                case NSREnums.eMDataType.M_dBool:
                    return Convert.ToBoolean(cv.shortInteger);

                case NSREnums.eMDataType.M_dPointer:
                    return cv.shortInteger;

                case NSREnums.eMDataType.M_dDouble:
                    return ConvertDouble(cv.doubleValue, cs.@null);

                case NSREnums.eMDataType.M_dFloat:
                    return ConvertDouble((double)cv.floatValue, cs.@null);

                case NSREnums.eMDataType.M_dShort:
                    return ConvertLong((long)cv.shortInteger, cs.@null);

                case NSREnums.eMDataType.M_dSmallIcon:
                    return (long)cv.iconIdentifier;
            }

            throw new ApplicationException("Unknown eMDataType");
        }

        public static object ConvertLong(long value, sophis.gui.eMNullValueType valueType)
        {
            if (valueType == sophis.gui.eMNullValueType.M_nvUndefined || valueType == sophis.gui.eMNullValueType.M_nvZeroAndUndefined)
            {
                if (value == SophisNull)
                    return null;
            }

            if (valueType == sophis.gui.eMNullValueType.M_nvZero || valueType == sophis.gui.eMNullValueType.M_nvZeroAndUndefined)
            {
                if(value == 0)
                    return null;
            }

            return value;
        }

        public static object ConvertDouble(double value, sophis.gui.eMNullValueType valueType)
        {
            if (valueType == sophis.gui.eMNullValueType.M_nvUndefined || valueType == sophis.gui.eMNullValueType.M_nvZeroAndUndefined)
            {
                if (value == SophisNull)
                    return null;
            }

            if (valueType == sophis.gui.eMNullValueType.M_nvZero || valueType == sophis.gui.eMNullValueType.M_nvZeroAndUndefined)
            {
                if (value == 0)
                    return null;
            }

            return value;
        }

        public static int ConvertDateTime(DateTime value)
        {
            using var day = new CSMDay(value.Day, value.Month, value.Year);
            return day.toLong();
        }

        private static object ConvertString(string inString)
        {
            if (inString == null) return null;
            var newString = new System.Text.StringBuilder();
            char ch;
            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                if (!char.IsControl(ch))
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();
        }
    }
}