using UnityEngine;

namespace LevelTrigger
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    [RequireComponent(typeof(Trigger))]
#endif
    public abstract class BaseLevelUnit : MonoBehaviour
    {
        public enum UnitCategory
        {
            Simple = 0,
            Location = 1,
            Music = 2,
            Ambient = 3,
        }

        public virtual UnitCategory category
        {
            get
            {
                return UnitCategory.Simple;
            }
        }

        public float height
        {
            set
            {
                trigger.height = value;
            }
        }

        public Trigger  trigger
        {
            get
            {
                var t = gameObject.GetComponent<Trigger>();
                if (!t) t = gameObject.AddComponent<Trigger>();
                return t;
            }
        }

#if UNITY_EDITOR

        protected virtual void OnDrawGizmos()
        {
            if (!EditorSetting.NeedDisplay(this))
            {
                return;
            }
            var t = transform;
            Gizmos.DrawIcon(t.position, Icon, true);
        }

        public virtual string Icon
        {
            get
            {
                return "Terrain Icon";
            }
        }
#endif

    }

}
