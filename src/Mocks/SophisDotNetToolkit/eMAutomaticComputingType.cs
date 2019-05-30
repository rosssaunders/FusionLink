using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum eMAutomaticComputingType
{
    M_acLast = 5,
    M_acPortfolioWithoutPNL = 4,
    M_acPortfolioOnlyPNL = 3,
    M_acNothing = 2,
    M_acFolio = 1,
    M_acQuotation = 0
}