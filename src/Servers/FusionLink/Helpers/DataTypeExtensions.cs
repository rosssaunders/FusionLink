//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using sophisTools;

namespace RxdSolutions.FusionLink.Helpers
{
    public static class DataTypeExtensions
    {
        public static int SophisNull = -10_000_000;

        public static object GetDateTime(this int day)
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

        public static object GetDateTime(this short day)
        {
            return GetDateTime((int)day);
        }

        public static object ExtractValueFromSophisCell(this SSMCellValue cv, SSMCellStyle cs)
        {
            switch (cs.kind)
            {
                case NSREnums.eMDataType.M_dDate:
                case NSREnums.eMDataType.M_dDateTime:
                    
                    if(cv.integerValue == 0)
                        return 0;

                    return GetDateTime(cv.integerValue);
                    

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