using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.market_data;

#if V72

namespace sophis.DAL
{
    public class MYieldCurvePoints : IDisposable
    {
        public unsafe SSMYieldPoint fPointList { get; set; }
        
        public unsafe int fPointCount { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

#endif
