using GalaSoft.MvvmLight;
using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.ViewModel
{
    public class MangaDetailViewModel : MangAppViewModelBase
    {
        private Manga manga;
        private IRequests service;

        public string Name
        {
            get
            {
                return manga.Name;
            }

            set
            {
                manga.Name = value;
                RaisePropertyChanged();
            }
        }

        public MangaDetailViewModel(IRequests service)
        {
            this.service = service;
            LoadData();
        }

        private async void LoadData()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                manga = await service.GetMangaDetailAsync(1);
            }
            else
            {
                manga = await service.GetMangaDetailAsync(1);
            }
        }

        
    }
}
