using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelTrigger
{
    [Serializable]
    public class EventParam
    {
        public string name = "";
        public float value = 0;
    }

    public class FmodParamUnit : BaseLevelUnit
    {
        public List<EventParam> _params = new List<EventParam>();

        [NonSerialized]
        public bool isPlaying = false;
        [HideInInspector]
        public bool is3D = false;

        public override UnitCategory category
        {
            get
            {
                return UnitCategory.Ambient;
            }
        }

#if UNITY_EDITOR

        public override string Icon
        {
            get
            {
                return "AudioSource Icon";
            }
        }
#endif

        private void OnEnable()
        {
            if (trigger)
            {
                trigger.ActionEnter += PlayAudios;
                trigger.ActionExit += StopAudios;
            }
        }

        private void OnDisable()
        {
            if (trigger)
            {
                trigger.ActionEnter -= PlayAudios;
                trigger.ActionExit -= StopAudios;
            }
        }

        protected virtual void PlayAudios()
        {
            if (!EditorSetting.Setting.previewSound)
            {
                return;
            }
            isPlaying = true;
        }

        protected virtual void StopAudios()
        {
            isPlaying = false;
        }
    }
}
