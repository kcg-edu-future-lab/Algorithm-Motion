using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoBalance1Wpf
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

        double _Angle;

        public double Angle
        {
            get { return _Angle; }
            set
            {
                if (_Angle == value) return;
                _Angle = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime NextMoveTime { get; set; }

        public PointObject(int id, double angle = 0)
        {
            Id = id;
            Angle = angle;
        }

        public override string ToString() => $"{Id}: {Angle:F3}";
    }

    public static class NumberHelper
    {
        public static int Mod(this int i, int n)
        {
            var j = i % n;
            return j >= 0 ? j : j + n;
        }
    }
}
