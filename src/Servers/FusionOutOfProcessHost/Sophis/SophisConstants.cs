using System;

namespace sophis
{
    public static class Constants
    {
        public static int SophisNull = -10000000;
        public static DateTime SophisStartDate;

        static Constants()
        {
            SophisStartDate = new DateTime(1904, 1, 1);
        }
    }
}
