using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace AutoBalance2Wpf
{
    public class AppModel : NotifyBase
    {
        const int PointsCount = 24;
        const double MoveInterval = 1.0;
        const double Fps = 30;

        internal static Random Random { get; } = new Random();

        public PointObject[] Points { get; }
        Queue<(DateTime time, PointObject point)> TimeQueue;

        Timer FrameTimer;
        Stopwatch FrameWatch = new Stopwatch();

        double _ActualFrameTime;

        public double ActualFrameTime
        {
            get { return _ActualFrameTime; }
            set
            {
                if (_ActualFrameTime == value) return;
                _ActualFrameTime = value;
                NotifyPropertyChanged();
            }
        }

        public AppModel()
        {
            Points = Enumerable.Range(0, PointsCount)
                .Select(i => new PointObject(i, new Vector(-300 + 600 * Random.NextDouble(), -300 + 600 * Random.NextDouble())))
                .ToArray();

            var now = DateTime.Now;
            var queueQuery = Points
                .Select(o => (time: now.AddSeconds(2 + MoveInterval * Random.NextDouble()), point: o))
                .OrderBy(_ => _.time);
            TimeQueue = new Queue<(DateTime, PointObject)>(queueQuery);

            FrameTimer = new Timer(UpdateFrame, null, TimeSpan.Zero, TimeSpan.FromSeconds(1 / Fps));
        }

        void UpdateFrame(object state)
        {
            FrameWatch.Restart();
            var now = DateTime.Now;

            var countToUpdate = TimeQueue.TakeWhile(_ => _.time < now).Count();
            for (var i = 0; i < countToUpdate; i++)
            {
                var (time, point) = TimeQueue.Dequeue();
                TimeQueue.Enqueue((time.AddSeconds(MoveInterval), point));

                UpdatePoint(point);
            }

            FrameWatch.Stop();
            ActualFrameTime = FrameWatch.Elapsed.TotalMilliseconds;
        }

        void UpdatePoint(PointObject p)
        {
            var radius = new[] { -1, 1 }
                .Average(i => Points[(p.Id + i).Mod(PointsCount)].Position.Length);
            var angle = new[] { -1, 1 }
                .Select(i => Points[(p.Id + i).Mod(PointsCount)].Angle)
                .Average(a => a > p.Angle + 180 ? a - 360 : a < p.Angle - 180 ? a + 360 : a);

            p.Position = new Vector(radius * Math.Cos(angle * Math.PI / 180), radius * Math.Sin(angle * Math.PI / 180));
        }
    }
}
