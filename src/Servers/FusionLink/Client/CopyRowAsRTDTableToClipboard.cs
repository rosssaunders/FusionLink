//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Collections;
using System.Linq;
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

            var sb = new StringBuilder();
            var columns = GetDisplayedColumns();

            sb.Append($"Id").Append("\t");
            sb.Append($"Portfolio Name").Append("\t");
            foreach (string column in columns)
            {
                sb.Append(column).Append("\t");
            }
            sb.AppendLine();

            for (int i = 0; i < folioVector.Count; i++)
            {
                var portfolio = folioVector[i] as CSMPortfolio;
                var portfolioId = portfolio.GetCode();

                sb.Append(portfolioId).Append("\t");

                foreach (string column in columns)
                {
                    string formula = GetPortfolioFormula(portfolioId, column);
                    sb.Append(formula).Append("\t");
                }

                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        public override void Action(ArrayList positionList, CMString ActionName)
        {
            if (positionList.Count == 0)
                return;

            var sb = new StringBuilder();

            var isFlatView = false;
            for(var i = 0; i < positionList.Count; i++)
            {
                if (isFlatView)
                    break;

                var position = positionList[i] as CSMPosition;
                isFlatView = position.IsFlat();
            }

            var columns = GetDisplayedColumns();

            if (isFlatView)
            {
                sb.Append($"PortfolioId").Append("\t"); ;
                sb.Append($"InstrumentId").Append("\t"); ;
                foreach (string column in columns)
                {
                    sb.Append(column).Append("\t");
                }
                sb.AppendLine();

                for (int i = 0; i < positionList.Count; i++)
                {
                    var position = positionList[i] as CSMPosition;
                    var portfolioId = position.GetPortfolioCode();
                    var instrumentId = position.GetInstrumentCode();

                    sb.Append(portfolioId).Append("\t");
                    sb.Append(instrumentId).Append("\t");

                    foreach (string column in columns)
                    {
                        string formula = GetFlatPositionFormula(portfolioId, instrumentId, column);
                        sb.Append(formula).Append("\t");
                    }

                    sb.AppendLine();
                }
            }
            else
            {
                sb.Append($"Id").Append("\t"); ;
                foreach (string column in columns)
                {
                    sb.Append(column).Append("\t");
                }
                sb.AppendLine();

                for (int i = 0; i < positionList.Count; i++)
                {
                    var position = positionList[i] as CSMPosition;
                    var positionId = position.GetIdentifier();
                    
                    sb.Append(positionId).Append("\t");

                    foreach (string column in columns)
                    {
                        string formula = GetPositionFormula(positionId, column);
                        sb.Append(formula).Append("\t");
                    }

                    sb.AppendLine();
                }
            }

            Clipboard.SetText(sb.ToString());
        }
    }
}