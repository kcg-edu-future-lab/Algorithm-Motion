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
using AlgoMotionLib;

namespace FillBalance2Wpf
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

            var (move, _, _) = CreateMoveAnimation("PositionTransform", point.Position.X, point.Position.Y, NumberHelper.NextDouble(0.5, 1.5));
            move.Begin(ellipse);

            var (moveForChange, frameX, frameY) = CreateMoveAnimation("PositionTransform");

            point.PropertyChanged += (o, pe) =>
            {
                if (pe.PropertyName != nameof(point.Position)) return;

                Dispatcher.InvokeAsync(() =>
                {
                    frameX.Value = point.Position.X;
                    frameY.Value = point.Position.Y;
                    moveForChange.Begin(ellipse);
                });
            };
        }

        static (Storyboard, DoubleKeyFrame, DoubleKeyFrame) CreateMoveAnimation(string translateTransformName, double x = 0, double y = 0, double time = 0.4)
        {
            var storyboard = new Storyboard();

            var framesX = new DoubleAnimationUsingKeyFrames();
            var frameX = new EasingDoubleKeyFrame(x, TimeSpan.FromSeconds(time), new CubicEase { EasingMode = EasingMode.EaseOut });
            Storyboard.SetTargetName(framesX, translateTransformName);
            Storyboard.SetTargetProperty(framesX, new PropertyPath(TranslateTransform.XProperty.Name));
            framesX.KeyFrames.Add(frameX);
            storyboard.Children.Add(framesX);

            var framesY = new DoubleAnimationUsingKeyFrames();
            var frameY = new EasingDoubleKeyFrame(y, TimeSpan.FromSeconds(time), new CubicEase { EasingMode = EasingMode.EaseOut });
            Storyboard.SetTargetName(framesY, translateTransformName);
            Storyboard.SetTargetProperty(framesY, new PropertyPath(TranslateTransform.YProperty.Name));
            framesY.KeyFrames.Add(frameY);
            storyboard.Children.Add(framesY);

            return (storyboard, frameX, frameY);
        }
    }
}
