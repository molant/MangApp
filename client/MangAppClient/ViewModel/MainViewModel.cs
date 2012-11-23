using GalaSoft.MvvmLight;
using MangAppClient.Core.Model;
using System.Collections.ObjectModel;
using MangAppClient.Core.Services;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using System;
using GalaSoft.MvvmLight.Command;
using MangAppClient.Views;

namespace MangAppClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : MangAppViewModelBase
    {
        private readonly ILocalData dataBase;
        private object gridViewSelectedItem = null;
        private ObservableCollection<MangaGroupViewModel> mangaGroups;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<MangaGroupViewModel> MangaGroups
        {
            get
            {
                return mangaGroups;
            }

            set
            {
                if (mangaGroups == value)
                {
                    return;
                }

                mangaGroups = value;
                RaisePropertyChanged();
            }
        }

        public Uri BackgroundImage
        {
            get
            {
                return new Uri(this.dataBase.GetDefaultBackgroundImage());
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ILocalData dataBase)
        {
            this.dataBase = dataBase;
            this.mangaGroups = new ObservableCollection<MangaGroupViewModel>();
            //this.dataBase.CreateInitialDb();
            LoadMangaList();
        }

        private async void LoadMangaList()
        {
            IEnumerable<Manga> summaries;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                summaries = (this.dataBase as Design.MockDatabase).GetMangaListDesign();
            }
            else
            {
                summaries = this.dataBase.GetMangaList().OrderBy(s => s.Popularity);
            }

            var genreList = summaries.SelectMany(s => s.Categories).Distinct();
            var mangaGroupList = new List<MangaGroupViewModel>();

            foreach (var genre in genreList)
            {
                var group = new MangaGroupViewModel
                {
                    Key = genre
                };

                group.GroupItems = new ObservableCollection<MangaSummaryViewModel>();
                foreach(var manga in summaries.Where(s => s.Categories.Contains(genre)).Take(6))
                {
                    group.GroupItems.Add(new MangaSummaryViewModel(manga));
                }

                mangaGroupList.Add(group);
            }

            var latestGroup = new MangaGroupViewModel
            {
                Key = "Latest",
                GroupItems = summaries.OrderByDescending(s => s.LastChapterDate).Take(10).Select(s => { return new MangaSummaryViewModel(s); }).ToObservableCollection()
            };
            MangaGroups.Add(latestGroup);

            var popularGroup = new MangaGroupViewModel
            {
                Key = "Popular",
                GroupItems = summaries.Take(6).Select(s => { return new MangaSummaryViewModel(s); }).ToObservableCollection()
            };
            MangaGroups.Add(popularGroup);

            mangaGroupList.Sort();
            MangaGroups = MangaGroups.Concat(mangaGroupList).ToObservableCollection();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}