#if UNITY_EDITOR
#endif
using UnityEditor;
using UnityEngine;

namespace LevelTrigger
{
    public interface IShape
    {
        bool OverlapPoint(Vector3 position);
        void DebugGUI(float y, float height);
    }
}
