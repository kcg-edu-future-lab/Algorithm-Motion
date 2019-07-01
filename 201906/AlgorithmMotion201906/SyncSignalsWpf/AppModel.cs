using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SyncSignalsWpf
{
    public class AppModel
    {
        const int PointsCount = 12;
        const double SignalInterval = 1.0;
        const double Fps = 50;

        internal static Random Random { get; } = new Random();

        public PointObject[] Points { get; }
        OrderedList<PointObject, TimeSpan> SignalTimes;
        OrderedList<PointObject, TimeSpan> ThinkingTimes;

        public DateTime StartTime { get; }
        Timer FrameTimer;

        public AppModel()
        {
            StartTime = DateTime.Now;

            var angleInterval = 360.0 / PointsCount;
            Points = Enumerable.Range(0, PointsCount)
                .Select(_ => TimeSpan.FromSeconds(1 + SignalInterval * Random.NextDouble()))
                .Select((t, i) => new PointObject(i, i * angleInterval)
                {
                    NextSignalTime = t,
                    NextThinkingTime = t + TimeSpan.FromSeconds(SignalInterval / 2),
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
            p.PreviousSignalTime = p.NextSignalTime;
        }

        void UpdateThinking(PointObject p)
        {
            var times = Enumerable.Range(p.Id - 1, 3)
                .Select(id => Points[(id + PointsCount) % PointsCount].PreviousSignalTime)
                .ToArray();

            if (times.Any(t => t == TimeSpan.Zero))
            {
                p.NextSignalTime = p.PreviousSignalTime + TimeSpan.FromSeconds(SignalInterval);
            }
            else
            {
                var averageTicks = (long)times.Average(t => t.Ticks);
                p.NextSignalTime = new TimeSpan(averageTicks) + TimeSpan.FromSeconds(SignalInterval);
            }

            p.NextThinkingTime = p.NextSignalTime + TimeSpan.FromSeconds(SignalInterval / 2);
        }
    }
}
