#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LevelTrigger
{
    public partial class TriggerManager
    {
        static bool debugTrigger = false;
        const string MENUTITLE = "Tools/DebugTriggers";

        [MenuItem(MENUTITLE, false)]
        static void DebugTrigger()
        {
            debugTrigger = !debugTrigger;
        }

        [MenuItem(MENUTITLE, true)]
        static bool DebugTriggerValidation()
        {
            UnityEditor.Menu.SetChecked(MENUTITLE, debugTrigger);
            return true;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!debugTrigger)
            {
                return;
            }
            Handles.BeginGUI();
            foreach (var layer in layers)
            {
                foreach (var c in layer.containers)
                {
                    if (c == null) continue;
                    if (c.shape == null) continue;
                    c.shape.DebugGUI(c.center.y, c.height);
                }
            }
            Handles.EndGUI();
        }
    }
}
#endif
