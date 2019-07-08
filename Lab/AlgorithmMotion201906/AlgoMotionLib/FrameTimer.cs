using System;
using System.Diagnostics;
using System.Threading;

namespace AlgoMotionLib
{
    public class FrameTimer : NotifyBase
    {
        Timer Timer;
        Stopwatch FrameWatch = new Stopwatch();

        public DateTime StartTime { get; } = DateTime.Now;

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
        double _ActualFrameTime;

        public FrameTimer(Action<TimeSpan> action, TimeSpan period)
        {
            Timer = new Timer(o => MeasureTime(action), null, TimeSpan.Zero, period);
        }

        void MeasureTime(Action<TimeSpan> action)
        {
            lock (Timer)
            {
                FrameWatch.Restart();
                action(DateTime.Now - StartTime);
                FrameWatch.Stop();
                ActualFrameTime = FrameWatch.Elapsed.TotalMilliseconds;
            }
        }
    }
}
