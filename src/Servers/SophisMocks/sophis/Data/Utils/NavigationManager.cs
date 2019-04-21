//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

namespace Sophis.Data.Utils
{
    public class NavigationManager
    {
        public static NavigationManager Instance { get; private set; }

        public ITreeView FocusedTreeView { get; set; }
    }
}