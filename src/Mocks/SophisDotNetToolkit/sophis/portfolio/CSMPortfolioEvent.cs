using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sophis.portfolio
{
    public class CSMPortfolioEvent : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public enum eMOrder
        {
            M_oAfter = 2,
            M_oUpdate = 1,
            M_oBefore = 0
        }

        public static void Register(string key, eMOrder e, CSMPortfolioEvent prototype)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual bool HasBeenCreated(CSMPortfolio portfolio)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void HasBeenModified(CSMPortfolio portfolio)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void HasBeenTransferred(CSMPortfolio portfolio)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void HasBeenDeleted(int code)
        {
            throw new NotImplementedException();
        }
    }
}
