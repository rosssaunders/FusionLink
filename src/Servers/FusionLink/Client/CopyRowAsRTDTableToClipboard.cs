//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using sophis.portfolio;
using sophis.utils;
using static RxdSolutions.FusionLink.Client.ExcelHelper;
using static RxdSolutions.FusionLink.Client.PortfolioViewHelper;

namespace RxdSolutions.FusionLink.Client
{
    public class CopyRowAsRTDTableToClipboard : CSMPositionCtxMenu
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

        public override int GetContextMenuGroup()
        {
            return 101;
        }

        public override void FolioAction(ArrayList folioVector, CMString ActionName)
        {
            if (folioVector.Count == 0)
                return;

            var table = BuildTable(

                "Id",
                i => {

                    var portfolio = folioVector[i] as CSMPortfolio;
                    return portfolio.GetCode();
                },

                "Portfolio Name",
                i => {

                    var portfolio = folioVector[i] as CSMPortfolio;
                    return GetPortfolioFormula(portfolio.GetCode(), "Portfolio Name");
                },

                (i, column) => {

                    var portfolio = folioVector[i] as CSMPortfolio;

                    return GetPortfolioFormula(portfolio.GetCode(), column);

                },

                folioVector.Count);

            Clipboard.SetText(table);
        }

        public override void Action(ArrayList positionList, CMString ActionName)
        {
            if (positionList.Count == 0)
                return;

            var table = BuildTable(

                "Id",
                i => {
                    var position = positionList[i] as CSMPosition;
                    return position.GetIdentifier();
                },

                "Instrument name",
                i => {
                    var position = positionList[i] as CSMPosition;
                    return GetPositionFormula(position.GetIdentifier(), "Instrument name");
                },

                (i, column) => {
                    var position = positionList[i] as CSMPosition;
                    return GetPositionFormula(position.GetIdentifier(), column);
                }, 
                
                positionList.Count);

            Clipboard.SetText(table);
        }

        private string BuildTable(string id, Func<int, int> getUniqueIdentifier, string title, Func<int, string> getFormulaForTitle, Func<int, string, string> getFormulaForColumn, int count)
        {
            var sb = new StringBuilder();

            sb.Append($"{id}\t");
            sb.Append($"{title}\t");
            foreach (string column in GetDisplayedColumns())
            {
                sb.Append(column).Append("\t");
            }
            sb.AppendLine();

            for (int i = 0; i < count; i++)
            {
                sb.Append(getUniqueIdentifier(i)).Append("\t");
                sb.Append(getFormulaForTitle(i)).Append("\t");

                foreach (string column in GetDisplayedColumns())
                {
                    string formula = getFormulaForColumn(i, column);

                    sb.Append(formula).Append("\t");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}