//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace Sophis.Data.Utils
{
    public class FocusedTreeViewHub : ITreeView
    {
        public virtual ITreeView FocusedTreeView { get { throw new NotImplementedException(); } }
    }
}