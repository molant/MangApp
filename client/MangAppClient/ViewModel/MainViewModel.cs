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
        private IEnumerable<MangaSummary> mangaSummaries;

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
            LoadMangaList();
        }

        private async void LoadMangaList()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                mangaSummaries = (this.dataBase as Design.MockDatabase).GetMangaListDesign();
            }
            else
            {
                mangaSummaries = await this.dataBase.GetMangaList();
            }
            
            MangaGroups = mangaSummaries.GroupBy(m => m.Genre.First())
                .Select(group => new MangaGroupViewModel
                {
                    Key = group.Key,
                    GroupItems = group.ToObservableCollection()
                }).ToObservableCollection();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}