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
        Timer FrameTimer;

        public AppModel()
        {
            var angleDelta = 360.0 / PointsCount;
            Points = Enumerable.Range(0, PointsCount)
                .Select(i => new PointObject(i, i * angleDelta))
                .ToArray();

            FrameTimer = new Timer(UpdateFrame, null, TimeSpan.Zero, TimeSpan.FromSeconds(1 / Fps));
        }

        void UpdateFrame(object state)
        {
        }
    }
}
