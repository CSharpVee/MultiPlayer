using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MultiPlayer.UILements
{
    /// <summary>
    /// Interaction logic for ImageButton_CODED.xaml
    /// </summary>
    public partial class ImageButton : UserControl
    {
        public static readonly DependencyProperty ImageSourceProperty =
                    DependencyProperty.Register("ImageSource", typeof(string), typeof(ImageButton), new PropertyMetadata(default(string), OnImageSourceChanged));

        public static readonly DependencyProperty SourceRectProperty =
            DependencyProperty.Register("SourceRect", typeof(Int32Rect), typeof(ImageButton), new PropertyMetadata(default(Int32Rect), OnSourceRectChanged));

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public Int32Rect SourceRect
        {
            get { return (Int32Rect)GetValue(SourceRectProperty); }
            set { SetValue(SourceRectProperty, value); }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click",RoutingStrategy.Bubble,typeof(RoutedEventHandler),typeof(ImageButton));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }


        public ImageButton()
        {
            InitializeComponent();

            button.Click += (object sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (ImageButton)d;
            if (button.image != null)
            {
                var uri = new Uri(button.ImageSource, UriKind.RelativeOrAbsolute);
                var bitmapImage = new BitmapImage(uri);
                if (button.SourceRect != default(Int32Rect))
                {
                    var croppedBitmap = new CroppedBitmap(bitmapImage, button.SourceRect);
                    button.image.Source = croppedBitmap;
                }
                else
                {
                    button.image.Source = bitmapImage;
                }
            }
        }

        private static void OnSourceRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (ImageButton)d;
            if (button.ImageSource != null && button.image != null && button.SourceRect != default(Int32Rect))
            {
                var uri = new Uri(button.ImageSource, UriKind.RelativeOrAbsolute);
                var bitmapImage = new BitmapImage(uri);
                var croppedBitmap = new CroppedBitmap(bitmapImage, button.SourceRect);
                button.image.Source = croppedBitmap;
            }
        }
    }
}
