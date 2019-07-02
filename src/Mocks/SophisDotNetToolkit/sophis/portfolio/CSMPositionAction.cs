//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.tools;

namespace sophis.portfolio
{
    public class CSMPositionAction : IDisposable
    {
        public enum eMOrder
        {
            M_oAfter = 2,
            M_oSave = 1,
            M_oBefore = 0
        }

        public static void Register(string key, eMOrder e, CSMPositionAction prototype)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void NotifyTransferred(CSMPosition position, CSMEventVector message)
        {

        }

        public unsafe virtual void NotifyDeleted(CSMPosition position, CSMEventVector message)
        {

        }

        public unsafe virtual void NotifyModified(CSMPosition position, CSMEventVector message)
        {
        }
    }
}
