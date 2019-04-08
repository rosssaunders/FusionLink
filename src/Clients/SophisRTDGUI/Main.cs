//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.ServiceModel;
using sophis;
using sophis.portfolio;

namespace RxdSolutions.Sophis2Excel
{
    public class Main : IMain
    {
        public void EntryPoint()
        {
            CSMPositionCtxMenu.Register("Copy Cell as Excel Reference", new CopyRTDCellToClipboard());

            CSMPositionCtxMenu.Register("Copy Table as Excel References", new CopyRTDTableToClipboard());
        }

        public void Close()
        {
        }
    }
}
