using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LevelTrigger
{
    public partial class TriggerManager
    {
        public const float UPDATE_INTERVAL = 0.5f;

        readonly Dictionary<int, Layer> layers = new Dictionary<int, Layer>();

        public void Init()
        {
#if UNITY_EDITOR
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
#endif
        }

        public void Destroy()
        {
#if UNITY_EDITOR
            SceneView.duringSceneGui -= OnSceneGUI;
#endif
        }

        public void CommonUpdate()
        {
            for (int i = 0; i < listeners.Count; ++i)
            {
                var listener = listeners[i];
                if (!layers.TryGetValue(listener.layerId, out var layer)) continue;
                if (layer == null) continue;
                if (layer.additive)
                {
                    InclusiveCase(listeners[i]);
                }
                else
                {
                    ExclusiveCast(listeners[i]);
                }
            }
        }

        public void CreateLayer(int layerId, Container[] array, bool additive)
        {
            layers.TryGetValue(layerId, out var layer);
            if (layer != null)
            {
                Debug.LogWarning($"Layer List already exist! layerId = {layerId}");
                return;
            }
            layer = new Layer();
            layer.layerID = layerId;
            layer.additive = additive;
            layer.containers = System.Linq.Enumerable.ToList(array);
            layers.Add(layerId, layer);
        }

        public void RemoveLayer(int layerId)
        {
            if (layers.TryGetValue(layerId, out var layer))
            {
                layers.Remove(layerId);
            }
            if (layer == null)
            {
                return;
            }
            foreach (var listener in listeners)
            {
                if (listener.layerId != layerId)
                {
                    continue;
                }
                RemoveLayer(listener);
            }
        }

        public void RemoveAllLayer()
        {
            foreach (var listener in listeners)
            {
                RemoveLayer(listener);
            }
            layers.Clear();
        }

        private void RemoveLayer(Listener listener)
        {
            if (!layers.TryGetValue(listener.layerId, out var layer)) return;
            if (layer == null) return;

            foreach (var container in listener.casted)
            {
                actionExit?.Invoke(listener.transform, container);
            }
            if (listener.last != null)
            {
                actionChanged?.Invoke(listener.transform, null, listener.last);
            }
            listener.Clear();
            actionLayerRemove?.Invoke(listener.transform, listener.layerId);
        }

        #region Cast

        public void ForceCastLayer(int layerId, Transform transform)
        {
            if (!transform) return;
            if (!layers.TryGetValue(layerId, out var layer)) return;

            var position = transform.position;
            foreach (var listener in listeners)
            {
                if (layerId != listener.layerId) continue;
                if (transform != listener.transform) continue;

                foreach (var container in layer.containers)
                {
                    if (container == null) continue;

                    if (OverlapPoint(container, position))
                    {
                        actionEnter(transform, container);
                        listener.Add(container);
                    }
                    else if (listener.Contains(container))
                    {
                        actionExit(transform, container);
                        listener.Remove(container);
                    }
                }
            }
        }

        public int GetCastedContainerID(int layerId, Vector3 position)
        {
            if (!layers.TryGetValue(layerId, out var layer))return -1;
            var container = CastLayer(layer, position);
            if (container == null) return -2;
            return container.containerID;
        }

        void InclusiveCase(Listener listener)
        {
            if (listener.transform == null) return;
            if (!layers.TryGetValue(listener.layerId, out var layer)) return;
            if (layer == null) return;

            Vector3 position = listener.transform.position;
            for (int i = 0; i < layer.containers.Count; ++i)
            {
                var container = layer.containers[i];
                if (container == null) continue;
                var casted = listener.Contains(container);
                if (OverlapPoint(container, position))
                {
                    if (!casted)
                    {
                        listener.Add(container);
                        actionEnter?.Invoke(listener.transform, container);
                    }
                }
                else if (casted)
                {
                    listener.Remove(container);
                    actionExit?.Invoke(listener.transform, container);
                }
            }
        }

        void ExclusiveCast(Listener listener)
        {
            if (listener.transform == null) return;
            if (!layers.TryGetValue(listener.layerId, out var layer)) return;

            var container = CastLayer(layer, listener.transform.position);
            if (container != listener.last)
            {
                var last = listener.last;
                listener.last = container;
                if (last != null)
                {
                    listener.Remove(last);
                    actionExit?.Invoke(listener.transform, last);
                }
                if (container != null)
                {
                    listener.Add(container);
                    actionEnter?.Invoke(listener.transform, container);
                }
                actionChanged?.Invoke(listener.transform, container, last);
            }
        }

        Container CastLayer(Layer layer, Vector3 position)
        {
            if (layer == null) return null;
            for (int i = 0; i < layer.containers.Count; ++i)
            {
                var container = layer.containers[i];
                if (OverlapPoint(container, position))
                {
                    return container;
                }
            }
            return null;
        }

        public Container CastLayerById(int layerId, Vector3 position)
        {
            if (layers.TryGetValue(layerId, out var layer))
            {
                return CastLayer(layer, position);
            }
            else
            {
                return null;
            }
        }
        
        public bool OverlapPoint(Container container, Vector3 position)
        {
            if (container == null) return false;
            if (!container.Overlap(position)) return false;
            return true;
        }

#endregion
    }
}
