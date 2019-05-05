using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.utils;

namespace sophis.scenario
{
    public class CSMScenario : IDisposable
    {
        public static void Register(string key, CSMScenario prototype)
        {

        }

        public virtual eMProcessingType GetProcessingType()
        {
            return eMProcessingType.M_pUserPreference;
        }

        public virtual CMString GetName()
        {
            throw new NotImplementedException();
        }

        public virtual void Run()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
