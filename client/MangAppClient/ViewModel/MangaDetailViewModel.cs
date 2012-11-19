using GalaSoft.MvvmLight;
using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MangAppClient.ViewModel
{
    public class MangaDetailViewModel : MangAppViewModelBase
    {
        private Manga manga;
        private IWebRequests service;
        private ObservableCollection<Chapter> chapters;

        public string Title
        {
            get
            {
                return manga.Title;
            }

            set
            {
                manga.Title= value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Chapter> Chapters
        {
            get
            {
                if (chapters == null)
                {
                    chapters = new ObservableCollection<Chapter>(manga.Chapters);
                }

                return chapters;
            }

            set
            {
                chapters = value;
                RaisePropertyChanged();
            }
        }

        public Uri Image
        {
            get
            {
                return manga.RemoteSummaryImage;
            }
        }

        public string Description 
        {
            get
            {
                return manga.Description;
            }
            set
            {
                manga.Description = value;
                RaisePropertyChanged();
            }
        }

        public MangaDetailViewModel(IWebRequests service)
        {
            this.service = service;
            LoadData();
        }

        private async void LoadData()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                service.GetMangaChapters(manga);
            }
            else
            {
                service.GetMangaChapters(manga);
            }
        }
    }
}
