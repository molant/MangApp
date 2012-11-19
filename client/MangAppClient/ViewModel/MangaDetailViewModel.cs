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
        private IDatabase dataBase;
        private ObservableCollection<Chapter> chapters;
        private BitmapImage background;
        private string mangaId;

        public Manga Manga
        {
            get
            {
                return manga;
            }

            set
            {
                manga = value;

                LoadData();
                RaisePropertyChanged();
            }
        }

        public string Title
        {
            get
            {
                return Manga.Title;
            }

            set
            {
                Manga.Title= value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Chapter> Chapters
        {
            get
            {
                if (chapters == null)
                {
                    chapters = new ObservableCollection<Chapter>(Manga.Chapters);
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
                return new Uri(Manga.LocalSummaryImage);
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
                return Manga.Description;
            }
            set
            {
                Manga.Description = value;
                RaisePropertyChanged();
            }
        }

        public MangaDetailViewModel(IWebRequests service)
        {
            this.service = service;
            this.dataBase = dataBase;
            LoadData();
        }

        private void LoadData()
        {
            Manga = service.GetMangaDetail(Manga);

            var imageUri = dataBase.GetBackgroundImage(Manga);            
            if(imageUri == null)
                imageUri = dataBase.GetDefaultBackgroundImage();

            Background = new BitmapImage(new Uri(imageUri));
        }
    }
}
