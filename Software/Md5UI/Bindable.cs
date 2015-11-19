using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public abstract class Bindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void InvokePropertyChanged([CallerMemberName] string callerName = null)
        {
            var p = PropertyChanged;

            if (p != null)
            {
                p(this, new PropertyChangedEventArgs(callerName));
            }
        }

        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string callerName = null)
        {
            property = value;
            InvokePropertyChanged(callerName);
        }
    }
}
