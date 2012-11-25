namespace MangAppClient.Core.Model
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    // TODO: add code to raise a property changed using an Expression
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetValue<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (!object.Equals(field, newValue))
            {
                field = newValue;
                this.RaisePropertyChanged(propertyName);

                return true;
            }

            return false;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            var temp = this.PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
