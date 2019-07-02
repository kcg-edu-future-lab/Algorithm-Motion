﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoBalance1Wpf
{
    public class PointObject : INotifyPropertyChanged
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

        public PointObject(int id, double angle = 0)
        {
            Id = id;
            _Angle = angle;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString() => $"{Id}: {Angle:F3}";
    }
}
