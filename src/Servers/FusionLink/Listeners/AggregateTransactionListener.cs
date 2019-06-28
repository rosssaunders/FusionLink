//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink
{
    public class AggregateTransactionListener : ITransactionListener
    {
        private readonly TransactionActionListener _actionListener;
        private readonly TransactionEventListener _eventListener;

        public event EventHandler<TransactionChangedEventArgs> TransactionChanged;

        public AggregateTransactionListener(TransactionActionListener actionListener, TransactionEventListener eventListener)
        {
            _actionListener = actionListener;
            _eventListener = eventListener;

            TransactionActionListener.TransactionChanged += OnTransactionChanged;
            TransactionEventListener.TransactionChanged += OnTransactionChanged;
        }

        private void OnTransactionChanged(object sender, TransactionChangedEventArgs e)
        {
            TransactionChanged?.Invoke(this, e);
        }
    }
}