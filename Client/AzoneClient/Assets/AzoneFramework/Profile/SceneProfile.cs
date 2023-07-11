using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 场景定义
    /// </summary>
    public enum ESceneDefine
    {
        Invalid = 0,

        // 启动场景
        StartScene = 1,
        // 主场景
        MainScene = 2,
    }

    /// <summary>
    /// 场景配置
    /// </summary>
    [Serializable]
    public struct SceneConfig
    {
        [Header("场景类型")]
        public ESceneDefine define;

        [Header("场景资源名")]
        public string SceneName;

        [Header("脚本类型")]
        public string scriptType;

        [Header("是否")]
        public bool isAddressbale;

        [Header("常驻面板")]
        public EUIPanelDefine residentPanel;
    }

    /// <summary>
    /// 场景配置文件
    /// </summary>
    [CreateAssetMenu(fileName = "SceneProfile", menuName = "AzoneFramework/Profile/SceneProfile", order = 0)]
    public class SceneProfile : ScriptableObjectBase
    {
        //配置列表
        public List<SceneConfig> configs;

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
                GameLog.Error("场景配置错误！---> 场景配置文件为空或无有效数据。");
                return;
            }

            for (int i = 0; i < configs.Count; i++)
            {
                SceneConfig curConfig = configs[i];
                if (curConfig.define == ESceneDefine.Invalid)
                {
                    GameLog.Error($"场景配置错误！---> 场景配置文件中第{i}个元素为无效数据。");
                    continue;
                }

                for (int j = i + 1; j < configs.Count; j++)
                {
                    if (curConfig.define == configs[j].define)
                    {
                        GameLog.Error($"场景配置错误！---> 场景配置文件中第{i}个和{j}元素重复。");
                    }
                }
            }
        }

        /// <summary>
        /// 获取Scene配置
        /// </summary>
        /// <param name="define"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool TryGetSceneConfig(ESceneDefine define, out SceneConfig config)
        {
            config = default(SceneConfig);

            if (define == ESceneDefine.Invalid)
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
        /// 获取Scene配置
        /// </summary>
        /// <param name="define"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool Contains(ESceneDefine define)
        {
            if (define == ESceneDefine.Invalid)
            {
                return false;
            }

            SceneConfig config = configs.Find(a => a.define == define);

            return define == config.define;
        }
    }

}
