//  Copyright (c) RXD Solutions. All rights reserved.
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class GetInstrumentPropertyFunctions
    {
        [ExcelFunction(Name = "GETINSTRUMENTPROPERTY",
                       Description = "Returns an Instrument Property",
                       HelpTopic = "Get-Instrument-Property")]
        public static object GetInstrumentProperty(
            [ExcelArgument(Name = "instrument", Description = "The instrument id or reference")]object instrument,
            [ExcelArgument(Name = "property", Description = "The instrument property to subscribe to")]string property)
        {
            return ExcelAsyncUtil.Observe(nameof(GetInstrumentProperty), new object[] { instrument, property }, () => new InstrumentPropertyExcelObservable(instrument, property, AddIn.Client));
        }
    }
}
