//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.ServiceModel;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.Client
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class DataServiceCallbackClient : IRealTimeCallbackClient
    {
        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<FlatPositionValueReceivedEventArgs> OnFlatPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;
        public event EventHandler<PortfolioPropertyReceivedEventArgs> OnPortfolioPropertyReceived;
        public event EventHandler<InstrumentPropertyReceivedEventArgs> OnInstrumentPropertyReceived;
        public event EventHandler<CurrencyPropertyReceivedEventArgs> OnCurrencyPropertyReceived;
        public event EventHandler<SystemValueReceivedEventArgs> OnSystemValueReceived;
        public event EventHandler<ServiceStatusReceivedEventArgs> OnServiceStatusReceived;

        public DateTime LastReceivedMessageTimestamp { get; private set; }

        public void Heartbeat()
        {
            //DoNothing
        }

        public void SendPortfolioProperty(int portfolioId, PortfolioProperty property, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnPortfolioPropertyReceived?.Invoke(this, new PortfolioPropertyReceivedEventArgs(portfolioId, property, value));
        }

        public void SendInstrumentProperty(object instrument, string property, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnInstrumentPropertyReceived?.Invoke(this, new InstrumentPropertyReceivedEventArgs(instrument, property, value));
        }

        public void SendPortfolioValue(int portfolioId, string column, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnPortfolioValueReceived?.Invoke(this, new PortfolioValueReceivedEventArgs(portfolioId, column, value));
        }

        public void SendPositionValue(int positionId, string column, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnPositionValueReceived?.Invoke(this, new PositionValueReceivedEventArgs(positionId, column, value));
        }

        public void SendFlatPositionValue(int portfolioId, int instrumentId, string column, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnFlatPositionValueReceived?.Invoke(this, new FlatPositionValueReceivedEventArgs(portfolioId, instrumentId, column, value));
        }

        public void SendServiceStaus(ServiceStatus status)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnServiceStatusReceived?.Invoke(this, new ServiceStatusReceivedEventArgs(status));
        }

        public void SendSystemValue(SystemProperty property, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnSystemValueReceived?.Invoke(this, new SystemValueReceivedEventArgs(property, value));
        }

        public void SendCurrencyProperty(object id, string propertyName, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnCurrencyPropertyReceived?.Invoke(this, new CurrencyPropertyReceivedEventArgs(id, propertyName, value));
        }
    }
}