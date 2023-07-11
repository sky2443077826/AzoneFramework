using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AzoneFramework.UI
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

        // 渲染层级步进值
        private static readonly int _ORDER_STEP = 10;

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
        private EUIPanelDefine _curResident;
        // 全屏面板打开队列
        private List<EUIPanelDefine> _fullScreenOpen;
        // 浮动面板打开队列
        private List<EUIPanelDefine> _floatingOpen;
        // 全局面板打开队列
        private List<EUIPanelDefine> _overLayOpen;
        // 已打开的面板
        private Dictionary<EUIPanelDefine, UIPanel> _openPanel;

        // 常驻面板是否可见
        private bool _residentVisiable;

        /// <summary>
        /// 最上层全屏面板
        /// </summary>
        public EUIPanelDefine TopFullScreen
        {
            get
            {
                if (_fullScreenOpen != null && _fullScreenOpen.Count > 0)
                {
                    return _fullScreenOpen[_fullScreenOpen.Count - 1];
                }
                return EUIPanelDefine.Invalid;
            }
        }

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


            GameObject uiRoot = GameObject.Instantiate(Resources.Load<GameObject>(_UIROOT_NAME));
            Root = uiRoot.GetComponent<UIRoot>();
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

            // 监听事件
            FrameEvent.Instance.Listen(EFrameEventID.LoadResidentPanel, OnLoadResidentPanel);
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        protected override void OnDispose()
        {
            foreach (var panel in _fullScreenCache)
            {
                GameObject.Destroy(panel.gameObject);
            }
            _fullScreenCache.Clear();
            _fullScreenCache = null;

            foreach (var panel in _floatingCache)
            {
                GameObject.Destroy(panel.gameObject);
            }
            _floatingCache.Clear();
            _floatingCache = null;


            foreach (var panel in _overLayCache)
            {
                GameObject.Destroy(panel.gameObject);
            }
            _overLayCache.Clear();
            _overLayCache = null;


            foreach (var panel in _openPanel.Values)
            {
                GameObject.Destroy(panel.gameObject);
            }
            _openPanel.Clear();
            _openPanel = null;

            GameObject.Destroy(Root);
            Root = null;

            GameObject.Destroy(_uIProfile);
            _uIProfile = null;

            base.OnDispose();
        }


        #region 面板管理

        /// <summary>
        /// 将面板弹到顶部
        /// </summary>
        /// <returns></returns>
        private UIPanel PopPanelTop(UIPanelConfig config, List<EUIPanelDefine> openList)
        {
            if (openList == null)
            {
                return null;
            }

            UIPanel panel = null;
            //如果UI已经打开，则只刷新一下渲染顺序
            if (openList.Contains(config.define))
            {
                openList.Remove(config.define);
                openList.Add(config.define);
                panel = _openPanel[config.define];
            }
            return panel;
        }

        /// <summary>
        /// 从缓存中取出面板
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private UIPanel TakePanelFromCache(UIPanelConfig config, List<UIPanel> cacheList)
        {
            if (cacheList == null)
            {
                return null;
            }

            UIPanel panel = null;
            int panelIndex = cacheList.FindIndex(a => a.Config.define == config.define);
            if (panelIndex > -1)
            {
                panel = cacheList[panelIndex];
                cacheList.RemoveAt(panelIndex);
            }
            return panel;
        }

        /// <summary>
        /// 创建面板对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private UIPanel CreatePanel(UIPanelConfig config)
        {
            UIPanel panel = AddressableLoader.Instance.InstantiateUI(config.address, Type.GetType(config.scriptType)) as UIPanel;
            if (panel == null)
            {
                GameLog.Error($"创建面板失败！---> 资源{config.address}无法加载为UIPanel。");
                return null;
            }

            panel.Config = config;
            panel.Create();
            panel.transform.SetParent(Stage.transform);
            return panel;
        }

        /// <summary>
        /// 取出面板
        /// </summary>
        /// <returns></returns>
        private UIPanel FetchPanel(UIPanelConfig config)
        {
            List<UIPanel> cacheList = null;
            switch (config.type)
            {
                case EUIPanelType.FullScreen:
                    cacheList = _fullScreenCache;
                    break;
                case EUIPanelType.Floating:
                    cacheList = _floatingCache;
                    break;
                case EUIPanelType.OverLay:
                    cacheList = _overLayCache;
                    break;
            }

            if (cacheList == null)
            {
                return null;
            }

            UIPanel panel = TakePanelFromCache(config, cacheList);
            if (panel == null)
            {
                panel = CreatePanel(config);
            }

            return panel;
        }

        /// <summary>
        /// 执行打开面板
        /// </summary>
        /// <returns></returns>
        private UIPanel ExcuteOpenPanel(UIPanelConfig config, DataList args = null)
        {
            UIPanel panel = null;

            // 根据类型执行相关逻辑
            switch (config.type)
            {
                case EUIPanelType.FullScreen:
                    panel = PopPanelTop(config, _fullScreenOpen);
                    break;

                case EUIPanelType.Floating:
                    panel = PopPanelTop(config, _floatingOpen);
                    break;

                case EUIPanelType.OverLay:
                    panel = PopPanelTop(config, _overLayOpen);
                    break;
            }

            if (panel == null)
            {
                // 取出一个
                panel = FetchPanel(config);
                if (panel == null)
                {
                    GameLog.Error($"打开面板失败！---> 无法正确加载面板{config.define}");
                    return null;
                }
                _openPanel[config.define] = panel;
            }

            // 处理打开流程
            switch (config.type)
            {
                // 打开全屏
                case EUIPanelType.FullScreen:
                    // 隐藏常驻面板
                    if (_residentVisiable && _curResident != EUIPanelDefine.Invalid)
                    {
                        _openPanel[_curResident]?.Hide();
                        _residentVisiable = false;
                    }

                    // 隐藏上一个全屏面板
                    if (TopFullScreen != EUIPanelDefine.Invalid)
                    {
                        _openPanel[TopFullScreen]?.Hide();
                    }

                    // 加入打开队列
                    _fullScreenOpen.Add(config.define);

                    break;

                // 打开悬浮
                case EUIPanelType.Floating:
                    _floatingOpen.Add(config.define);
                    break;

                case EUIPanelType.OverLay:
                    _overLayOpen.Add(config.define);
                    break;
            }

            panel.Open(args);
            args?.Dispose();
            //刷新层级
            RefreshPanelSortingOrder();

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
                else
                {
                    cacheList.Add(panel);
                }
            }
        }

        /// <summary>
        /// 刷新面板渲染层级
        /// </summary>
        private void RefreshPanelSortingOrder()
        {
            //默认层级从1开始
            int order = 1;

            //刷新常驻UI
            if (_curResident != EUIPanelDefine.Invalid)
            {
                SetPanelSortingOrder(_curResident, order);
                order += _ORDER_STEP;
            }

            //刷新全屏UI
            if (TopFullScreen != EUIPanelDefine.Invalid)
            {
                SetPanelSortingOrder(TopFullScreen, order);
                order += _ORDER_STEP;
            }

            //刷新悬浮UI
            foreach (EUIPanelDefine define in _floatingOpen)
            {
                SetPanelSortingOrder(define, order);
                order += _ORDER_STEP;
            }

            //刷新全局UI
            foreach (EUIPanelDefine define in _overLayOpen)
            {
                SetPanelSortingOrder(define, order);
                order += _ORDER_STEP;
            }
        }

        /// <summary>
        /// 设置面板的渲染层级
        /// </summary>
        /// <param name="define"></param>
        /// <param name="order"></param>
        private void SetPanelSortingOrder(EUIPanelDefine define, int order)
        {
            _openPanel[define]?.SetSortingOrder(order);
        }

        /// <summary>
        /// 打开面板(不可操作常驻面板)
        /// </summary>
        /// <param name="define"></param>
        public UIPanel OpenPanel(EUIPanelDefine define, DataList args = null)
        {
            UIPanel panel = null;

            if (!_uIProfile.TryGetConfig(define, out UIPanelConfig config))
            {
                GameLog.Error($"打开面板失败！——-> 未找到面板{define}相关配置。");
                return panel;
            }

            if (config.type == EUIPanelType.Resident)
            {
                return panel;
            }

            panel = ExcuteOpenPanel(config, args);
            return panel;
        }

        /// <summary>
        /// 显示常驻面板
        /// </summary>
        /// <returns></returns>
        public UIPanel ShowResientPanel(EUIPanelDefine define)
        {
            UIPanel panel = null;
            if (_curResident == EUIPanelDefine.Invalid || _curResident != define)
            {
                GameLog.Error("显示常驻面板失败！---> 当前常驻面板{_curResisdent}无效。");
                return panel;
            }

            // 显示常驻面板
            panel = _openPanel[_curResident];
            if (!_residentVisiable)
            {
                panel.Show();
                _residentVisiable = true;
            }

            // 清空全屏UI并关闭
            if (_fullScreenOpen.Count > 0)
            {
                // 从后往前关闭
                for (int i = _fullScreenOpen.Count - 1; i >= 0; --i)
                {
                    ExcuteClosePanel(define);
                }
                _fullScreenOpen.Clear();
            }
            return panel;
        }

        /// <summary>
        /// 关闭面板(不可操作常驻面板)
        /// </summary>
        /// <param name="define"></param>
        public void ClosePanel(EUIPanelDefine define)
        {
            // 获取配置
            if (!_uIProfile.TryGetConfig(define, out UIPanelConfig config) || define == EUIPanelDefine.Invalid)
            {
                return;
            }

            // 不执行常驻面板
            if (config.type == EUIPanelType.Resident)
            {
                return;
            }

            // 执行关闭逻辑
            ExcuteClosePanel(define);

            // 处理关闭后续流程
            switch (config.type)
            {
                case EUIPanelType.FullScreen:

                    _fullScreenOpen.Remove(define);

                    // 是否存在上一个全屏UI，有则显示
                    if (_fullScreenOpen.Count >= 1)
                    {
                        EUIPanelDefine lastFullScreen = _fullScreenOpen[_fullScreenOpen.Count - 1];
                        _openPanel[lastFullScreen]?.Show();
                    }
                    // 显示常驻UI
                    else if (!_residentVisiable && _curResident != EUIPanelDefine.Invalid)
                    {
                        _openPanel[_curResident]?.Show();
                        _residentVisiable = true;
                    }

                    break;
                case EUIPanelType.Floating:
                    _floatingOpen.Remove(define);
                    break;
                case EUIPanelType.OverLay:
                    _overLayOpen.Remove(define);
                    break;
            }
            if (config.type == EUIPanelType.FullScreen)
            {

            }
            //刷新层级
            RefreshPanelSortingOrder();
        }

        #endregion

        #region 事件回调

        /// <summary>
        /// 加载常驻界面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataList"></param>
        private void OnLoadResidentPanel(EFrameEventID id, DataList dataList)
        {
            if (id != EFrameEventID.LoadResidentPanel || dataList == null)
            {
                return;
            }

            EUIPanelDefine define = (EUIPanelDefine)dataList.ReadInt(0);
            if (!Enum.IsDefined(typeof(EUIPanelDefine), define) || define == EUIPanelDefine.Invalid)
            {
                return;
            }

            // 加载配置
            if (!_uIProfile.TryGetConfig(define, out UIPanelConfig config))
            {
                return;
            }

            UIPanel panel = null;

            // 若打开相同常驻界面
            if (_curResident != EUIPanelDefine.Invalid && _curResident == define)
            {
                // 直接展示
                ShowResientPanel(_curResident);
                return;
            }

            // 先创建
            panel = CreatePanel(config);
            if (panel == null)
            {
                GameLog.Error($"打开常驻面板失败！ ---> 未正确加载常驻面板{config.define}。");
                return;
            }

            // 若打开不同常驻界面
            if (_curResident != EUIPanelDefine.Invalid)
            {
                // 关闭上一个
                UIPanel lastResident = _openPanel[_curResident];
                _openPanel.Remove(_curResident);
                GameObject.Destroy(lastResident.gameObject);
                _curResident = EUIPanelDefine.Invalid;
                _residentVisiable = false;
            }

            // 打开
            _openPanel.Add(config.define, panel);
            panel.Open();
            _curResident = config.define;
            _residentVisiable = true;
        }

        #endregion


    }

}
