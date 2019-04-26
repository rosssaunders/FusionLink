using sophis.utils;

namespace sophis.static_data
{
    public static class CSMInterestRateExtensions
    {
        public static string GetFamilyName(this CSMInterestRate rate)
        {
            var familyId = rate.GetFamily();

            if (familyId == 0)
                familyId = CSMCurrency.GetCSRCurrency(rate.GetCurrency()).GetDefaultFamilyYieldCurveCode();

            var family = CSMYieldCurveFamily.GetCSRYieldCurveFamily(familyId);

            using (CMString familyName = new CMString())
            {
                family.GetName(familyName);
                return familyName.ToString();
            }
        }
    }
}
