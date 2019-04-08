//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using NSREnums;

public class SSMCellStyle : IDisposable
{
    public SSMCellStyle()
    {
    }

    public eMDataType kind { get; set; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}