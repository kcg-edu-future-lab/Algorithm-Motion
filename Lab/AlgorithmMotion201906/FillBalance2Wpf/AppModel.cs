﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AlgoMotionLib;

namespace FillBalance2Wpf
{
    public class AppModel : NotifyBase
    {
        const int PointsCount = 36;
        const int NeighborsCount = 18;
        const int MaxSize = 270;
        static readonly TimeSpan MoveInterval = TimeSpan.FromSeconds(1.0);
        static readonly TimeSpan TimerInterval = TimeSpan.FromSeconds(1.0 / 30);

        public PointObject[] Points { get; }
        Queue<PointObject> MoveTimes;

        public FrameTimer FrameTimer { get; }

        public AppModel()
        {
            Points = Enumerable.Range(0, PointsCount)
                .Select(i => new PointObject(i, new Vector(NumberHelper.NextDouble(-MaxSize, MaxSize), NumberHelper.NextDouble(-MaxSize, MaxSize)))
                {
                    NextMoveTime = TimeSpan.FromSeconds(NumberHelper.NextDouble(2, 2 + MoveInterval.TotalSeconds)),
                })
                .ToArray();

            MoveTimes = new Queue<PointObject>(Points.OrderBy(_ => _.NextMoveTime));

            FrameTimer = new FrameTimer(UpdateFrame, TimerInterval);
        }

        void UpdateFrame(TimeSpan now)
        {
            var countToUpdate = MoveTimes.TakeWhile(_ => _.NextMoveTime < now).Count();
            for (var i = 0; i < countToUpdate; i++)
            {
                var point = MoveTimes.Dequeue();
                point.NextMoveTime += MoveInterval;
                MoveTimes.Enqueue(point);

                UpdatePoint(point);
            }
        }

        void UpdatePoint(PointObject p)
        {
            var neighbors = Points
                .Where(q => q.Id != p.Id)
                .OrderBy(q => (q.Position - p.Position).LengthSquared)
                .Take(NeighborsCount)
                .Select(q => q.Position)
                .ToArray();

            var walls = new[]
            {
                new Vector(Math.Sign(p.Position.X) * MaxSize, p.Position.Y),
                new Vector(p.Position.X, Math.Sign(p.Position.Y) * MaxSize),
            };

            p.Position += Sum(neighbors.Concat(walls).Select(q => GetRepulsion(q - p.Position)));

            Vector GetRepulsion(Vector d) => d.LengthSquared == 0 ? d : -40000.0 / PointsCount / (d.LengthSquared + PointsCount / 20.0) * d;
            Vector Sum(IEnumerable<Vector> source) => source.Aggregate((x, y) => x + y);
        }
    }
}
