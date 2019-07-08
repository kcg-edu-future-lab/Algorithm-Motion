using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AlgoMotionLib;

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

        public FrameTimer FrameTimer { get; }

        public AppModel()
        {
            Points = Enumerable.Range(0, PointsCount)
                .Select(i => new PointObject(i, new Vector(NumberHelper.NextDouble(-300, 300), NumberHelper.NextDouble(-300, 300)))
                {
                    NextMoveTime = TimeSpan.FromSeconds(NumberHelper.NextDouble(2, 2 + MoveInterval.TotalSeconds)),
                })
                .ToArray();

            MoveTimes = new Queue<PointObject>(Points.OrderBy(_ => _.NextMoveTime));

            FrameTimer = new FrameTimer(UpdateFrame, TimeSpan.FromSeconds(1 / Fps));
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
