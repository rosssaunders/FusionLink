//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ActionDisposable : IDisposable
    {
        Action _disposeAction;
        public ActionDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }
        public void Dispose()
        {
            _disposeAction();
        }
    }
}