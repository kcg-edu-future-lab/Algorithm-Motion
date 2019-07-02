using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoBalance1Wpf
{
    public class AppModel
    {
        const int PointsCount = 12;
        const double MoveInterval = 1.0;
        const double Fps = 30;

        internal static Random Random { get; } = new Random();

        public PointObject[] Points { get; }
        Queue<(DateTime time, PointObject point)> TimeQueue;

        Timer FrameTimer;

        public AppModel()
        {
            Points = Enumerable.Range(0, PointsCount)
                .Select(_ => 360 * Random.NextDouble())
                .OrderBy(x => x)
                .Select((x, i) => new PointObject(i, x))
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
            var now = DateTime.Now;

            var countToUpdate = TimeQueue.TakeWhile(_ => _.time < now).Count();
            for (var i = 0; i < countToUpdate; i++)
            {
                var (time, point) = TimeQueue.Dequeue();
                TimeQueue.Enqueue((time.AddSeconds(MoveInterval), point));

                UpdatePoint(point);
            }
        }

        void UpdatePoint(PointObject p)
        {
            var angle1 = GetAngleOnMinusSide();
            var angle2 = GetAngleOnPlusSide();
            p.Angle = (angle1 + angle2) / 2;

            double GetAngleOnMinusSide()
            {
                var angle = Points[(p.Id - 1 + PointsCount) % PointsCount].Angle;
                var angle_ = angle - 360;
                return (Math.Abs(p.Angle - angle) < Math.Abs(p.Angle - angle_)) ?
                    angle : angle_;
            }

            double GetAngleOnPlusSide()
            {
                var angle = Points[(p.Id + 1) % PointsCount].Angle;
                var angle_ = angle + 360;
                return (Math.Abs(p.Angle - angle) < Math.Abs(p.Angle - angle_)) ?
                    angle : angle_;
            }
        }
    }
}
