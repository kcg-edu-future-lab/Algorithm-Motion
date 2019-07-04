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

namespace AutoBalance1Wpf
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

        void Circle_Loaded(object sender, RoutedEventArgs e)
        {
            var ellipse = (Ellipse)sender;
            var point = (PointObject)ellipse.DataContext;

            var (rotate, _) = CreateRotateAnimation("AngleTransform", point.Angle, 0.5 + AppModel.Random.NextDouble());
            rotate.Begin(ellipse);

            var (rotateForChange, frame) = CreateRotateAnimation("AngleTransform");

            point.PropertyChanged += (o, pe) =>
            {
                if (pe.PropertyName != nameof(point.Angle)) return;

                Dispatcher.InvokeAsync(() =>
                {
                    frame.Value = point.Angle;
                    rotateForChange.Begin(ellipse);
                });
            };
        }

        static (Storyboard, DoubleKeyFrame) CreateRotateAnimation(string rotateTransformName, double angle = 0, double time = 0.4)
        {
            var storyboard = new Storyboard();
            var frames = new DoubleAnimationUsingKeyFrames();
            var frame = new EasingDoubleKeyFrame(angle, TimeSpan.FromSeconds(time), new CubicEase { EasingMode = EasingMode.EaseOut });

            // Storyboard.SetTarget を使えるのは FrameworkElement のとき。
            Storyboard.SetTargetName(frames, rotateTransformName);
            Storyboard.SetTargetProperty(frames, new PropertyPath(RotateTransform.AngleProperty.Name));
            frames.KeyFrames.Add(frame);
            storyboard.Children.Add(frames);

            return (storyboard, frame);
        }
    }
}
