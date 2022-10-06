#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LevelTrigger
{
    public class Sphere : IShape
    {
        private Vector3 _position;
        private float r2;

        public Vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public float radius
        {
            get
            {
                return Mathf.Sqrt(r2);
            }
            set
            {
                r2 = value * value;
            }
        }

        public Sphere(Vector3 position, float radius)
        {
            _position = position;
            r2 = radius * radius;
        }

        public bool OverlapPoint(Vector3 position)
        {
            return (_position - position).sqrMagnitude < r2;
        }

        public void DebugGUI(float y, float height)
        {
#if UNITY_EDITOR
            var c = Handles.color;
            c.a = 0.1f;
            Handles.color = c;
            Handles.SphereHandleCap(0, _position, Quaternion.identity,
                Mathf.Sqrt(r2), EventType.Repaint);
            Handles.color = Color.white;
#endif
        }
    }


}
