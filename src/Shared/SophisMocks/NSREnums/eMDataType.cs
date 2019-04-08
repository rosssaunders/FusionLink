//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSREnums
{
#pragma warning disable IDE1006 // Naming Styles
    public enum eMDataType
#pragma warning restore IDE1006 // Naming Styles
    {
        M_dDateTime = 15,
        M_dInt = 14,
        M_dUnicodeString = 13,
        M_dLongLong = 12,
        M_dArray = 11,
        M_dSlidingDate = 10,
        M_dBool = 9,
        M_dPointer = 8,
        M_dFloat = 7,
        M_dShort = 6,
        M_dDouble = 5,
        M_dDate = 4,
        M_dLong = 3,
        M_dPascalString = 2,
        M_dNullTerminatedString = 1,
        M_dSmallIcon = 0
    }
}
