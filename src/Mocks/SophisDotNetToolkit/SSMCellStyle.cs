//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using NSREnums;
using sophis.gui;

public class SSMCellStyle : IDisposable
{
    public SSMCellStyle()
    {
    }

    public eMDataType kind { get; set; }

    public eMNullValueType @null { get; set; }

    public int @decimal { get; set; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}