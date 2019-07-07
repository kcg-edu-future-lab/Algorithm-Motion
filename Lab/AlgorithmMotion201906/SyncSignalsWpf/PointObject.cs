using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SyncSignalsWpf
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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

    public static class NumberHelper
    {
        internal static Random Random { get; } = new Random();

        public static double NextDouble(double minValue, double maxValue)
        {
            return minValue + (maxValue - minValue) * Random.NextDouble();
        }

        public static int Mod(this int i, int n)
        {
            var j = i % n;
            return j >= 0 ? j : j + n;
        }
    }
}
