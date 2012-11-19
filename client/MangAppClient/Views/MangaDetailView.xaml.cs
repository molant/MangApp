using MangAppClient.Common;
using MangAppClient.Core.Model;
using MangAppClient.ViewModel;
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

namespace MangAppClient.Views
{
    public sealed partial class MangaDetailView : LayoutAwarePage
    {


        public string MangaId
        {
            get { return (string)GetValue(MangaIdProperty); }
            set { SetValue(MangaIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MangaId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MangaIdProperty =
            DependencyProperty.Register("MangaId", typeof(string), typeof(MangaDetailView), new PropertyMetadata(String.Empty));

        public MangaDetailView()
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
            if (e.NavigationMode != NavigationMode.New)
            {
                var manga = e.Parameter as Manga;
                var viewModel = DataContext as MangaDetailViewModel;
                viewModel.Manga = manga;
            }
        }
    }
}
