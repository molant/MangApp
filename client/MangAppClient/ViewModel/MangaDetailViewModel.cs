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
                    chapters = new ObservableCollection<Chapter>();

                    for (int i = 0; i < 10; i++)
                        chapters.Add(new Chapter() { Title = "Chapter" + 1, });
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

            set
            {
                manga.SummaryImageUrl = value;
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

        public MangaDetailViewModel(IRequests service)
        {
            this.service = service;
            LoadData();
        }

        private async void LoadData()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                manga = await service.GetMangaDetailAsync("1");
            }
            else
            {
                manga = await service.GetMangaDetailAsync("1");
            }
        }

        
    }
}
