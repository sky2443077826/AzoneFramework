using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// UI面板定义
    /// </summary>
    public enum EUIPanelDefine 
    {
        Invalid = 0,

        LoadingPanel = 1,       // 加载界面
        StartPanel = 2,         // 开始界面
    }

    /// <summary>
    /// UI面板类型
    /// </summary>
    public enum EUIPanelType
    {
        /*
         * 常驻界面
         * 跟随场景绑定，在场景中默认显示。
         */
        Resident = 0,

        /*
         * 全屏界面
         * 铺满屏幕的UI,
         */
        FullScreen = 1,
    }

    /// <summary>
    /// 场景配置
    /// </summary>
    [Serializable]
    public struct UIPanelConfig
    {
        [Header("UI定义")]
        public EUIPanelDefine define;

        [Header("UI资源地址")]
        public string address;

        [Header("脚本类型")]
        public string scriptType;
    }

    [CreateAssetMenu(fileName = "UIProfile", menuName = "AzoneFramework/Profile/UIProfile", order = 1)]
    public class UIProfile : ScriptableObjectBase
    {
        // 面板配置列表
        [Header("UI面板配置")]
        public List<UIPanelConfig> panelConfigs;

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
            if (panelConfigs == null || panelConfigs.Count == 0)
            {
                GameLog.Error("UI配置错误！---> UI配置文件为空或无有效数据。");
                return;
            }

            for (int i = 0; i < panelConfigs.Count; i++)
            {
                UIPanelConfig curConfig = panelConfigs[i];
                if (curConfig.define == EUIPanelDefine.Invalid)
                {
                    GameLog.Error($"UI配置错误！---> UI配置文件中第{i}个元素为无效数据。");
                    continue;
                }

                for (int j = i + 1; j < panelConfigs.Count; j++)
                {
                    if (curConfig.define == panelConfigs[j].define)
                    {
                        GameLog.Error($"UI配置错误！---> UI配置文件中第{i}个和{j}元素重复。");
                    }
                }
            }
        }


        /// <summary>
        /// 获取UI面板配置
        /// </summary>
        /// <param name="define"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool TryGetConfig(EUIPanelDefine define, out UIPanelConfig config)
        {
            config = default(UIPanelConfig);

            if (define == EUIPanelDefine.Invalid)
            {
                return false;
            }

            config = panelConfigs.Find(a => a.define == define);

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
        public bool Contains(EUIPanelDefine define)
        {
            if (define == EUIPanelDefine.Invalid)
            {
                return false;
            }

            UIPanelConfig config = panelConfigs.Find(a => a.define == define);

            return define == config.define;
        }
    }
}
