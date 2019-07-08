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
        const int PointsCount = 36;
        const int NeighborsCount = 8;
        static readonly TimeSpan MoveInterval = TimeSpan.FromSeconds(1.0);
        const double Fps = 30;

        public PointObject[] Points { get; }
        Queue<PointObject> MoveTimes;

        public DateTime StartTime { get; } = DateTime.Now;
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
                .Select(i => new PointObject(i, new Vector(NumberHelper.NextDouble(-300, 300), NumberHelper.NextDouble(-300, 300)))
                {
                    NextMoveTime = TimeSpan.FromSeconds(NumberHelper.NextDouble(2, 2 + MoveInterval.TotalSeconds)),
                })
                .ToArray();

            MoveTimes = new Queue<PointObject>(Points.OrderBy(_ => _.NextMoveTime));

            FrameTimer = new Timer(o => MeasureTime(UpdateFrame), null, TimeSpan.Zero, TimeSpan.FromSeconds(1 / Fps));
        }

        void MeasureTime(Action<TimeSpan> action)
        {
            lock (FrameTimer)
            {
                FrameWatch.Restart();
                action(DateTime.Now - StartTime);
                FrameWatch.Stop();
                ActualFrameTime = FrameWatch.Elapsed.TotalMilliseconds;
            }
        }

        void UpdateFrame(TimeSpan now)
        {
            var countToUpdate = MoveTimes.TakeWhile(_ => _.NextMoveTime < now).Count();
            for (var i = 0; i < countToUpdate; i++)
            {
                var point = MoveTimes.Dequeue();
                point.NextMoveTime += MoveInterval;
                MoveTimes.Enqueue(point);

                UpdatePoint(point);
            }
        }

        void UpdatePoint(PointObject p)
        {
            var neighbors = Points
                .Where(q => q.Id != p.Id)
                .OrderBy(q => (q.Position - p.Position).LengthSquared)
                .Take(NeighborsCount)
                .ToArray();
            var radius = neighbors
                .Average(q => q.Position.Length);
            var angle = p.Angle + neighbors
                .Select(q => q.Angle)
                .Select(CorrectAngle)
                .Sum(a => GetRepulsion(a - p.Angle));

            p.Position = new Vector(radius * Math.Cos(angle * Math.PI / 180), radius * Math.Sin(angle * Math.PI / 180));

            double CorrectAngle(double a) => a > p.Angle + 180 ? a - 360 : a < p.Angle - 180 ? a + 360 : a;
            double GetRepulsion(double d) => d == 0 ? 0 : -2000.0 / PointsCount / (d + Math.Sign(d) * PointsCount / 6.0);
        }
    }
}
