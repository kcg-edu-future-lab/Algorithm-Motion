using System;
using System.Collections.Generic;
using System.Linq;
using AlgoMotionLib;

namespace AutoBalance1Wpf
{
    public class AppModel : NotifyBase
    {
        const int PointsCount = 12;
        static readonly TimeSpan MoveInterval = TimeSpan.FromSeconds(1.0);
        static readonly TimeSpan TimerInterval = TimeSpan.FromSeconds(1.0 / 30);

        public PointObject[] Points { get; }
        Queue<PointObject> MoveTimes;

        public FrameTimer FrameTimer { get; }

        public AppModel()
        {
            Points = Enumerable.Range(0, PointsCount)
                .Select(_ => NumberHelper.NextDouble(0, 360))
                .OrderBy(x => x)
                .Select((x, i) => new PointObject(i, x)
                {
                    NextMoveTime = TimeSpan.FromSeconds(NumberHelper.NextDouble(2, 2 + MoveInterval.TotalSeconds)),
                })
                .ToArray();

            MoveTimes = new Queue<PointObject>(Points.OrderBy(_ => _.NextMoveTime));

            FrameTimer = new FrameTimer(UpdateFrame, TimerInterval);
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
                .Average(a => CorrectAngle(a));

            double CorrectAngle(double a) => a > p.Angle + 180 ? a - 360 : a < p.Angle - 180 ? a + 360 : a;
        }
    }
}
