using System.Collections.Generic;
using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    public static class Primitive
    {
        public enum PrimitiveType
        {
            Circle,
            Polygon,
        };

        public static Bezier.Handle[] Circle(int numPoints, float size, Vector2 offset)
        {
            size *= 0.5f;   // radius
            var handles = new List<Bezier.Handle>();
            var controlSize = Mathf.Tan(Mathf.PI / (2 * numPoints)) * 4 / 3 * size;
            var angleOffset = Mathf.PI / 2 + ((numPoints % 2 == 0) ? Mathf.PI / numPoints : 0);
            for (var i = 0; i < numPoints; i++)
            {
                var theta = Mathf.PI * 2 * i / numPoints + angleOffset;
                var cos = Mathf.Cos(theta);
                var sin = Mathf.Sin(theta);
                var handle = new Bezier.Handle(new Bezier.Handle(new Vector2(cos, sin) * -size + offset));
                handle.controlB = new Vector2(sin, -cos) * -controlSize;
                handle.controlA = new Vector2(sin, -cos) * controlSize;
                handles.Add(handle);
            }
            return handles.ToArray();
        }

        public static Bezier.Handle[] Polygon(int numPoints, float size, Vector2 offset)
        {
            size *= 0.5f;   // radius
            var handles = new List<Bezier.Handle>();
            var angleOffset = Mathf.PI / 2 + ((numPoints % 2 == 0) ? Mathf.PI / numPoints : 0);
            for (var i = 0; i < numPoints; i++)
            {
                var theta = Mathf.PI * 2 * i / numPoints + angleOffset;
                handles.Add(new Bezier.Handle(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * -size + offset));
            }
            return handles.ToArray();
        }
    }
}
