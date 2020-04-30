using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Helpers;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class Equity : Instrument
    {
        public Equity(int code) : base(code)
        {
        }

        public double Beta
        {
            get
            {
                using CSMEquity instrument = CSMInstrument.GetInstance(code);
                return instrument.GetBeta();
            }
        }

        public double TradingUnits
        {
            get
            {
                using CSMEquity instrument = CSMInstrument.GetInstance(code);
                return instrument.GetTradingUnits();
            }
        }

        public object AccountingReferenceDate
        {
            get
            {
                using CSMEquity instrument = CSMInstrument.GetInstance(code);
                return instrument.GetAccountingReferenceDate().ToDateTime();
            }
        }

        public double SharesOutstanding
        {
            get
            {
                using CSMEquity instrument = CSMInstrument.GetInstance(code);
                return instrument.GetInstrumentCount();
            }
        }

        public int IssuerCode
        {
            get
            {
                using CSMEquity instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerCode();
            }
        }
    }
}
