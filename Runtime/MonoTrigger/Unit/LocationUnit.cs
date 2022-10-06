#if UNITY_EDITOR
using System;
using UnityEngine;

namespace LevelTrigger
{
    public class LocationUnit : BaseLevelUnit
    {
        [Header("配置内容")]
        public Location location = new Location();

        public override UnitCategory category
        {
            get
            {
                return UnitCategory.Location;
            }
        }
    }

    [Serializable]
    public struct Location
    {
        [Tooltip("给服务端用的标识id")]
        public int id;
        [Tooltip("用来拼欢迎字幕")]
        public string name;
        [Tooltip("区域类型")]
        public LocationType type;
        [Tooltip("播放的脚本动画名字")]
        public string cutscene;
        [Tooltip("播放的音乐名字")]
        public string music;
    }

    public enum LocationType
    {
        None = 0,
        First = 1,
        Second = 2,
        Landscape = 4,
    }

}

#endif
