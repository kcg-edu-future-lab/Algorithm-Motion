using System;
using AlgoMotionLib;

namespace SyncSignalsWpf
{
    public class PointObject : NotifyBase
    {
        public int Id { get; }
        public double Angle { get; }

        TimeSpan _SignalTime;

        public TimeSpan SignalTime
        {
            get { return _SignalTime; }
            set
            {
                if (_SignalTime == value) return;
                _SignalTime = value;
                NotifyPropertyChanged();
            }
        }

        public TimeSpan NextSignalTime { get; set; }
        public TimeSpan NextThinkingTime { get; set; }

        public PointObject(int id, double angle)
        {
            Id = id;
            Angle = angle;
        }

        public override string ToString() => $@"{Id}: {SignalTime:mm\:ss\.fff}";
    }
}
