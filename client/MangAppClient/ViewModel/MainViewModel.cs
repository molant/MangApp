using GalaSoft.MvvmLight;
using MangAppClient.Core.Model;
using System.Collections.ObjectModel;
using MangAppClient.Core.Services;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IDatabase dataBase;

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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDatabase dataBase)
        {
            this.dataBase = dataBase;
            this.mangaGroups = new ObservableCollection<MangaGroupViewModel>();
            LoadMangaList();
        }

        private async void LoadMangaList()
        {
            IEnumerable<MangaSummary> summaries;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                summaries = (this.dataBase as Design.MockDatabase).GetMangaListDesign();
            }
            else
            {
                summaries = this.dataBase.GetMangaList();
            }

            var genreList = summaries.SelectMany(s => s.Categories).Distinct();

            foreach (var genre in genreList)
            {
                var group = new MangaGroupViewModel
                {
                    Key = genre
                };

                group.GroupItems = new ObservableCollection<MangaSummaryViewModel>();
                foreach(var manga in summaries.Where(s => s.Categories.Contains(genre)))
                {
                    group.GroupItems.Add(new MangaSummaryViewModel(manga));
                }

                mangaGroups.Add(group);
            }           
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}