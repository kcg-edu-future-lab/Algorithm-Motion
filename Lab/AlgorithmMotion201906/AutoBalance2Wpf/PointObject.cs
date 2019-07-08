using System;
using System.Windows;
using AlgoMotionLib;

namespace AutoBalance2Wpf
{
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

        public TimeSpan NextMoveTime { get; set; }

        public PointObject(int id, Vector position)
        {
            Id = id;
            Position = position;
        }

        public override string ToString() => $"{Id}: {Position:F3}";
    }
}
