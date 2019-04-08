//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Columns;
using sophis.portfolio;
using sophis.utils;
using Sophis.Data.Utils;
using Sophis.Portfolios;
using static RxdSolutions.Sophis2Excel.ExcelHelper;

namespace RxdSolutions.Sophis2Excel
{
    public class CopyRTDTableToClipboard : CSMPositionCtxMenu
    {
        public override bool IsFolioAuthorized(ArrayList folioVector)
        {
            for(var i = 0; i < folioVector.Count; i++)
            {
                var portfolio = folioVector[i] as CSMPortfolio;
                {
                    if (portfolio.GetPortfolioType() == 7)
                        return false;
                }
            }

            return true;
        }

        public override bool IsAuthorized(ArrayList positionList)
        {
            return true;
        }

        private IEnumerable<string> GetDisplayedColumns()
        {
            var pv = NavigationManager.Instance.FocusedTreeView as PortfolioView;
            var columns = pv.TreeList.Columns;
            return columns
                        .Cast<TreeListColumn>()
                        .Select(x => x.Caption)
                        .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        public override int GetContextMenuGroup()
        {
            return 101;
        }

        public override void FolioAction(ArrayList folioVector, CMString ActionName)
        {
            if (folioVector.Count == 0)
                return;

            var table = BuildTable(

                i => {

                    var portfolio = folioVector[i] as CSMPortfolio;
                    return portfolio.GetCode();
                },

                i => {

                    var portfolio = folioVector[i] as CSMPortfolio;
                    using (var name = new CMString())
                    {
                        portfolio.GetName(name);

                        return name.StringValue;
                    }
                },

                (i, column) => {

                    var folio = folioVector[i] as CSMPortfolio;

                    return GetPortfolioFormula(folio.GetCode(), column);

                },

                folioVector.Count);

            Clipboard.SetText(table);
        }

        public override void Action(ArrayList positionList, CMString ActionName)
        {
            if (positionList.Count == 0)
                return;

            var table = BuildTable(

                i => {
                    var position = positionList[i] as CSMPosition;
                    return position.GetIdentifier();
                },

                i => {
                    var position = positionList[i] as CSMPosition;
                    using (var instrument = position.GetCSRInstrument())
                    using (var cmString = new CMString())
                    {
                        instrument.GetName(cmString);
                        return cmString.StringValue;
                    }
                },

                (i, column) => {
                    var position = positionList[i] as CSMPosition;
                    return GetPositionFormula(position.GetIdentifier(), column);
                }, 
                
                positionList.Count);

            Clipboard.SetText(table);
        }

        private string BuildTable(Func<int, int> getUniqueIdentifier, Func<int, string> getFormulaForTitle, Func<int, string, string> getFormulaForColumn, int count)
        {
            var sb = new StringBuilder();

            sb.Append("Id\t");
            sb.Append("Name\t");
            foreach (var column in GetDisplayedColumns())
            {
                sb.Append(column).Append("\t");
            }
            sb.AppendLine();

            for (var i = 0; i < count; i++)
            {
                sb.Append(getUniqueIdentifier(i)).Append("\t");
                sb.Append(getFormulaForTitle(i)).Append("\t");

                foreach (var column in GetDisplayedColumns())
                {
                    var formula = getFormulaForColumn(i, column);

                    sb.Append(formula).Append("\t");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}