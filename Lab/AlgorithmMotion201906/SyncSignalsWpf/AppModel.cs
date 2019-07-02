using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SyncSignalsWpf
{
    public class AppModel
    {
        const int PointsCount = 12;
        const int NeighborsCount = 3;
        static readonly TimeSpan SignalInterval = TimeSpan.FromSeconds(1.0);
        static readonly TimeSpan ThinkingOffset = TimeSpan.FromSeconds(SignalInterval.TotalSeconds / 2);
        const double Fps = 50;

        internal static Random Random { get; } = new Random();

        public PointObject[] Points { get; }
        OrderedList<PointObject, TimeSpan> SignalTimes;
        OrderedList<PointObject, TimeSpan> ThinkingTimes;

        public DateTime StartTime { get; } = DateTime.Now;
        Timer FrameTimer;

        public AppModel()
        {
            var angleInterval = 360.0 / PointsCount;
            Points = Enumerable.Range(0, PointsCount)
                .Select(_ => TimeSpan.FromSeconds(1 + SignalInterval.TotalSeconds * Random.NextDouble()))
                .Select((t, i) => new PointObject(i, i * angleInterval)
                {
                    NextSignalTime = t,
                    NextThinkingTime = t + ThinkingOffset,
                })
                .ToArray();

            SignalTimes = new OrderedList<PointObject, TimeSpan>(p => p.NextSignalTime, Points);
            ThinkingTimes = new OrderedList<PointObject, TimeSpan>(p => p.NextThinkingTime, Points);

            FrameTimer = new Timer(UpdateFrame, null, TimeSpan.Zero, TimeSpan.FromSeconds(1 / Fps));
        }

        void UpdateFrame(object state)
        {
            var now = DateTime.Now - StartTime;

            var countToSignal = SignalTimes.TakeWhile(_ => _.key < now).Count();
            for (var i = 0; i < countToSignal; i++)
            {
                var point = SignalTimes[0].item;
                SignalTimes.RemoveAt(0);

                UpdateSignal(point);
            }

            var countToThink = ThinkingTimes.TakeWhile(_ => _.key < now).Count();
            for (var i = 0; i < countToThink; i++)
            {
                var point = ThinkingTimes[0].item;
                ThinkingTimes.RemoveAt(0);

                UpdateThinking(point);

                SignalTimes.AddForOrder(point);
                ThinkingTimes.AddForOrder(point);
            }
        }

        void UpdateSignal(PointObject p)
        {
            p.SignalTime = p.NextSignalTime;
        }

        void UpdateThinking(PointObject p)
        {
            p.NextSignalTime = GetAverageTime(p) + SignalInterval;
            p.NextThinkingTime = p.NextSignalTime + ThinkingOffset;
        }

        TimeSpan GetAverageTime(PointObject p)
        {
            var times = Enumerable.Range(p.Id - NeighborsCount / 2 + PointsCount, NeighborsCount)
                .Select(id => Points[id % PointsCount].SignalTime)
                .ToArray();
            return times.Any(t => t == TimeSpan.Zero) ?
                p.SignalTime :
                new TimeSpan((long)times.Average(t => t.Ticks));
        }
    }
}
