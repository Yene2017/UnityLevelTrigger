using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelTrigger
{
    public class FmodEventUnit : BaseLevelUnit
    {
        [Header("Event")]
        [FormerlySerializedAs("_event")]
        public string Event = "";
        public List<EventParam> startParams = new List<EventParam>();
        public bool overrideAttenuation = false;
        public float minDistance = 5f;
        public float maxDistance = 20f;
        public bool useWeather = false;
        public bool useDaytime = false;

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
