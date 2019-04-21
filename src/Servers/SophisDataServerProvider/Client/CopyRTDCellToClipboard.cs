//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using sophis.portfolio;
using sophis.utils;
using Sophis.Data.Utils;
using Sophis.Portfolios;
using static RxdSolutions.FusionLink.ExcelHelper;

namespace RxdSolutions.FusionLink
{
    public class CopyRTDCellToClipboard : CSMPositionCtxMenu
    {
        public override bool IsFolioAuthorized(ArrayList folioVector)
        {
            return !string.IsNullOrWhiteSpace(GetSelectedColumn());
        }

        public override bool IsAuthorized(ArrayList positionList)
        {
            return !string.IsNullOrWhiteSpace(GetSelectedColumn());
        }

        public override int GetContextMenuGroup()
        {
            return 101;
        }

        public override void FolioAction(ArrayList folioVector, CMString ActionName)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < folioVector.Count; i++)
            {
                var folio = folioVector[i] as CSMPortfolio;

                var formula = GetPortfolioFormula(folio.GetCode(), GetSelectedColumn());
                sb.Append(formula).AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        public override void Action(ArrayList positionList, CMString ActionName)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < positionList.Count; i++)
            {
                //Check if the position is a future. If so, allow it
                var position = positionList[i] as CSMPosition;

                var formula = GetPositionFormula(position.GetIdentifier(), GetSelectedColumn());
                sb.Append(formula).AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        private string GetSelectedColumn()
        {
            var pv = NavigationManager.Instance.FocusedTreeView as PortfolioView;
            var column = pv.TreeList.FocusedColumn;
            return column.Caption;
        }
    }
}