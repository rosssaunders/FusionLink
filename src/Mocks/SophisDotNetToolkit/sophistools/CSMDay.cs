//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.utils;

namespace sophisTools
{
    public class CSMDay : IDisposable
    {
        public unsafe short fYear { get; set; }

        public unsafe short fMonth { get; set; }

        public unsafe short fDay { get; set; }

        public unsafe CSMDay()
        {
        }

        public unsafe CSMDay(CMString yyyymmdd)
        {
        }

        public unsafe CSMDay(int day, int month, int year)
        {
        }

        public unsafe CSMDay(int day)
        {
        }

        public int toLong()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
