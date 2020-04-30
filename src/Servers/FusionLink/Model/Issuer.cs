using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Model
{
    public class Issuer : Instrument
    {
        public Issuer(int code) : base(code)
        {
        }

        public int IndexCode
        {
            get
            {
                using CSMIssuer instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIndexCode();
            }
        }

        public int IssuerCode
        {
            get
            {
                using CSMIssuer instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerCode();
            }
        }

        public string IssuerIndustry
        {
            get
            {
                using CSMIssuer instrument = CSMInstrument.GetInstance(code);
                using var industry = instrument.GetIssuerIndustry();
                return industry.StringValue;
            }
        }

        public int IndustryCode
        {
            get
            {
                using CSMIssuer instrument = CSMInstrument.GetInstance(code);
                return instrument.GetIssuerIndustryCode();
            }
        }

        public int ReferenceEntity
        {
            get
            {
                using CSMIssuer instrument = CSMInstrument.GetInstance(code);
                return instrument.GetParentReferenceEntity();
            }
        }

        public double GetQuotity
        {
            get
            {
                using CSMIssuer instrument = CSMInstrument.GetInstance(code);
                return instrument.GetQuotity();
            }
        }

        public int ReferenceEquity
        {
            get
            {
                using CSMIssuer instrument = CSMInstrument.GetInstance(code);
                return instrument.GetReferenceEquity();
            }
        }
    }
}
