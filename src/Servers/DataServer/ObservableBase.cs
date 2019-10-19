//  Copyright (c) RXD Solutions. All rights reserved.


using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RxdSolutions.FusionLink
{
    public class ObservableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}