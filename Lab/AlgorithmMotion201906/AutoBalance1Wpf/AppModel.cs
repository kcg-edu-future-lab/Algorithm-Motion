using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AutoBalance1Wpf
{
    public class AppModel : NotifyBase
    {
        const int PointsCount = 12;
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
                .Select(_ => 360 * NumberHelper.Random.NextDouble())
                .OrderBy(x => x)
                .Select((x, i) => new PointObject(i, x)
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
            p.Angle = new[] { -1, 1 }
                .Select(i => Points[(p.Id + i).Mod(PointsCount)].Angle)
                .Average(a => a > p.Angle + 180 ? a - 360 : a < p.Angle - 180 ? a + 360 : a);
        }
    }
}
