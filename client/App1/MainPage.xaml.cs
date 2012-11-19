using MangAppClient.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void CreateInitialDbClick(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            database.CreateInitialDb();
        }

        private void GetMangaDetailAsyncClick(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            var mangas = database.GetMangaList();
            
            if (mangas.Count() > 0)
            {
                Requests request = new Requests();
                var mangaDetails = request.GetMangaDetail(mangas.First().Key);

                this.TitleTB.Text = mangaDetails.Title;
                this.DescriptionTB.Text = mangaDetails.Description;
            }
        }

        private void GetChapter(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            var mangas = database.GetMangaList();

            if (mangas.Count() > 0)
            {
                Requests request = new Requests();
                var mangaDetails = request.GetMangaDetail(mangas.Skip(10).First().Id);
                var chapter = request.GetChapter(mangaDetails.Key, mangaDetails.Chapters.First().Key);

                this.TitleTB.Text = chapter.Number.ToString();
                this.DescriptionTB.Text = chapter.Title;
            }
        }
    }
}
