//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.utils;

public class CSMUserRights : IDisposable
{
    public unsafe CSMUserRights()
    {
    }

    public unsafe CSMUserRights(uint ident)
    {
    }

    public unsafe CSMUserRights(uint ident, uint parentIdent)
    {
    }

    public eMRightStatusType GetUserDefRight(CMString right)
    {
        throw new NotImplementedException();
    }

    public int GetParentID()
    {
        throw new NotImplementedException();
    }

    public int LoadDetails()
    {
        throw new NotImplementedException();
    }

    public CMString GetName()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}