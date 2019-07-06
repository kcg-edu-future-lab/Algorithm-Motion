using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AutoBalance2Wpf
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class PointObject : NotifyBase
    {
        static readonly Vector UnitX = new Vector(1, 0);

        public int Id { get; }

        Vector _Position;

        public Vector Position
        {
            get { return _Position; }
            set
            {
                if (_Position == value) return;
                _Position = value;
                Angle = Vector.AngleBetween(UnitX, value);
                NotifyPropertyChanged();
            }
        }

        public double Angle { get; private set; }

        public DateTime NextMoveTime { get; set; }

        public PointObject(int id, Vector position)
        {
            Id = id;
            Position = position;
        }

        public override string ToString() => $"{Id}: {Position}";
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
