using System.Collections.Generic;
using UnityEngine;

namespace LevelTrigger
{
    public partial class TriggerManager
    {
        public EnterDelegate actionEnter;
        public ExitDelegate actionExit;
        public ChangeDelegate actionChanged;
        public RemoveDelegate actionLayerRemove;
        public UnregisterDelegate actionUnregister;

        List<CastHandler> handlers = new List<CastHandler>();

        public void RegisterCallback(
            EnterDelegate actionEnter,
            ExitDelegate actionExit,
            ChangeDelegate actionChanged,
            RemoveDelegate actionLayerRemove,
            UnregisterDelegate actionUnregister)
        {
            this.actionEnter = actionEnter;
            this.actionExit = actionExit;
            this.actionChanged = actionChanged;
            this.actionLayerRemove = actionLayerRemove;
            this.actionUnregister = actionUnregister;
        }

        public void RegisterCastHandler(int layerId, Transform transform)
        {
            if (!transform)
            {
                return;
            }
            var handler = new CastHandler();
            handler.transform = transform;
            handler.layer = layerId;
            handler.curt = null;
            handlers.Add(handler);
        }

        public void UnregisterAllCastHandler()
        {
            foreach (var handler in handlers)
            {
                Unregister(handler);
            }
            handlers.Clear();
        }

        public void Unregister(int layerId, Transform transform)
        {
            var list = new List<CastHandler>();
            for (int i = 0; i < handlers.Count; i++)
            {
                if (handlers[i].layer == layerId && handlers[i].transform == transform)
                {
                    list.Add(handlers[i]);
                }
            }
            int lastIndex = handlers.Count - 1;
            for (int i = lastIndex; i >= 0; --i)
            {
                if (handlers[i].layer == layerId && handlers[i].transform == transform)
                {
                    handlers[i] = handlers[lastIndex];
                    lastIndex -= 1;
                }
            }
            handlers.RemoveRange(lastIndex + 1, handlers.Count - lastIndex - 1);

            foreach (var handler in list)
            {
                Unregister(handler);
            }
        }

        private void Unregister(CastHandler handler)
        {
            if (actionUnregister != null)
            {
                if (!TryGetLayer(handler.layer, out var layer))
                {
                    return;
                }
                var t = handler.transform;
                foreach (var container in handler.casted)
                {
                    actionExit?.Invoke(t, container);
                }
                handler.Clear();
                actionUnregister(handler.transform, handler.layer);
            }
        }

        class CastHandler
        {
            public Transform transform;
            public int layer;
            public Container curt;
            public List<Container> casted = new List<Container>();

            public bool Contains(Container c)
            {
                return casted.Contains(c);
            }

            public void Add(Container c)
            {
                casted.Add(c);
            }

            public void Remove(Container c)
            {
                casted.Remove(c);
            }

            public void Clear()
            {
                curt = null;
                casted.Clear();
            }
        }

        public delegate void EnterDelegate(Transform target, Container enter);
        public delegate void ExitDelegate(Transform target, Container exit);
        public delegate void ChangeDelegate(Transform target, Container enter, Container exit);
        public delegate void RemoveDelegate(Transform target, int layerID);
        public delegate void UnregisterDelegate(Transform target, int layerID);
    }
}
