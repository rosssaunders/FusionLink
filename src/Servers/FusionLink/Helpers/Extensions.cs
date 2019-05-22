//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophisTools;

namespace RxdSolutions.FusionLink
{
    public static class Extensions
    {
        public static DateTime GetDateTime(this CSMDay day)
        {
            return new DateTime(day.fYear, day.fMonth, day.fDay);
        }

        public static DateTime GetDateTime(this int day)
        {
            if(day == 99999)
            {
                return DateTime.Today;
            }

            using(var dt = new CSMDay(day))
                return new DateTime(dt.fYear, dt.fMonth, dt.fDay);
        }

        public static object ExtractValueFromSophisCell(this SSMCellValue cv, SSMCellStyle cs)
        {
            switch (cs.kind)
            {
                case NSREnums.eMDataType.M_dDate:
                case NSREnums.eMDataType.M_dDateTime:
                    {
                        var day = new CSMDay(cv.integerValue);
                        return new DateTime(day.fYear, day.fMonth, day.fDay);
                    }

                case NSREnums.eMDataType.M_dInt:
                    return (long)cv.integerValue;

#if !V72
                case NSREnums.eMDataType.M_dPascalString:
#endif
                case NSREnums.eMDataType.M_dUnicodeString:
                case NSREnums.eMDataType.M_dNullTerminatedString:
                    return cv.GetString();

                case NSREnums.eMDataType.M_dLong:
                case NSREnums.eMDataType.M_dLongLong:
                    return cv.longlongValue;

                case NSREnums.eMDataType.M_dArray:
                    return cv.GetString();

                case NSREnums.eMDataType.M_dSlidingDate:
                    return cv.GetString();

                case NSREnums.eMDataType.M_dBool:
                    return cv.shortInteger;

                case NSREnums.eMDataType.M_dPointer:
                    return cv.shortInteger;

                case NSREnums.eMDataType.M_dDouble:
                    return cv.doubleValue;

                case NSREnums.eMDataType.M_dFloat:
                    return (double)cv.floatValue;

                case NSREnums.eMDataType.M_dShort:
                    return (double)cv.shortInteger;

                case NSREnums.eMDataType.M_dSmallIcon:
                    return (double)cv.iconIdentifier;
            }

            throw new ApplicationException("Unknown eMDataType");
        }
    }
}