#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LevelTrigger
{
    public partial class TriggerManager
    {
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

        public void Tick()
        {
            for (int i = 0; i < handlers.Count; ++i)
            {
                var handler = handlers[i];
                if (!TryGetLayer(handler.layer, out var layer)) continue;
                if (layer.excludeCast)
                {
                    ExclusiveCast(handlers[i]);
                }
                else
                {
                    InclusiveCast(handlers[i]);
                }
            }
        }

        #region Cast

        public void ForceCastLayer(int id, Transform transform)
        {
            if (!transform) return;
            if (!TryGetLayer(id, out var layer)) return;

            var position = transform.position;
            foreach (var handler in handlers)
            {
                if (id != handler.layer) continue;
                if (transform != handler.transform) continue;

                foreach (var container in layer.containers)
                {
                    if (container == null) continue;

                    if (OverlapPoint(container, position))
                    {
                        actionEnter(transform, container);
                        handler.Add(container);
                    }
                    else if (handler.Contains(container))
                    {
                        actionExit(transform, container);
                        handler.Remove(container);
                    }
                }
            }
        }

        public int CastLayerForID(int id, Vector3 position)
        {
            if (!TryGetLayer(id, out var layer)) return -1;
            var container = CastLayer(layer, position);
            if (container == null) return -2;
            return container.id;
        }

        public Container CastLayer(int id, Vector3 position)
        {
            if (TryGetLayer(id, out var layer))
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

        void InclusiveCast(CastHandler handler)
        {
            if (handler.transform == null) return;
            if (!TryGetLayer(handler.layer, out var layer)) return;

            Vector3 position = handler.transform.position;
            for (int i = 0; i < layer.containers.Count; ++i)
            {
                var container = layer.containers[i];
                if (container == null) continue;
                var casted = handler.Contains(container);
                if (OverlapPoint(container, position))
                {
                    if (!casted)
                    {
                        handler.Add(container);
                        actionEnter?.Invoke(handler.transform, container);
                    }
                }
                else if (casted)
                {
                    handler.Remove(container);
                    actionExit?.Invoke(handler.transform, container);
                }
            }
        }

        void ExclusiveCast(CastHandler handler)
        {
            if (handler.transform == null) return;
            if (!TryGetLayer(handler.layer, out var layer)) return;

            var container = CastLayer(layer, handler.transform.position);
            if (container != handler.curt)
            {
                var last = handler.curt;
                handler.curt = container;
                if (last != null)
                {
                    handler.Remove(last);
                    actionExit?.Invoke(handler.transform, last);
                }
                if (container != null)
                {
                    handler.Add(container);
                    actionEnter?.Invoke(handler.transform, container);
                }
                actionChanged?.Invoke(handler.transform, container, last);
            }
        }

        Container CastLayer(Layer layer, Vector3 position)
        {
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

#endregion
    }
}
