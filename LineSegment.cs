using System;
using System.Collections.Generic;
using UnityEngine;

namespace Radishmouse
{
    [Serializable]
    public class LineSegment
    {
        public IReadOnlyList<Vector2> Points => points;
        public bool IsLooped => isLooped;

        [SerializeField] private List<Vector2> points = new();
        [SerializeField] private bool isLooped;

        public void AddPoint(Vector2 point) =>
            points.Add(point);

        public void RemovePoint(int index) =>
            points.RemoveAt(index);

        public void SetLooped(bool isLooped) =>
            this.isLooped = isLooped;
    }
}