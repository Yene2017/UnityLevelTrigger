#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LevelTrigger 
{
    public class EditorSetting : ScriptableObject
    {
        public bool displayAll = true;

        [Header("Aduio")]
        public bool displayMusic = true;
        public bool display2dSound = true;
        public bool display3dSound = true;
        public bool previewSound = true;

        public Color[] colorForTriggers = new Color[0];

        public static Color TriggerColor(BaseLevelUnit.UnitCategory type)
        {
            var index = (int)type;
            if (setting && setting.colorForTriggers != null && index < setting.colorForTriggers.Length)
            {
                return setting.colorForTriggers[index];
            }
            return Color.gray * 0.2f;
        }

        public static bool NeedDisplay(BaseLevelUnit unit)
        {
            var result = true;
            return result && Setting.displayAll && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }
        static EditorSetting setting;

        public static EditorSetting Setting
        {
            get
            {
                if (!setting)
                {
                    setting = Resources.FindObjectsOfTypeAll<EditorSetting>().FirstOrDefault();
                }
                if (!setting)
                {
                    var guid = AssetDatabase.FindAssets("t:LevelTrigger.EditorSetting", null).FirstOrDefault();
                    if (!string.IsNullOrEmpty(guid))
                    {
                        setting = AssetDatabase.LoadAssetAtPath<EditorSetting>(AssetDatabase.GUIDToAssetPath(guid));
                    }
                }
                if (!setting)
                {
                    setting = CreateInstance<EditorSetting>();
                    var SettingPath = UnityEditor.EditorUtility.SaveFilePanel(nameof(EditorSetting), "", nameof(EditorSetting), "asset");
                    if (!string.IsNullOrEmpty(SettingPath))
                    {
                        var path = SettingPath.Replace(Application.dataPath, "Assets");
                        AssetDatabase.CreateAsset(setting, path);
                    }
                }
                return setting;
            }
        }

    }
}
#endif
