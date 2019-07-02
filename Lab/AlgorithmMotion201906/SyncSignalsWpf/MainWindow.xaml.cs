using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SyncSignalsWpf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void SignalCircle_Loaded(object sender, RoutedEventArgs e)
        {
            var ellipse = (Ellipse)sender;
            var point = (PointObject)ellipse.DataContext;

            var wink = CreateWinkAnimation(ellipse);

            point.PropertyChanged += (o, pe) =>
            {
                if (pe.PropertyName != nameof(point.SignalTime)) return;

                Dispatcher.InvokeAsync(() =>
                {
                    wink.Begin(ellipse);
                });
            };
        }

        static Storyboard CreateWinkAnimation(FrameworkElement element, double opacity = 1, double time = 0.6)
        {
            var storyboard = new Storyboard();
            var frames = new DoubleAnimationUsingKeyFrames();

            Storyboard.SetTarget(frames, element);
            Storyboard.SetTargetProperty(frames, new PropertyPath(UIElement.OpacityProperty.Name));
            frames.KeyFrames.Add(new EasingDoubleKeyFrame(opacity, TimeSpan.Zero));
            frames.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromSeconds(time), new CubicEase { EasingMode = EasingMode.EaseOut }));
            storyboard.Children.Add(frames);

            return storyboard;
        }
    }
}
