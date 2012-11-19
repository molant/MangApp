using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangAppClient.Core.Model;
using System.Collections.ObjectModel;

namespace MangAppClient.ViewModel
{
    public class MangaGroupViewModel : MangAppViewModelBase, IComparable<MangaGroupViewModel>
    {
        public string Key
        { get; set; }

        public ObservableCollection<MangaSummaryViewModel> GroupItems
        {
            get;
            set;
        }

        public int CompareTo(MangaGroupViewModel other)
        {
            return this.Key.CompareTo(other.Key);
        }
    }
}
