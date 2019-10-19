//  Copyright (c) RXD Solutions. All rights reserved.


using System.Collections.Generic;
using sophis.market_data;
using sophis.static_data;

namespace RxdSolutions.FusionLink.Services
{
    public class CurveService
    {
        public List<CurvePoint> GetCurvePoints(string currency, string family, string reference)
        {
            var currencyCode = CSMCurrency.StringToCurrency(currency);

            if (currencyCode == 0)
            {
                throw new CurrencyNotFoundException();
            }

            var familyCode = CSMYieldCurveFamily.GetYieldCurveFamilyCode(currencyCode, family);

            if (familyCode == 0)
            {
                throw new CurveFamilyFoundException();
            }

            var curveId = CSMYieldCurve.LookUpYieldCurveId(familyCode, reference);

            if (curveId == 0)
            {
                throw new CurveNotFoundException();
            }

            var results = new List<CurvePoint>();

            using (CSMYieldCurve yieldCurve = CSMYieldCurve.GetCSRYieldCurve(curveId))
            using (SSMYieldCurve activeCurve = yieldCurve.GetActiveSSYieldCurve())
            {
                for (int i = 0; i < GetPointCount(activeCurve); i++)
                {
                    using (var yieldPoint = GetPointList(activeCurve).GetNthElement(i))
                    {
                        var cp = new CurvePoint();
                        results.Add(cp);

                        double multiplier = 1;

                        if (yieldPoint.fType == 'x')
                        {
                            string startDateOffset = yieldPoint.fStartDate > 0 ? $"{yieldPoint.fStartDate}" : "";
                            cp.Tenor = $"{yieldPoint.fMaturity}{yieldPoint.fType}{startDateOffset}";
                            multiplier = 0.01d;
                        }
                        else
                        {
                            string startDateOffset = yieldPoint.fStartDate > 0 ? $"+{yieldPoint.fStartDate}" : "";
                            cp.Tenor = $"{yieldPoint.fMaturity}{yieldPoint.fType}{startDateOffset}";
                        }

                        cp.PointType = yieldPoint.IsPointOfType(eMTypeSegment.M_etsFutureFRA) ? "FutureFRA" : yieldPoint.IsPointOfType(eMTypeSegment.M_etsMoneyMarket) ? "Money Market" : yieldPoint.IsPointOfType(eMTypeSegment.M_etsSwap) ? "Swap" : "Unknown";
                        cp.Rate = yieldPoint.fYield * multiplier;
                        cp.IsEnabled = yieldPoint.fInfoPtr.fIsUsed;
                        cp.RateCode = yieldPoint.fInfoPtr.fRateCode.ToString();
                    }
                }
            }

            return results;
        }

        private int GetPointCount(SSMYieldCurve curve)
        {
#if V72
            return curve.fPoints.fPointCount;
#else
            return curve.fPointCount;
#endif
        }

        private SSMYieldPoint GetPointList(SSMYieldCurve curve)
        {
#if V72
            return curve.fPoints.fPointList;
#else
            return curve.fPointList;
#endif
        }
    }
}
