/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="using:ProjectForTemplates.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MangAppClient.Core.Services;
using Microsoft.Practices.ServiceLocation;
using Windows.UI.Xaml.Controls;

namespace MangAppClient.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<ILocalData, Design.MockDatabase>();
                SimpleIoc.Default.Register<IWebData, Design.MockService>();
            }
            else
            {
                SimpleIoc.Default.Register<ILocalData, LocalData>();
                SimpleIoc.Default.Register<IWebData, WebData>();
            }

            RegisterViewModels();
        }

        private static void RegisterViewModels()
        {   
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MangaDetailViewModel>();
        }

        /// <summary>
        /// Gets the Main view model.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Gets the Manga Detail view model
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MangaDetailViewModel MangaDetail
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MangaDetailViewModel>();
            }
        }

        public static void RegisterRootFrame(Frame frame)
        {
            SimpleIoc.Default.Register<Frame>(() => frame);
        }

        public static Frame GetRootFrame()
        {
            return SimpleIoc.Default.GetInstance<Frame>();
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}