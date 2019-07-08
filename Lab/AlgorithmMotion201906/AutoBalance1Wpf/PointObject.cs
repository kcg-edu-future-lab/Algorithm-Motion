using System;
using AlgoMotionLib;

namespace AutoBalance1Wpf
{
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

        public TimeSpan NextMoveTime { get; set; }

        public PointObject(int id, double angle = 0)
        {
            Id = id;
            Angle = angle;
        }

        public override string ToString() => $"{Id}: {Angle:F3}";
    }
}
