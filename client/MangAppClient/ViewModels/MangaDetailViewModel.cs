using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangAppClient.Core.Model;
using Caliburn.Micro;

namespace MangAppClient.ViewModels
{
    public class MangaDetailViewModel : ViewModelBase
    {
        private Manga _manga = new Manga() { Name = "Manga molon" };

        public MangaDetailViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        public string Name 
        {
            get
            {
                return _manga.Name;
            }
            set
            {
                _manga.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }
    }
}
