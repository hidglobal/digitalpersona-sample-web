using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Base class for PasswordManager data classes.
    /// </summary>
    [DataContract]
    public abstract class DataObject : INotifyPropertyChanged
    {
        #region Bindable base

        /// <summary>
        /// Multicast event for property change notifications.
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        ///             notifies listeners only when necessary.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam><param name="storage">Reference to a property with both getter and setter.</param><param name="value">Desired value for the property.</param><param name="propertyName">Name of the property used to notify listeners.  This
        ///             value is optional and can be provided automatically when invoked from compilers that
        ///             support CallerMemberName.</param>
        /// <returns>
        /// True if the value was changed, false if the existing value matched the
        ///             desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// 
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        ///             value is optional and can be provided automatically when invoked from compilers
        ///             that support <see cref="T:System.Runtime.CompilerServices.CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler changedEventHandler = this.PropertyChanged;
            if (changedEventHandler == null)
                return;
            changedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
