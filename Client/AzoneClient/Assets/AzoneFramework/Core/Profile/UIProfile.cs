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
         * 定义：跟随场景绑定，跟随场景一起创建销毁。单场景仅存在一个。
         * 渲染层级：最低。
         * 缓存机制：无。
         */
        Resident = 0,

        /*
         * 全屏界面
         * 定义：铺满屏幕，该界面之下不允许存在其他界面。
         * 渲染层级：顺序高于常驻UI，当打开全屏UI时，常驻UI自动隐藏。
         * 开关规则：可同时打开多个全屏，但只显示最后打开的。之前打开的全屏UI进入显示栈中。
         *         关闭全屏界面时，若显示栈中有则打开上一个全屏界面，无则打开常驻界面。
         *         同时有数量限制，若超过则关闭最早显示的全屏界面。
         *         直接回到常驻界面，会清空全屏的显示栈，并关闭其中的全屏界面
         * 缓存机制：被关闭的全屏界面进入缓存池，超过数量则销毁最早的。
         * 
         */
        FullScreen = 1,

        /*
         * 悬浮界面
         * 定义：打开时弹出在其他界面之上。
         * 渲染层级：高于常驻和全屏界面。同类按打开顺序排列，越后的层级越高。
         * 开关规则：打开或关闭时，不影响任何其他界面。不限制打开数量，注意及时关闭。
         * 缓存机制：同上。
         */
        Floating = 2,

        /*
         * 全局界面
         * 定义：会覆盖在所有面板之上。
         * 渲染层级：最高。同类按打开顺序排列，越后的层级越高。
         * 开关规则：打开或关闭时，不影响任何其他界面。不限制打开数量，注意及时关闭。
         * 缓存机制：同上。
         */
        OverLay = 3,
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

        [Header("面板类型")]
        public EUIPanelType type;
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
