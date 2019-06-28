using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SyncSignalsWpf
{
    public class PointObject : INotifyPropertyChanged
    {
        public int Id { get; }
        public double Angle { get; }

        DateTime _PreviousSignalTime;

        public DateTime PreviousSignalTime
        {
            get { return _PreviousSignalTime; }
            set
            {
                if (_PreviousSignalTime == value) return;
                _PreviousSignalTime = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime NextSignalTime { get; set; }
        public DateTime NextThinkingTime { get; set; }

        public PointObject(int id, double angle)
        {
            Id = id;
            Angle = angle;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString() => $"{Id}: {PreviousSignalTime:HH:mm:ss.fff}";
    }
}
