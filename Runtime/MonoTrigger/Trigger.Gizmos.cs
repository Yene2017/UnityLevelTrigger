#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelTrigger
{
    public partial class Trigger
    {
        //Polygon Mesh
        Mesh mesh;
        readonly List<Vector3> vertexs = new List<Vector3>();
        readonly List<Vector3> normals = new List<Vector3>();
        readonly List<int> indices = new List<int>();

        [NonSerialized]
        int circleMeshSplit = 16;

        [NonSerialized]
        Vector2[] circleDirs = new Vector2[0];

        [HideInInspector]
        public Color GUIColor = new Color(1, 1, 1, 0.4f);
        public bool useHeight
        {
            get
            {
                return height > 0;
            }
        }

        public Quaternion circleRandomRotate;

        protected virtual void OnDrawGizmos()
        {
            switch (type)
            {
                case TriggerType.Polygon:
                    DrawPoly();
                    break;
                case TriggerType.Circle:
                    DrawCircle();
                    break;
                case TriggerType.Sphere:
                    DrawSphere();
                    break;
            }
            ResetGUIColor();
        }

        private void OnDrawGizmosSelected()
        {
            switch (type)
            {
                case TriggerType.Polygon:
                    DrawPolySelected();
                    break;
                case TriggerType.Circle:
                    DrawCircleSelected();
                    break;
                case TriggerType.Sphere:
                    DrawSphereSelected();
                    break;
            }
        }

        private void DrawPolySelected()
        {
            var center = transform.position;
            polygon.position = center;
            polygon.points = points.ToArray();
            var length = points.Count;
            var from = new Vector3(0, center.y, 0);
            var to = new Vector3(0, center.y, 0);
            Vector3 fh, th;
            for (var i = 0; i < length; i++)
            {
                if (i == 0 && length > 1)
                {
                    from.x = points[length - 1].x + center.x;
                    from.z = points[length - 1].y + center.z;
                    to.x = points[0].x + center.x;
                    to.z = points[0].y + center.z;
                }
                else
                {
                    from.x = points[i - 1].x + center.x;
                    from.z = points[i - 1].y + center.z;
                    to.x = points[i].x + center.x;
                    to.z = points[i].y + center.z;
                }
                fh = Vector3.up * Size(from);
                th = Vector3.up * Size(to);
                Gizmos.DrawLine(to, to + th);
                Gizmos.DrawLine(from, to);
                Gizmos.DrawLine(from + fh, to + th);
            }
        }

        void ResetGUIColor()
        {
            Gizmos.color = Color.white;
            Handles.color = Color.white;
        }

        private float Size(Vector3 position, float scale = 1)
        {
            return useHeight ? height : UnityEditor.HandleUtility.GetHandleSize(position) * scale;
        }

        private void DrawCircle()
        {
            RebuildCircle();
            Gizmos.DrawMesh(mesh);
            Gizmos.color = Color.white;
        }

        private void DrawCircleSelected()
        {
            var center = transform.position;
            circle.position = center;
            circle.radius = radius;
            var h = Size(center) * Vector3.up;
            var p_bc = center;
            var p_bf = p_bc + circleRandomRotate * Vector3.forward * radius;
            var p_bb = p_bc - circleRandomRotate * Vector3.forward * radius;
            var p_br = p_bc + circleRandomRotate * Vector3.right * radius;
            var p_bl = p_bc - circleRandomRotate * Vector3.right * radius;
            var p_tc = p_bc + h;
            var p_tf = p_bf + h;
            var p_tb = p_bb + h;
            var p_tr = p_br + h;
            var p_tl = p_bl + h;

            Handles.CircleHandleCap(0, p_bc, Quaternion.AngleAxis(90, Vector3.right), radius, EventType.Repaint);
            Handles.CircleHandleCap(0, p_tc, Quaternion.AngleAxis(90, Vector3.right), radius, EventType.Repaint);

            Handles.DrawLine(p_bb, p_bf);
            Handles.DrawLine(p_br, p_bl);

            Handles.DrawLine(p_tf, p_bf);
            Handles.DrawLine(p_tb, p_bb);
            Handles.DrawLine(p_tr, p_br);
            Handles.DrawLine(p_tl, p_bl);

        }

        void DrawSphere()
        {
            var center = transform.position;
            Gizmos.color = GUIColor;
            Handles.color = GUIColor;
            Gizmos.DrawSphere(center, radius);
        }

        void DrawSphereSelected()
        {
            var center = transform.position;
            sphere.position = center;
            sphere.radius = radius;
            Handles.CircleHandleCap(0, center, Quaternion.AngleAxis(90, Vector3.forward), radius, EventType.Repaint);
            Handles.CircleHandleCap(0, center, Quaternion.AngleAxis(90, Vector3.right), radius, EventType.Repaint);
            Handles.CircleHandleCap(0, center, Quaternion.AngleAxis(90, Vector3.up), radius, EventType.Repaint);
        }

        void DrawPoly()
        {
            RebuildPolygon();
            Gizmos.DrawMesh(mesh);
            Gizmos.color = Color.white;
        }

        void RebuildCircle()
        {
            if (Event.current.type == EventType.Layout) return;
            var center = transform.position;
            var length = circleDirs.Length;
            Vector3 last = Vector3.zero;
            Gizmos.color = GUIColor;
            vertexs.Clear();
            normals.Clear();
            indices.Clear();
            var h = Size(center);
            for (int i = 0; i < length; ++i)
            {
                var tmp1 = EditorUtility.Merge(center, last * radius);
                var tmp2 = EditorUtility.Merge(center, circleDirs[i] * radius);
                if (i == 0)
                {
                    tmp1 = EditorUtility.Merge(center, circleDirs[i] * radius);
                    tmp2 = EditorUtility.Merge(center, circleDirs[length - 1] * radius);
                }
                last = circleDirs[i];
                var tmp3 = tmp1;
                var tmp4 = tmp2;
                tmp3.y += h;
                tmp4.y += h;

                vertexs.Add(tmp1);
                vertexs.Add(tmp2);
                vertexs.Add(tmp3);
                vertexs.Add(tmp4);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                indices.Add(i * 4);
                indices.Add(i * 4 + 1);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4 + 1);

                indices.Add(i * 4);
                indices.Add(i * 4 + 2);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4 + 2);
            }
            mesh.Clear();
            mesh.SetVertices(vertexs);
            mesh.SetNormals(normals);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        }

        void RebuildPolygon()
        {
            if (Event.current.type == EventType.Layout) return;
            var pos = transform.position;
            var length = points.Count;
            Vector3 last = Vector3.zero;
            Gizmos.color = GUIColor;
            vertexs.Clear();
            normals.Clear();
            indices.Clear();
            for (int i = 0; i < length; ++i)
            {
                var tmp1 = EditorUtility.Merge(pos, last);
                var tmp2 = EditorUtility.Merge(pos, points[i]);
                if (i == 0)
                {
                    tmp1 = EditorUtility.Merge(pos, points[i]);
                    tmp2 = EditorUtility.Merge(pos, points[points.Count - 1]);
                }
                last = points[i];
                tmp1.y = pos.y;
                tmp2.y = pos.y;
                var tmp3 = tmp1;
                var tmp4 = tmp2;
                tmp3.y = pos.y + Size(tmp1);
                tmp4.y = pos.y + Size(tmp2);

                vertexs.Add(tmp1);
                vertexs.Add(tmp2);
                vertexs.Add(tmp3);
                vertexs.Add(tmp4);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                indices.Add(i * 4);
                indices.Add(i * 4 + 1);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4 + 1);

                indices.Add(i * 4);
                indices.Add(i * 4 + 2);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4 + 2);
            }
            mesh.Clear();
            mesh.SetVertices(vertexs);
            mesh.SetNormals(normals);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        }
    }
}

#endif
