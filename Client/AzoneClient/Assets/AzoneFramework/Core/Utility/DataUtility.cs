using System;
using System.Collections.Generic;

namespace AzoneFramework
{
    /// <summary>
    /// QS通用函数类
    /// </summary>
    public class DataUtility : Singleton<DataUtility>
    {
        /// <summary>
        /// 视图类型到视图ID的映射
        /// </summary>
        private Dictionary<eViewPort, int> _viewType2Config;
        /// <summary>
        /// 性别类型映射到角色ID
        /// </summary>
        private Dictionary<eGender, int> _genderType2Config;

        /// <summary>
        /// UID索引
        /// </summary>
        private static uint gUIDIndex = 0;

        /// <summary>
        /// RID 基础索引
        /// </summary>
        private static uint gBaseRID = 1672502400;

        protected override void OnCreate()
        {
            _viewType2Config = new Dictionary<eViewPort, int>();
            _genderType2Config = new Dictionary<eGender, int>();
            // 解析视图结构
            if (!ParseViewPortConfig())
            {
                GameLog.Error("解析viewport映射失败。");
                return;
            }

            // 解析角色结构
            if (!ParseRoleConfig())
            {
                GameLog.Error("解析Role映射失败。");
                return;
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// 根据configID生成UID
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ulong GenerateUID(int config)
        {
            /**
             *     | Config | index | unix时间戳 |
             *     | 16bit  | 16bit |   32bit   |
             * */
            if (gUIDIndex >= 0xfff0)
            {
                gUIDIndex = 0;
            }

            uint lowValue = (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() & UInt32.MaxValue);
            ulong highValue = (ushort)(config & 0xffff);
            return (((highValue << 16) + gUIDIndex++) << 32) + lowValue;
        }

        /// <summary>
        /// 生成RID
        /// </summary>
        /// <returns></returns>
        public static int GenerateRID()
        {
            return (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - gBaseRID);
        }

        /// <summary>
        /// 属性字符串转化为类型
        /// </summary>
        public static eDataType Str2DataType(string str)
        {
            if (!Enum.TryParse(str, true, out eDataType type))
            {
                GameLog.Error("未支持的数据类型：{0}", str);
                return eDataType.Invalid;
            }

            return type;
        }

        /// <summary>
        /// 根据视图类型获取视图id
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetViewPortConfig(eViewPort type)
        {
            if (!_viewType2Config.ContainsKey(type))
            {
                GameLog.Error($"没有找到{type}类型的视图。");
                return 0;
            }
            return _viewType2Config[type];
        }

        /// <summary>
        /// 根据性别类型获取角色ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetRoleConfig(eGender type)
        {
            if (!_genderType2Config.ContainsKey(type))
            {
                GameLog.Error($"没有找到性别{type}的角色。");
                return 0;
            }
            return _genderType2Config[type];
        }


        /// <summary>
        /// 解析viewport类型的配置
        /// </summary>
        /// <returns></returns>
        private bool ParseViewPortConfig()
        {
            // 获取所有的veiwport类型
            if (!ConfigManager.Instance.GetConfigIDsByClass("ViewPort", out List<int> IDs))
            {
                GameLog.Error("获取所有ViewPort失败。");
                return false;
            }

            if (IDs.Count == 0)
            {
                GameLog.Error("ViewPort类型的数据为0");
                return false;
            }

            for (int index = 0; index < IDs.Count; ++index)
            {
                // 获取视图类型
                int type = ConfigManager.Instance.GetConfigInt(IDs[index], "ViewType");
                if (!Enum.IsDefined(typeof(eViewPort), type))
                {
                    GameLog.Error($"无效的视图类型。{IDs[index]} -> {type}");
                    continue;
                }

                if (_viewType2Config.ContainsKey((eViewPort)type))
                {
                    GameLog.Error($"重复的视图结构。{IDs[index]} -> {type}");
                    continue;
                }

                _viewType2Config.Add((eViewPort)type, IDs[index]);
            }

            return true;
        }

        /// <summary>
        /// 解析Role类型的配置
        /// </summary>
        /// <returns></returns>
        private bool ParseRoleConfig()
        {
            // 获取所有的Role类型
            if (!ConfigManager.Instance.GetConfigIDsByClass("Role", out List<int> IDs))
            {
                GameLog.Error("获取所有Role失败。");
                return false;
            }

            if (IDs.Count == 0)
            {
                GameLog.Error("Role类型的数据为0");
                return false;
            }

            for (int index = 0; index < IDs.Count; ++index)
            {
                // 获取视图类型
                int type = ConfigManager.Instance.GetConfigInt(IDs[index], "Gender");
                if (!Enum.IsDefined(typeof(eGender), type))
                {
                    GameLog.Error($"无效的性别类型。{IDs[index]} -> {type}");
                    continue;
                }

                if (_genderType2Config.ContainsKey((eGender)type))
                {
                    GameLog.Error($"重复的视图结构。{IDs[index]} -> {type}");
                    continue;
                }

                _genderType2Config.Add((eGender)type, IDs[index]);
            }

            return true;
        }
    }
}
