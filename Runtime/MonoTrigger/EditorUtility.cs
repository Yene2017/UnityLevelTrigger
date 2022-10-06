#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelTrigger
{
    public class EditorUtility
    {
        public const int LAYER_GROUND = 10;

        public static Vector3 Merge(Vector3 center, Vector2 point)
        {
            return new Vector3(point.x, 0, point.y) + center;
        }

        public static Scene CopyScene(string path)
        {
            if (Application.isPlaying)
            {
                return EditorSceneManager.GetActiveScene();
            }
            var tmpPath = "Assets/tmp.unity";
            AssetDatabase.CopyAsset(path, tmpPath);
            var tmpScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(tmpPath);
            tmpScene.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideAndDontSave;
            return EditorSceneManager.OpenScene(tmpPath, OpenSceneMode.Single);
        }

        public static Vector3 GetPointPos(float y = 0)
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

        public static Vector3 CalcFoot(Vector3 point, Vector3 line1, Vector3 line2)
        {
            var x0 = point.x;
            var y0 = point.z;
            var x1 = line1.x;
            var y1 = line1.z;
            var x2 = line2.x;
            var y2 = line2.z;
            var k = ((x0 - x1) * (x2 - x1) + (y0 - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            return new Vector3((x2 - x1) * k + x1, 0, (y2 - y1) * k + y1);
        }

        public static Vector2 CalcFoot(Vector3 point, Vector2 line1, Vector2 line2)
        {
            var x0 = point.x;
            var y0 = point.z;
            var x1 = line1.x;
            var y1 = line1.y;
            var x2 = line2.x;
            var y2 = line2.y;
            var k = ((x0 - x1) * (x2 - x1) + (y0 - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            return new Vector2((x2 - x1) * k + x1, (y2 - y1) * k + y1);
        }

        public static Vector3 StickToEarth(Transform origin)
        {
            var defaultDistance = HandleUtility.GetHandleSize(origin.position);
            var ray = new Ray(origin.position, origin.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LAYER_GROUND))
            {
                return hitInfo.point;
            }
            var position = ray.GetPoint(defaultDistance);
            UnityEngine.AI.NavMeshHit navMeshHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(position, out navMeshHit, Mathf.Infinity, -1))
            {
                return navMeshHit.position;
            }
            return position;
        }

    }
}

#endif
