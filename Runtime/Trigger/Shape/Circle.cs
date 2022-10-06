#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LevelTrigger
{
    public class Circle : IShape
    {
        private float x;
        private float y;
        private float r2;

        public Vector3 position
        {
            get
            {
                return new Vector3(x, 0, y);
            }
            set
            {
                x = value.x;
                y = value.z;
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

        public Circle(Vector3 position, float radius)
        {
            x = position.x;
            y = position.z;
            r2 = radius * radius;
        }

        public Circle(float x, float y, float radius)
        {
            this.x = x;
            this.y = y;
            r2 = radius * radius;
        }

        public bool OverlapPoint(Vector3 position)
        {
            var dx = position.x - this.x;
            var dy = position.z - this.y;
            return dx * dx + dy * dy < r2;
        }

        public void DebugGUI(float y, float height)
        {
#if UNITY_EDITOR
            var useHeight = height > 0 && height < float.MaxValue;
            var random = Quaternion.Euler(0, 0, 0);
            var guiheight = (useHeight ? height : HandleUtility.GetHandleSize(position)) * Vector3.up;
            var p_bc = position;
            p_bc.y = y;
            var p_bf = p_bc + random * Vector3.forward * radius;
            var p_bb = p_bc - random * Vector3.forward * radius;
            var p_br = p_bc + random * Vector3.right * radius;
            var p_bl = p_bc - random * Vector3.right * radius;
            var p_tc = p_bc + guiheight;
            var p_tf = p_bf + guiheight;
            var p_tb = p_bb + guiheight;
            var p_tr = p_br + guiheight;
            var p_tl = p_bl + guiheight;

            Handles.CircleHandleCap(0, p_bc, Quaternion.AngleAxis(90, Vector3.right),
                radius, EventType.Repaint);
            Handles.CircleHandleCap(0, p_tc, Quaternion.AngleAxis(90, Vector3.right),
                radius, EventType.Repaint);

            Handles.DrawLine(p_bb, p_bf);

            Handles.DrawLine(p_tf, p_bf);
            Handles.DrawLine(p_tb, p_bb);
            Handles.DrawLine(p_tr, p_br);
            Handles.DrawLine(p_tl, p_bl);
#endif
        }
    }


}
