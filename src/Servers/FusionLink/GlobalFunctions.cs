//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.misc;
using sophis.portfolio;
using sophis.value;

namespace RxdSolutions.FusionLink
{
    public class GlobalFunctions : CSMAmGlobalFunctions
    {
        public event EventHandler PortfolioCalculationEnded;

        public event EventHandler PortfolioAdditionEnded;

        public event EventHandler PortfolioRefreshVersionChanged;

        private int _lastPortfolioRefreshVersion = 0;

        public override void EndPortfolioCalculation()
        {
            base.EndPortfolioCalculation();

            PortfolioCalculationEnded?.Invoke(this, new EventArgs());
        }

        public override void EndPortfolioAddition(CSMExtraction extraction, int folioId)
        {
            base.EndPortfolioAddition(extraction, folioId);

            if(_lastPortfolioRefreshVersion != CSMPortfolioColumn.GetRefreshVersion())
            {
                _lastPortfolioRefreshVersion = CSMPortfolioColumn.GetRefreshVersion();

                PortfolioRefreshVersionChanged?.Invoke(this, new EventArgs());
            }

            PortfolioAdditionEnded?.Invoke(this, new EventArgs());
        }

        //public override void EndPortfolioCalculation(CSMExtraction extraction)
        //{
        //    base.EndPortfolioCalculation(extraction);
        //}

        //public override void EndPortfolioCalculation(CSMExtraction extraction, int folioId)
        //{
        //    base.EndPortfolioCalculation(extraction, folioId);
        //}

        //public override void StartPortfolioCalculation()
        //{
        //    base.StartPortfolioCalculation();
        //}

        //public override void StartPortfolioCalculation(CSMExtraction extraction)
        //{
        //    base.StartPortfolioCalculation(extraction);
        //}

        //public override void StartPortfolioCalculation(CSMExtraction extraction, int folio_id)
        //{
        //    base.StartPortfolioCalculation(extraction, folio_id);
        //}
    }
}