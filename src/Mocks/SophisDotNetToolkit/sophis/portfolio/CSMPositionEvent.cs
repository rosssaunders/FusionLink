//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace sophis.portfolio
{
    public class CSMPositionEvent : IDisposable
    {
        public enum eMOrder
        {
            M_oAfter = 2,
            M_oUpdate = 1,
            M_oBefore = 0
        }

        public static void Register(string key, eMOrder e, CSMPositionEvent prototype)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public unsafe virtual bool HasBeenDeleted(CSMPosition position)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual bool HasBeenModified(CSMPosition position)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void HasBeenTransferred(CSMPosition position)
        {
            throw new NotImplementedException();
        }
    }
}