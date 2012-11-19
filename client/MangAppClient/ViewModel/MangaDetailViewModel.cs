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
        private ObservableCollection<ChapterSummary> chapters;
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
        public ObservableCollection<ChapterSummary> Chapters
        {
            get
            {
                if (chapters == null)
                {
                    chapters = new ObservableCollection<Chapter>(manga.Chapters);
                    chapters = new ObservableCollection<ChapterSummary>(manga.Chapters);
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
                return manga.SummaryImageUrl;
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
                MockData();
            }
            else
            {
                MockData();
            }
        }

        private void MockData()
        {
            manga = service.GetMangaDetail("1");
            Background = dataBase.GetBackgroundImage("1");
            if (Background == null)
                Background = dataBase.GetDefaultBackgroundImage();
        }
    }
}
