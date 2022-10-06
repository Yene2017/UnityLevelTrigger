#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace LevelTrigger
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Trigger))]
    public class TriggerEditor : Editor
    {
        protected const int CONTROLSIZE = 16;
        protected List<Vector2> defaultPoints = new List<Vector2>()
        {
            new Vector2(10, 10),
            new Vector2(-10, 10),
            new Vector2(-10, -10),
            new Vector2(10, -10)
        };
        Vector3[] square = new Vector3[]
        {
            new Vector3(1,0,0),
            new Vector3(-1,0,0),
            new Vector3(0,0,1),
            new Vector3(0,0,-1),
        };
        protected int control = -1;
        protected bool isMultiEdit;

        Trigger trigger;
        Trigger[] triggers;
        protected Transform transform;

        private void OnEnable()
        {
            trigger = target as Trigger;
            transform = trigger.transform;
            isMultiEdit = targets != null && targets.Length > 1;
            if (isMultiEdit)
            {
                triggers = new Trigger[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    triggers[i] = targets[i] as Trigger;
                }
            }
        }

        public void OnSceneGUI()
        {
            if (isMultiEdit)
            {
                foreach (var trigger in triggers)
                {
                    switch (trigger.type)
                    {
                        case TriggerType.Circle:
                            SceneGUIInCircle(trigger);
                            break;
                        case TriggerType.Polygon:
                            SceneGUIInPolygon(trigger);
                            break;
                        case TriggerType.Sphere:
                            SceneGUIInSphere(trigger);
                            break;
                    }
                }
            }
            else
            {
                switch (trigger.type)
                {
                    case TriggerType.Circle:
                        SceneGUIInCircle(trigger);
                        break;
                    case TriggerType.Polygon:
                        ShowTips();
                        SceneGUIInPolygon(trigger);
                        break;
                    case TriggerType.Sphere:
                        SceneGUIInSphere(trigger);
                        break;
                }
            }
        }

        void ShowTips()
        {
            Handles.BeginGUI();
            GUI.Box(new Rect(10, 10, 160, 80), "按住Ctrl后点击可以增加点", "flow node 1");
            if (GUI.Button(new Rect(20, 40, 60, 30), "重置"))
            {
                Undo.RecordObject(trigger, trigger.name);
                trigger.points.Clear();
                trigger.points.AddRange(defaultPoints);
                HandleUtility.Repaint();
            }
            Handles.EndGUI();
        }

        protected virtual void SceneGUIInCircle(Trigger trigger)
        {
            var center = trigger.transform.position;
            Handles.BeginGUI();
            GUI.Box(new Rect(10, 10, 160, 80), "拖动圆点定义半径", "flow node 1");
            Handles.EndGUI();

            foreach (var s in square)
            {
                var point = center + trigger.radius * (trigger.circleRandomRotate * s);
                var size = HandleUtility.GetHandleSize(point) / 10;
                EditorGUI.BeginChangeCheck();
                var pos = Handles.FreeMoveHandle(point,
                    Quaternion.identity, size, Vector3.zero, Handles.SphereHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(trigger, trigger.name);
                    trigger.radius = Mathf.Abs(pos.x - transform.position.x);
                }
            }
        }

        protected virtual void SceneGUIInSphere(Trigger trigger)
        {
            var transform = trigger.transform;
            EditorGUI.BeginChangeCheck();
            var radius = Handles.RadiusHandle(Quaternion.identity, transform.position, trigger.radius);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(trigger, "Change targetComponent Bounds");
                trigger.radius = radius;
            }
        }

        protected virtual void SceneGUIInPolygon(Trigger trigger)
        {
            var p = trigger.transform.position;
            if (trigger.points.Count < 1)
            {
                trigger.points.AddRange(defaultPoints);
            }

            var size = HandleUtility.GetHandleSize(p) / 10;
            var e = Event.current;
            //Add Point
            if (!isMultiEdit && e.control && e.button == 0 && e.type == EventType.MouseDown)
            {
                Undo.RecordObject(trigger, trigger.name);
                AddPoint();
                GUIUtility.hotControl = 0;
                e.Use();
            }

            // FreeMove
            Vector3 v = Vector3.zero;
            var length = trigger.points.Count;
            var center = transform.position;
            for (int i = 0; i < length; ++i)
            {
                v = EditorUtility.Merge(center, trigger.points[i]);

                Handles.Label(v, i + "");
                var nv = Handles.FreeMoveHandle(v, Quaternion.identity,
                    size, Vector3.zero, Handles.CubeHandleCap);
                if (nv != v)
                {
                    Undo.RecordObject(trigger, trigger.name);
                    v = GetPointPos(center.y) - center;
                    trigger.points[i] = new Vector2(v.x, v.z);
                    control = i;
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 &&
                    Vector2.Distance(HandleUtility.WorldToGUIPoint(nv), Event.current.mousePosition) < CONTROLSIZE)
                {
                    control = i;
                }
            }
            if (control > -1 && e.button == 1 && e.type == EventType.MouseDown)
            {
                var menu = new GenericMenu();
                var content = new GUIContent("DeletePoint");
                var index = control;
                menu.AddItem(content, false, delegate
                {
                    Undo.RecordObject(trigger, trigger.name);
                    trigger.points.RemoveAt(index);
                });
                menu.ShowAsContext();
                Event.current.Use();
            }
            control = -1;
        }

        protected static Vector3 GetPointPos(float y = 0)
        {
            Camera sceneCamera = SceneView.currentDrawingSceneView.camera;
            var mouse = Event.current.mousePosition;
            var pointPos = CalculateWorldPosition(Vector3.right * mouse.x + Vector3.up * (sceneCamera.pixelHeight - mouse.y),
                sceneCamera, y);
            return pointPos;
        }

        protected static Vector3 CalculateWorldPosition(Vector3 mouse, Camera camera, float y = 0)
        {
            var tmp1 = camera.ScreenToWorldPoint(mouse + Vector3.forward * 1);
            var tmp2 = camera.ScreenToWorldPoint(mouse + Vector3.forward * 2);
            var delta = tmp1 - tmp2;
            var k = (y - tmp2.y) / delta.y;
            var result = k * delta + tmp2;
            return result;
        }

        void AddPoint()
        {
            var center = transform.position;
            var point3d = EditorUtility.GetPointPos(center.y);
            var points = trigger.points;
            var length = points.Count;
            var distance = HandleUtility.DistancePointLine(point3d,
                EditorUtility.Merge(center, points[0]),
                EditorUtility.Merge(center, points[length - 1]));
            var closestIndex = length - 1;
            for (int i = 0; i < length - 1; i++)
            {
                var tmp = HandleUtility.DistancePointLine(point3d,
                    EditorUtility.Merge(center, points[i]),
                    EditorUtility.Merge(center, points[i + 1]));
                if (tmp < distance)
                {
                    distance = tmp;
                    closestIndex = i;
                }
            }
            var closestIndex2 = closestIndex == length - 1 ? 0 : closestIndex + 1;
            points.Insert(closestIndex2,
                new Vector2(point3d.x - center.x, point3d.z - center.z));
        }
    }
}
#endif
