using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace LevelTrigger
{
    public partial class TriggerManager
    {
        public class Layer
        {
            public int layerID = 0;
            public bool additive;
            public List<Container> containers = new List<Container>();
        }

        public class Container
        {
            public int layerID;
            public int containerID;
            public Vector3 center;
            public float height = -1;
            public IShape shape;

            public static Container CreateCircle(int layerID, int containerID,
                Vector3 center, float height, float radius)
            {
                var a = new Container();
                a.layerID = layerID;
                a.containerID = containerID;
                a.center = center;
                a.height = height;
                a.shape = new Circle(center, radius);
                return a;
            }

            public static Container CreateSphere(int layerID, int containerID,
                Vector3 center, float radius)
            {
                var a = new Container();
                a.layerID = layerID;
                a.containerID = containerID;
                a.center = center;
                a.height = -1;
                a.shape = new Sphere(center, radius);
                return a;
            }

            public static Container CreatePolygon(int layerID, int containerID,
                Vector3 center, float height, float[][] points)
            {
                var a = new Container();
                a.layerID = layerID;
                a.containerID = containerID;
                a.center = center;
                a.height = height;
                var v2points = Array.ConvertAll(points, v => new Vector2(v[0], v[1]));
                a.shape = new Polygon(Vector3.zero, v2points);
                return a;
            }

            public bool Overlap(Vector3 point)
            {
                if (shape != null)
                {
                    if (!shape.OverlapPoint(point))
                    {
                        return false;
                    }
                    if (this.height <= 0)
                    {
                        return true;
                    }
                    else
                    {
                        var y = point.y;
                        var yMin = this.center.y;
                        return !(y < yMin || y > yMin + this.height);
                    }
                }
                return false;
            }

        }
    }
}
