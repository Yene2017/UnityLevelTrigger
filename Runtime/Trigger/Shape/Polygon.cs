#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LevelTrigger
{
    public class Polygon : IShape
    {
        private float x;
        private float y;
        private Vector2[] _points;
        private bool _isValid;

        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

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

        public Vector2[] points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                this._isValid = _points != null && _points.Length > 2;
                UpdateAABB();
            }
        }

        public Polygon(Vector3 position, Vector2[] points)
        {
            x = position.x;
            y = position.z;
            this._isValid = points != null && points.Length > 2;
            if (!this._isValid)
            {
                this._points = null;
                return;
            }
            this._points = points;
            UpdateAABB();
        }

        public bool OverlapPoint(Vector3 position)
        {
            if (!this._isValid)
            {
                return false;
            }
            var x = position.x - this.x;
            var y = position.z - this.y;
            if (!CheckAABB(x, y))
            {
                return false;
            }

            var length = points.Length;
            var updown = 0;
            var crossFlag = false;
            for (int i = 0, j = length - 1; i < length; j = i++)
            {
                var p1 = points[j];
                var p2 = points[i];
                var cross = CrossLine(p1, p2, x, y);
                if (cross == 2)
                {
                    return true;
                }
                if (cross == 1)
                {
                    var p1UpDown = Dcamp(p1.y - y, -1, 0, 1);
                    var p2UpDown = Dcamp(p2.y - y, -1, 0, 1);
                    if (p1UpDown * p2UpDown < 0)
                    {
                        crossFlag = !crossFlag;
                        updown = p2UpDown;
                        //Debug.Log("交线 " + p1 + p2);
                    }
                    else if (p1UpDown == 0 && p2UpDown != 0)
                    {
                        if (updown * p2UpDown <= 0)
                        {
                            crossFlag = !crossFlag;
                            updown = -p2UpDown;
                            //Debug.Log("穿过 p1 " + updown + p1 + p2);
                        }
                        //else
                        //{
                        //    Debug.Log("穿过 p1 不计入" + updown + p1 + p2);
                        //}
                    }
                    else if (p2UpDown == 0 && p1UpDown != 0)
                    {
                        if (updown * p1UpDown <= 0)
                        {
                            crossFlag = !crossFlag;
                            updown = -p1UpDown;
                            //Debug.Log("穿过 p2 " + updown + p1 + p2);
                        }
                        //else
                        //{
                        //    Debug.Log("穿过 p2 不计入" + updown + p1 + p2);
                        //}
                    }
                }
            }
            return crossFlag;
        }

        int CrossLine(Vector2 p1, Vector2 p2, float x, float y)
        {
            if (Mathf.Abs(p2.y - p1.y) < float.Epsilon)
            {
                return 0;
            }
            var multY = (y - p1.y) * (y - p2.y);
            if (multY < float.Epsilon)
            {
                //var a = (p2.y - p1.y) / (p2.x - p1.x);
                //var b = p1.y - p1.x * a;
                //var px = (y - b) / a;
                //var px = (y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                var dx = (y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x - x;
                return Dcamp(dx, 0, 2, 1);
            }
            return 0;
        }

        int Dcamp(float v, int v1, int v2, int v3)
        {
            if (v < 0)
            {
                return v1;
            }
            if (v < float.Epsilon)
            {
                return v2;
            }
            return v3;
        }

        void UpdateAABB()
        {
            _minX = float.MaxValue;
            _minY = float.MaxValue;
            _maxX = float.MinValue;
            _maxY = float.MinValue;
            foreach (var p in _points)
            {
                _minX = Mathf.Min(_minX, p.x);
                _maxX = Mathf.Max(_maxX, p.x);
                _minY = Mathf.Min(_minY, p.y);
                _maxY = Mathf.Max(_maxY, p.y);
            }
        }

        bool CheckAABB(float x, float y)
        {
            return x <= _maxX && x >= _minX && y >= _minY && y <= _maxY;
        }

        public void DebugGUI(float y, float height)
        {
#if UNITY_EDITOR
            var useHeight = height > 0 && height < float.MaxValue;
            var length = _points.Length;
            Vector3 last = Vector3.zero;
            for (int i = 0, j = length - 1; i < length; j = i++)
            {
                var p1 = new Vector3(_points[j].x, y, _points[j].y);
                var p2 = new Vector3(_points[i].x, y, _points[i].y);
                var guiheight1 = (useHeight ? height : HandleUtility.GetHandleSize(p1)) * Vector3.up;
                var guiheight2 = (useHeight ? height : HandleUtility.GetHandleSize(p2)) * Vector3.up;
                var p3 = p1 + guiheight1;
                var p4 = p2 + guiheight2;

                Handles.DrawLine(p1, p2);
                Handles.DrawLine(p2, p4);
                Handles.DrawLine(p1, p3);
                Handles.DrawLine(p3, p4);
            }
#endif
        }
    }
}
