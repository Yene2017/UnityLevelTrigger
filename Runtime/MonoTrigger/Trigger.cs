using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LevelTrigger
{
    public enum TriggerType
    {
        Circle = 1,
        Polygon = 2,
        Sphere = 4,
    }

    [ExecuteInEditMode]
    public partial class Trigger : MonoBehaviour
    {
        public TriggerType type = TriggerType.Circle;
        [Range(0, 100)]
        public float height = 0;

        [HideInInspector]
        public List<Vector2> points = new List<Vector2>();
        [HideInInspector]
        public float radius = 10;

        public event Action ActionEnter;
        public event Action ActionExit;

        [HideInInspector]
        public bool triggered;
        Circle circle;
        Polygon polygon;
        Sphere sphere;

        private void OnEnable()
        {
            if (!mesh) mesh = new Mesh();
            var list = new List<Vector2>();
            for (var i = 0; i < circleMeshSplit; i++)
            {
                var radian = 360 / circleMeshSplit * i * Mathf.Deg2Rad;
                list.Add(new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));
            }
            circleDirs = list.ToArray();
            circleRandomRotate = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

            polygon = new Polygon(transform.position, points.ToArray());
            circle = new Circle(transform.position, radius);
            sphere = new Sphere(transform.position, radius);
        }

        private void Update()
        {
#if UNITY_EDITOR
            var units = GetComponents<BaseLevelUnit>();
            if (units.Length > 0)
            {
                var color = new Color(0, 0, 0, 0);
                foreach (var unit in units)
                {
                    color += EditorSetting.TriggerColor(unit.category);
                }
                color /= units.Length;
                GUIColor = color;
            }
            else
            {
                GUIColor = new Color(1, 1, 1, 0.4f);
            }
            if (Application.isPlaying) return;
            if (!SceneView.currentDrawingSceneView) return;
            Cast(SceneView.currentDrawingSceneView.camera.transform);
#endif
        }

        public void Cast(Transform target)
        {
            if (Cast(target.position))
            {
                if (!triggered)
                {
                    triggered = true;
                    OnEnter();
                }
            }
            else if (triggered)
            {
                triggered = false;
                OnExit();
            }
        }

        protected virtual void OnEnter()
        {
            ActionEnter?.Invoke();
        }

        protected virtual void OnExit()
        {
            ActionExit?.Invoke();
        }

        public bool Cast(Vector3 pos)
        {
            var trigger = false;
            switch (type)
            {
                case TriggerType.Circle:
                    if (IsInYRange(pos))
                    {
                        trigger = circle.OverlapPoint(pos);
                    }
                    else
                    {
                        trigger = false;
                    }
                    break;
                case TriggerType.Sphere:
                    trigger = sphere.OverlapPoint(pos);
                    break;
                case TriggerType.Polygon:
                    if (IsInYRange(pos))
                    {
                        trigger = polygon.OverlapPoint(pos);
                    }
                    else
                    {
                        trigger = false;
                    }
                    break;
            }

            return trigger;
        }

        bool IsInYRange(Vector3 pos)
        {
            if (height == 0) return true;
            var min = transform.position.y;
            var max = transform.position.y + height;
            return !(pos.y < min || pos.y > max);
        }
    }
}
