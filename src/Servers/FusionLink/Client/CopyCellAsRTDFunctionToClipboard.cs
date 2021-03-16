//  Copyright (c) RXD Solutions. All rights reserved.


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
    public class CopyCellAsRTDFunctionToClipboard : CSMPositionCtxMenu
    {
        public override bool IsFolioAuthorized(ArrayList folioVector)
        {
            var selectedColumn = GetSelectedColumn();

            return !string.IsNullOrWhiteSpace(selectedColumn);
        }

        public override bool IsAuthorized(ArrayList positionList)
        {
            var selectedColumn = GetSelectedColumn();

            return !string.IsNullOrWhiteSpace(selectedColumn);
        }

        public override int GetContextMenuGroup()
        {
            return 101;
        }

        public override void FolioAction(ArrayList folioVector, CMString ActionName)
        {
            try
            {
                var sb = new StringBuilder();

                for (int i = 0; i < folioVector.Count; i++)
                {
                    var folio = folioVector[i] as CSMPortfolio;

                    string formula = GetPortfolioFormula(folio.GetCode(), GetSelectedColumn());
                    sb.Append(formula).AppendLine();
                }

                Clipboard.SetText(sb.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FusionLink");
            }
        }

#if SOPHIS2021

        public override void Action(CSMExtraction extraction, ArrayList positionList, CMString ActionName)
        {

#endif

#if SOPHIS713

        public override void Action(ArrayList positionList, CMString ActionName)
        {

#endif
            try
            {
                var sb = new StringBuilder();

                for (int i = 0; i < positionList.Count; i++)
                {
                    var position = positionList[i] as CSMPosition;

                    if(position.IsFlat())
                    {
                        string formula = GetFlatPositionFormula(position.GetPortfolioCode(), position.GetInstrumentCode(), GetSelectedColumn());
                        sb.Append(formula).AppendLine();
                    }
                    else
                    {
                        string formula = GetPositionFormula(position.GetIdentifier(), GetSelectedColumn());
                        sb.Append(formula).AppendLine();
                    }
                }

                Clipboard.SetText(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FusionLink");
            }
        }
    }
}