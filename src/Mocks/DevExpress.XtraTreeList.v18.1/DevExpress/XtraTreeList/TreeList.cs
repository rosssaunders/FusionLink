//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using DevExpress.XtraTreeList.Columns;

namespace DevExpress.XtraTreeList
{
    public class TreeList
    {
        public TreeListColumn FocusedColumn { get; set; }

        public TreeListColumnCollection Columns { get; }
    }
}
