//  Copyright (c) RXD Solutions. All rights reserved.
using System;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public sealed class ActionDisposable : IDisposable
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