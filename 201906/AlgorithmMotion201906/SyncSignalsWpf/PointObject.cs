using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SyncSignalsWpf
{
    public class PointObject : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString() => $@"{Id}: {SignalTime:mm\:ss\.fff}";
    }
}
