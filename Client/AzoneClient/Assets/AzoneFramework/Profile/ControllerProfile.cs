using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 控制器定义
    /// </summary>
    public enum EControllerDefine
    {
        Invalid = 0,

        // 游戏世界控制器
        GameWorld = 1,
    }

    /// <summary>
    /// 控制器配置
    /// </summary>
    [Serializable]
    public struct ControllerConfig
    {
        [Header("控制器定义")]
        public EControllerDefine define;

        [Header("脚本类型")]
        public string scriptType;
    }

    [CreateAssetMenu(fileName = "ControllerProfile", menuName = "AzoneFramework/Profile/ControllerProfile", order = 1)]
    public class ControllerProfile : ScriptableObjectBase
    {
        // 面板配置列表
        [Header("控制器配置")]
        public List<ControllerConfig> configs;

        private void Awake()
        {
            //检查是否存在重复和无效数据
            CheckDataValid();
        }

        /// <summary>
        /// 检查数据有效性
        /// </summary>
        private void CheckDataValid()
        {
            if (configs == null || configs.Count == 0)
            {
                GameLog.Error("控制器配置错误！---> 控制器配置文件为空或无有效数据。");
                return;
            }

            for (int i = 0; i < configs.Count; i++)
            {
                ControllerConfig curConfig = configs[i];
                if (curConfig.define == EControllerDefine.Invalid)
                {
                    GameLog.Error($"控制器配置错误！---> 控制器配置文件中第{i}个元素为无效数据。");
                    continue;
                }

                for (int j = i + 1; j < configs.Count; j++)
                {
                    if (curConfig.define == configs[j].define)
                    {
                        GameLog.Error($"控制器配置错误！---> 控制器配置文件中第{i}个和{j}元素重复。");
                    }
                }
            }
        }


        /// <summary>
        /// 获取控制器配置
        /// </summary>
        /// <param name="define"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool TryGetConfig(EControllerDefine define, out ControllerConfig config)
        {
            config = default(ControllerConfig);

            if (define == EControllerDefine.Invalid)
            {
                return false;
            }

            config = configs.Find(a => a.define == define);

            if (define != config.define)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="define"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool Contains(EControllerDefine define)
        {
            if (define == EControllerDefine.Invalid)
            {
                return false;
            }

            ControllerConfig config = configs.Find(a => a.define == define);

            return define == config.define;
        }
    }
}
