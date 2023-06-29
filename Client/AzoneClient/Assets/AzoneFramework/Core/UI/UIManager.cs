using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        // 根节点资源名
        private static readonly string _UIROOT_NAME = "UIRoot";

        // 场景配置资产地址
        private static readonly string _UIPROFILE_ADDRESS = "UIProfile";

        // 最大全屏面板缓存数量
        private static readonly int _MAX_CACHE_COUNT_FULLSCREEN = 4;

        // 最大悬浮面板缓存数量
        private static readonly int _MAX_CACHE_COUNT_FLOATING = 3;

        // 最大全局面板缓存数量
        private static readonly int _MAX_CACHE_COUNT_OVERLAY = 3;

        // UI根节点
        public UIRoot Root { get; private set; }

        // UI根面板
        public Canvas Stage
        {
            get
            {
                if (Root?.stage == null)
                {
                    GameLog.Error("UIManager根面板为空！");
                }
                return Root?.stage;
            }
        }

        // UI相机为空
        public Camera UICamera
        {
            get
            {
                if (Root?.uiCamera == null)
                {
                    GameLog.Error("UIManager相机为空！");
                }
                return Root?.uiCamera;
            }
        }

        // UI配置文件
        private UIProfile _uIProfile;

        // 全屏面板缓存
        private List<UIPanel> _fullScreenCache;
        // 浮动面板缓存
        private List<UIPanel> _floatingCache;
        // 全局面板缓存
        private List<UIPanel> _overLayCache;


        // 当前常驻界面
        private EUIPanelDefine _curResisdent;
        // 全屏面板打开队列
        private List<EUIPanelDefine> _fullScreenOpen;
        // 浮动面板打开队列
        private List<EUIPanelDefine> _floatingOpen;
        // 全局面板打开队列
        private List<EUIPanelDefine> _overLayOpen;
        // 已打开的面板
        private Dictionary<EUIPanelDefine, UIPanel> _openPanel;

        // 常驻面板是否可见
        private bool _resisdentVisiable;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // 初始化UI
            _uIProfile = AddressableLoader.Instance.InstantiateScriptableObject<UIProfile>(_UIPROFILE_ADDRESS);
            if (_uIProfile == null)
            {
                GameLog.Error("初始化UIManager失败！ ---> 未正确加载配置文件。");
                return;
            }

            _fullScreenCache = new List<UIPanel>();
            _floatingCache = new List<UIPanel>();
            _overLayCache = new List<UIPanel>();

            _fullScreenOpen = new List<EUIPanelDefine>();
            _floatingOpen = new List<EUIPanelDefine>();
            _overLayOpen = new List<EUIPanelDefine>();
            _openPanel = new Dictionary<EUIPanelDefine, UIPanel>();



            Root = Resources.Load<GameObject>(_UIROOT_NAME).GetComponent<UIRoot>();
            if (Root == null)
            {
                GameLog.Error("初始化UIManager失败！---> 无法加载UIRoot组件。");
                return;
            }

            if (Root.stage == null)
            {
                GameLog.Error("初始化UIManager失败！---> 未配置Stage组件。");
                return;
            }

            if (Root.uiCamera == null)
            {
                GameLog.Error("初始化UIManager失败！---> 未配置相机组件。");
                return;
            }
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// 创建面板对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private T CreatePanel<T>(UIPanelConfig config) where T : UIPanel
        {
            T panel = AddressableLoader.Instance.InstantiateUI<T>(config.address);
            if (panel == null)
            {
                GameLog.Error($"创建面板失败！---> 资源{config.address}未正确加载。");
                return null;
            }

            panel.Create();
            panel.transform.SetParent(Root.transform);
            return panel;
        }

        /// <summary>
        /// 执行打开面板
        /// </summary>
        /// <returns></returns>
        private UIPanel ExcuteOpenPanel(UIPanelConfig config)
        {
            UIPanel panel = null;

            // 根据类型执行相关逻辑
            switch (config.type)
            {
                // 打开常驻面板
                case EUIPanelType.Resident:

                    if (_curResisdent == EUIPanelDefine.Invalid || _curResisdent != config.define)
                    {
                        GameLog.Error("打开面板失败！---> 当前常驻面板{_curResisdent}无效。");
                        return panel;
                    }
                    // 显示常驻面板
                    panel = _openPanel[_curResisdent];
                    if (!_resisdentVisiable)
                    {
                        panel.Show();
                        _resisdentVisiable = true;
                    }

                    // 清空全屏UI并关闭
                    if (_fullScreenOpen.Count > 0)
                    {
                        // 从后往前关闭
                        for (int i = _fullScreenOpen.Count - 1; i >= 0; --i)
                        {
                            ExcuteClosePanel(config.define);
                        }
                        _fullScreenOpen.Clear();
                    }
                    return panel;

                case EUIPanelType.FullScreen:
                    break;
                case EUIPanelType.Floating:
                    break;
                case EUIPanelType.OverLay:
                    break;
            }

            return panel;
        }

        /// <summary>
        /// 执行关闭面板
        /// </summary>
        private void ExcuteClosePanel(EUIPanelDefine define)
        {
            UIPanel panel = _openPanel[define];
            panel.Close();
            _openPanel.Remove(define);

            // 执行缓存机制
            //缓存管理
            List<UIPanel> cacheList = null;
            bool cacheFull = false;

            switch (panel.Config.type)
            {
                case EUIPanelType.FullScreen:
                    {
                        cacheList = _fullScreenCache;
                        cacheFull = _fullScreenCache.Count >= _MAX_CACHE_COUNT_FULLSCREEN;
                    }
                    break;
                //悬浮UI
                case EUIPanelType.Floating:
                    {
                        cacheList = _floatingCache;
                        cacheFull = _floatingCache.Count >= _MAX_CACHE_COUNT_FLOATING;
                        break;
                    }
                //全局UI
                case EUIPanelType.OverLay:
                    {
                        cacheList = _overLayCache;
                        cacheFull = _overLayCache.Count >= _MAX_CACHE_COUNT_OVERLAY;
                        break;
                    }
            }

            if (cacheList != null)
            {
                // 缓存满了，则销毁第一个
                if (cacheFull)
                {
                    UIPanel first = cacheList[0];
                    GameObject.Destroy(first.gameObject);
                    cacheList.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="define"></param>
        public UIPanel OpenPanel(EUIPanelDefine define)
        {
            UIPanel panel = null;

            if (!_uIProfile.TryGetConfig(define, out UIPanelConfig config))
            {
                GameLog.Error($"打开面板失败！——-> 未找到面板{define}相关配置。");
                return panel;
            }



            return panel;
        }
    }

}
