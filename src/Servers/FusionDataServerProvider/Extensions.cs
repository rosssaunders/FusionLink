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
    }
}