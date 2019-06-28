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
        const double Fps = 30;

        internal static Random Random { get; } = new Random();

        public PointObject[] Points { get; }

        OrderedList<PointObject, DateTime> SignalTimes;
        OrderedList<PointObject, DateTime> ThinkingTimes;
        Timer FrameTimer;

        public AppModel()
        {
            var now = DateTime.Now;
            var angleDelta = 360.0 / PointsCount;
            Points = Enumerable.Range(0, PointsCount)
                .Select(_ => now.AddSeconds(2 + SignalInterval * Random.NextDouble()))
                .Select((t, i) => new PointObject(i, i * angleDelta)
                {
                    NextSignalTime = t,
                    NextThinkingTime = t.AddSeconds(SignalInterval / 2),
                })
                .ToArray();

            SignalTimes = new OrderedList<PointObject, DateTime>(p => p.NextSignalTime);
            ThinkingTimes = new OrderedList<PointObject, DateTime>(p => p.NextThinkingTime);

            FrameTimer = new Timer(UpdateFrame, null, TimeSpan.Zero, TimeSpan.FromSeconds(1 / Fps));
        }

        void UpdateFrame(object state)
        {
            var now = DateTime.Now;

            var countToThink = ThinkingTimes.TakeWhile(_ => _.key < now).Count();
            for (var i = 0; i < countToThink; i++)
            {
                var point = ThinkingTimes[0].item;
                ThinkingTimes.RemoveAt(0);

                UpdateThinking(point);
            }

            var countToSignal = SignalTimes.TakeWhile(_ => _.key < now).Count();
            for (var i = 0; i < countToSignal; i++)
            {
                var point = SignalTimes[0].item;
                SignalTimes.RemoveAt(0);

                UpdateSignal(point);
            }
        }

        void UpdateThinking(PointObject p)
        {
        }

        void UpdateSignal(PointObject p)
        {
        }
    }
}
