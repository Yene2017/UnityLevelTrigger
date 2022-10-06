using System;
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

        List<Listener> listeners = new List<Listener>();

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

        public void RegisterListener(int layerId, Transform transform)
        {
            if (!transform)
            {
                return;
            }
            var listener = new Listener();
            listener.transform = transform;
            listener.layerId = layerId;
            listener.last = null;
            listeners.Add(listener);
        }

        public void UnregisterAllListener()
        {
            foreach (var listener in listeners)
            {
                UnregisterListener(listener);
            }
            listeners.Clear();
        }

        public void UnregisterListener(int layerId, Transform transform)
        {
            var list = new List<Listener>();
            for (int i = 0; i < listeners.Count; i++)
            {
                if (listeners[i].layerId == layerId && listeners[i].transform == transform)
                {
                    list.Add(listeners[i]);
                }
            }
            int lastIndex = listeners.Count - 1;
            for (int i = lastIndex; i >= 0; --i)
            {
                if (listeners[i].layerId == layerId && listeners[i].transform == transform)
                {
                    listeners[i] = listeners[lastIndex];
                    lastIndex -= 1;
                }
            }
            listeners.RemoveRange(lastIndex + 1, listeners.Count - lastIndex - 1);

            foreach (var listener in list)
            {
                UnregisterListener(listener);
            }
        }

        private void UnregisterListener(Listener listener)
        {
            if (actionUnregister != null)
            {
                if (!layers.TryGetValue(listener.layerId, out var layer))
                {
                    return;
                }
                if (layer == null)
                {
                    return;
                }
                var t = listener.transform;
                foreach (var container in listener.casted)
                {
                    actionExit?.Invoke(t, container);
                }
                listener.Clear();
                actionUnregister(listener.transform, listener.layerId);
            }
        }

        class Listener
        {
            public Transform transform;
            public int layerId;
            public Container last;
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
                last = null;
                casted.Clear();
            }
        }

        public delegate void EnterDelegate(Transform listener, Container enter);
        public delegate void ExitDelegate(Transform listener, Container exit);
        public delegate void ChangeDelegate(Transform listener, Container enter, Container exit);
        public delegate void RemoveDelegate(Transform listener, int layerID);
        public delegate void UnregisterDelegate(Transform listener, int layerID);
    }
}
