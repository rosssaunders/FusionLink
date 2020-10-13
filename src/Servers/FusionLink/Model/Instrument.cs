using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.backoffice_kernel;
using sophis.instrument;
using sophis.static_data;
using sophis.utils;

namespace RxdSolutions.FusionLink.Model
{
    public class Instrument
    {
        protected readonly int code;

        public Instrument(int code)
        {
            this.code = code;
        }

        public int Code
        {
            get
            {
                using var instrument = CSMInstrument.GetInstance(code);
                return instrument.GetCode();
            }
        }

        public string Reference
        {
            get
            {
                using var instrument = CSMInstrument.GetInstance(code);
                using var reference = instrument.GetReference();
                return reference.StringValue;
            }
        }

        public string Name
        {
            get
            {
                using var instrument = CSMInstrument.GetInstance(code);
                using var name = new CMString();
                instrument.GetName(name);
                return name.StringValue;
            }
        }

        public string Allotment
        {
            get
            {
                using var instrument = CSMInstrument.GetInstance(code);
                using var allotment = SSMAllotment.GetName(instrument.GetAllotment());
                return allotment.StringValue;
            }
        }

        public string Comment
        {
            get
            {
                using var instrument = CSMInstrument.GetInstance(code);
                using var comment = new CMString();
                instrument.GetComment(comment);
                return comment.StringValue;
            }
        }

        public virtual string Market
        {
            get
            {
                using var instrument = CSMInstrument.GetInstance(code);
                using var market = instrument.GetCSRMarket();
                using var name = new CMString();
                market.GetName(name);
                return name.StringValue;
            }
        }

        public string Currency
        {
            get
            {
                using var instrument = CSMInstrument.GetInstance(code);
                int currency = instrument.GetCurrency();
                using var isoCode = new CMString();
                CSMCurrency.CurrencyToString(currency, isoCode);
                return isoCode.StringValue;
            }
        }

        public string Type
        {
            get
            {
                using var instrument = CSMInstrument.GetInstance(code);
                return instrument.GetType_API().ToString();
            }
        }
    }
}
