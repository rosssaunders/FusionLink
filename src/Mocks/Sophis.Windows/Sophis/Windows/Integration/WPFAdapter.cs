//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Windows;

namespace Sophis.Windows.Integration
{
    public abstract class WPFAdapter
    {
        public static WPFAdapter Instance { get; }

        public abstract IntPtr GetActiveWindow(WindowKey wndKey);

        public abstract void ActivateWindow(IntPtr wnd);

        public IntPtr OpenWindow(FrameworkElement fwkElement, string title, WindowKey wndKey, bool enableScroll)
        {
            throw new NotImplementedException();
        }
    }
}