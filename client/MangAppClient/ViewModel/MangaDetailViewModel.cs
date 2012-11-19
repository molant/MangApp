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
        private IRequests service;
        private IDatabase dataBase;
        private ObservableCollection<Chapter> chapters;
        private BitmapImage background;

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
                return new Uri(manga.LocalSummaryImage);
            }
        }

        public BitmapImage Background
        {
            get
            {
                return background;
            }

            set
            {
                background = value;
                RaisePropertyChanged();
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

        public MangaDetailViewModel(IRequests service, IDatabase dataBase)
        {
            this.service = service;
            this.dataBase = dataBase;
            LoadData();
        }

        private void LoadData()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                manga = service.GetMangaDetail(null);
            }
            else
            {
                manga = service.GetMangaDetail(null);
            }
        }
        private void MockData()
        {
            manga = service.GetMangaDetail("1");
            Background = new BitmapImage(new Uri(dataBase.GetBackgroundImage("1")));
            if (Background == null)
                Background = new BitmapImage(new Uri(dataBase.GetBackgroundImage("1")));
        }
    }
}
