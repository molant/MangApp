using GalaSoft.MvvmLight;
using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MangAppClient.ViewModel
{
    public class MangaDetailViewModel : MangAppViewModelBase
    {
        private Manga manga;
        private IWebRequests service;
        private ILocalRequests dataBase;
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
                return new Uri(Path.Combine(ApplicationData.Current.LocalFolder.Path, Manga.LocalSummaryImage));
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

        public MangaDetailViewModel(IWebRequests service, ILocalRequests dataBase)
        {
            this.service = service;
            this.dataBase = dataBase;

            /* MOCKING! */
            //Manga = new Manga()
            //{
            //    Title = "Awesome manga",
            //    LocalSummaryImage = "ms-appx:/Assets/SOUL_EATER-Portada.jpg",
            //    ArtistsDb = string.Join("#", new List<string>() { "Awesome author 1", "Awesome author 2", "Awesome author 1" }),
            //    Description = "You think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man. /nYou think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man.",
            //    YearOfRelease = 9999,
            //    LastChapter = 10,
            //    StatusDb = 1,
            //};

            //var chapters = new List<Chapter>();
            //for (int i = 1; i <= 50; i++)
            //    chapters.Add(new Chapter() { Title = "Chapter" + i, Number = i });
            //this.dataBase.CreateInitialDb();
            Manga = dataBase.GetMangaList().First();
            service.GetMangaChapters(Manga);

            //LoadData();
        }

        private void LoadData()
        {
            service.GetMangaChapters(Manga);

            var imageUri = dataBase.GetBackgroundImage(Manga);            
            if(imageUri == null)
                imageUri = dataBase.GetDefaultBackgroundImage();

            Background = new BitmapImage(new Uri(Path.Combine(ApplicationData.Current.LocalFolder.Path, imageUri)));
        }
    }
}
