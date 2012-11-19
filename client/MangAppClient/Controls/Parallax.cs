using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MangAppClient.Controls
{
    public class Parallax : Canvas
    {
        private Image background;
        private CompositeTransform backgroundTransform;

        private const int BackGroundOffset = 100;

        public int ScrollWidth
        {
            get { return (int)GetValue(ScrollWidthProperty); }
            set { SetValue(ScrollWidthProperty, value); }
        }

        public static readonly DependencyProperty ScrollWidthProperty =
            DependencyProperty.Register("ScrollWidth", typeof(int), typeof(Parallax), new PropertyMetadata(0));

        public double ScrollHorizontalOffset
        {
            get { return (double)GetValue(ScrollHorizontalOffsetProperty); }
            set 
            {
                SetValue(ScrollHorizontalOffsetProperty, value); 
            }
        }

        public static readonly DependencyProperty ScrollHorizontalOffsetProperty = DependencyProperty.Register("ScrollHorizontalOffset", typeof(double), typeof(Parallax), new PropertyMetadata(0, OnScrollHorizontalOffsetChanged));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(Parallax), new PropertyMetadata(0, OnImageSourceChanged));

        public Parallax()
        {
            background = new Image();
            backgroundTransform = new CompositeTransform();
            background.RenderTransform = backgroundTransform;
            background.Stretch = Stretch.None;

            Children.Add(background);
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            background.Width = ActualWidth + BackGroundOffset;
            SetLeft(background, 0);
        }

        private static void OnScrollHorizontalOffsetChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var parallax = dependencyObject as Parallax;
            var scrollHorizontalOffset = (double)eventArgs.NewValue;
            var translate = (scrollHorizontalOffset * BackGroundOffset) / parallax.ScrollWidth;
            parallax.backgroundTransform.TranslateX = -translate;
        }

        private static void OnImageSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var parallax = dependencyObject as Parallax;
            parallax.background.Source = eventArgs.NewValue as ImageSource;
        }
    }
}
