using System;
using System.Windows;
using AlgoMotionLib;

namespace FillBalance2Wpf
{
    public class PointObject : NotifyBase
    {
        public int Id { get; }

        public Vector Position
        {
            get { return _Position; }
            set
            {
                if (_Position == value) return;
                _Position = value;
                NotifyPropertyChanged();
            }
        }
        Vector _Position;

        public TimeSpan NextMoveTime { get; set; }

        public PointObject(int id, Vector position)
        {
            Id = id;
            Position = position;
        }

        public override string ToString() => $"{Id}: {Position:F3}";
    }
}
