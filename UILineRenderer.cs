using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Radishmouse
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineRenderer : MaskableGraphic
    {
        public IReadOnlyList<LineSegment> Segments => segments;

        [SerializeField] private List<LineSegment> segments = new();

        private float _thickness = 1f;
        public bool center = true;

        public void AddSegment()
        {
            segments.Add(new LineSegment());
        }

        public void AddPoint(int idx, Vector2 point)
        {
            if (segments.Count == 0)
            {
                AddSegment();
            }

            segments[^1].AddPoint(idx, point);
            UpdateGeometry();
        }

        public void AddPoint(Vector2 point)
        {
            if (segments.Count == 0)
            {
                AddSegment();
            }

            segments[^1].AddPoint(point);
            UpdateGeometry();
        }

        public void RemovePoint(int index)
        {
            if (segments.Count == 0)
                return;

            segments[^1].RemovePoint(index);
            UpdateGeometry();
        }

        public void ClearPoints()
        {
            segments.Clear();
            UpdateGeometry();
        }

        public void SetThickness(float thickness)
        {
            _thickness = thickness;
            UpdateGeometry();
        }

        public void SetLooped(int segmentIdx, bool isLooped)
        {
            if (segmentIdx >= segments.Count)
                return;

            segments[segmentIdx].SetLooped(isLooped);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (segments == null)
                return;

            if (segments.Count == 0)
                return;

            for (var segmentIdx = 0; segmentIdx < segments.Count; segmentIdx++)
            {
                var segment = segments[segmentIdx];
                var points = segment.Points;

                if (points == null)
                    return;

                if (points.Count < 2)
                    return;

                var segmentOffset = vh.currentVertCount;

                var index = segmentOffset;

                for (int i = 0; i < points.Count - 1; i++)
                {
                    // Create a line segment between the next two points
                    CreateLineSegment(points[i], points[i + 1], vh);

                    // Add the line segment to the triangles array
                    vh.AddTriangle(index, index + 1, index + 3);
                    vh.AddTriangle(index + 3, index + 2, index);

                    // These two triangles create the beveled edges
                    // between line segments using the end point of
                    // the last line segment and the start points of this one
                    if (i != 0)
                    {
                        vh.AddTriangle(index, index - 1, index - 3);
                        vh.AddTriangle(index + 1, index - 1, index - 2);
                    }

                    index += 5;
                }

                if (segment.IsLooped && points.Count > 2)
                {
                    var startPoint = points[^1];
                    var endPoint = points[0];

                    // Create a line segment between the next two points
                    CreateLineSegment(startPoint, endPoint, vh);

                    // Add the line segment to the triangles array
                    vh.AddTriangle(index, index + 1, index + 3);
                    vh.AddTriangle(index + 3, index + 2, index);

                    // These two triangles create the beveled edges
                    // between line segments using the end point of
                    // the last line segment and the start points of this one
                    vh.AddTriangle(index, index - 1, index - 3);
                    vh.AddTriangle(index + 1, index - 1, index - 2);

                    var lastVertIdx = vh.currentVertCount - 1;
                    vh.AddTriangle(0, lastVertIdx, lastVertIdx - 2);
                    vh.AddTriangle(1, lastVertIdx, lastVertIdx - 1);
                }
            }
        }

        /// <summary>
        /// Creates a rect from two points that acts as a line segment
        /// </summary>
        /// <param name="point1">The starting point of the segment</param>
        /// <param name="point2">The endint point of the segment</param>
        /// <param name="vh">The vertex helper that the segment is added to</param>
        private void CreateLineSegment(Vector3 point1, Vector3 point2, VertexHelper vh)
        {
            Vector3 offset = center ? (rectTransform.sizeDelta / 2) : Vector2.zero;

            // Create vertex template
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            // Create the start of the segment
            Quaternion point1Rotation = Quaternion.Euler(0, 0, RotatePointTowards(point1, point2) + 90);
            vertex.position = point1Rotation * new Vector3(-_thickness / 2, 0);
            vertex.position += point1 - offset;
            vh.AddVert(vertex);
            vertex.position = point1Rotation * new Vector3(_thickness / 2, 0);
            vertex.position += point1 - offset;
            vh.AddVert(vertex);

            // Create the end of the segment
            Quaternion point2Rotation = Quaternion.Euler(0, 0, RotatePointTowards(point2, point1) - 90);
            vertex.position = point2Rotation * new Vector3(-_thickness / 2, 0);
            vertex.position += point2 - offset;
            vh.AddVert(vertex);
            vertex.position = point2Rotation * new Vector3(_thickness / 2, 0);
            vertex.position += point2 - offset;
            vh.AddVert(vertex);

            // Also add the end point
            vertex.position = point2 - offset;
            vh.AddVert(vertex);
        }

        /// <summary>
        /// Gets the angle that a vertex needs to rotate to face target vertex
        /// </summary>
        /// <param name="vertex">The vertex being rotated</param>
        /// <param name="target">The vertex to rotate towards</param>
        /// <returns>The angle required to rotate vertex towards target</returns>
        private float RotatePointTowards(Vector2 vertex, Vector2 target)
        {
            return (float)(Mathf.Atan2(target.y - vertex.y, target.x - vertex.x) * (180 / Mathf.PI));
        }
    }
}