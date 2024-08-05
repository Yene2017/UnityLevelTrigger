using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelTrigger
{
    public partial class TriggerManager
    {
        public void InitLayerCapacity(int capacity)
        {
            layers = new Layer[capacity];
        }


        public void CreateLayer(int id, List<Container> containers, bool excludeCast)
        {
            if (TryGetLayer(id, out _))
            {
                Debug.LogWarning($"Layer List already exist! layerId = {id}");
                return;
            }
            var layer = new Layer();
            layer.id = id;
            layer.excludeCast = excludeCast;
            layer.containers = containers;
            layers[id] = layer;
        }

        public void RemoveLayer(int id)
        {
            if (!TryGetLayer(id, out _))
            {
                return;
            }
            foreach (var handler in handlers)
            {
                if (handler.layer != id)
                {
                    continue;
                }
                RemoveLayer(handler);
            }
            layers[id] = null;
        }

        public void RemoveAllLayers()
        {
            foreach (var handler in handlers)
            {
                RemoveLayer(handler);
            }
            for(var i = 0; i < layers.Length; i++)
            {
                layers[i] = null;
            }
        }

        public bool TryGetLayer(int id, out Layer layer)
        {
            layer = layers[id];
            return layer != null;
        }

        private void RemoveLayer(CastHandler handler)
        {
            if (!TryGetLayer(handler.layer, out var layer)) return;

            foreach (var container in handler.casted)
            {
                actionExit?.Invoke(handler.transform, container);
            }
            if (handler.curt != null)
            {
                actionChanged?.Invoke(handler.transform, null, handler.curt);
            }
            handler.Clear();
            actionLayerRemove?.Invoke(handler.transform, handler.layer);
        }


        Layer[] layers = new Layer[8];


        public class Layer
        {
            public int id = 0;
            public bool excludeCast;
            public List<Container> containers = new List<Container>();
        }

        public class Container
        {
            public int layer;
            public int id;
            public Vector3 center;
            public float height = -1;
            public IShape shape;

            public static Container CreateCircle(int layer, int id,
                Vector3 center, float height, float radius)
            {
                var a = new Container();
                a.layer = layer;
                a.id = id;
                a.center = center;
                a.height = height;
                a.shape = new Circle(center, radius);
                return a;
            }

            public static Container CreateSphere(int layer, int id,
                Vector3 center, float radius)
            {
                var a = new Container();
                a.layer = layer;
                a.id = id;
                a.center = center;
                a.height = -1;
                a.shape = new Sphere(center, radius);
                return a;
            }

            public static Container CreatePolygon(int layer, int id,
                Vector3 center, float height, float[][] points)
            {
                var a = new Container();
                a.layer = layer;
                a.id = id;
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
