using System;
using System.Collections.Generic;
using UnityEngine;

namespace Radishmouse
{
    [Serializable]
    public class LineSegment
    {
        public IReadOnlyList<Vector2> Points => points;

        [SerializeField] private List<Vector2> points = new();

        public void AddPoint(Vector2 point) =>
            points.Add(point);

        public void RemovePoint(int index) =>
            points.RemoveAt(index);
    }
}