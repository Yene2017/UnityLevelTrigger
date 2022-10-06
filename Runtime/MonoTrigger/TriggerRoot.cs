using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelTrigger
{
    public class TriggerRoot : MonoBehaviour
    {
#if UNITY_EDITOR
        public const string DEFUALTNAME = nameof(TriggerRoot);
        public static TriggerRoot root
        {
            get
            {
                return GameObject.FindObjectOfType<TriggerRoot>();
            }
        }

        public static TriggerRoot GetOrCreateRoot()
        {
            var root = GameObject.FindObjectOfType<TriggerRoot>();
            if (!root)
            {
                root = new GameObject(DEFUALTNAME).AddComponent<TriggerRoot>();
                root.name = SceneManager.GetActiveScene().name;
            }
            return root;
        }
#endif
    }
}

