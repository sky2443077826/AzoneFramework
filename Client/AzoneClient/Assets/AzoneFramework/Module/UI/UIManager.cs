using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AzoneFramework.UI
{
    /// <summary>
    /// UI������
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        // ���ڵ���Դ��
        private static readonly string _UIROOT_NAME = "UIRoot";

        // ���������ʲ���ַ
        private static readonly string _UIPROFILE_ADDRESS = "UIProfile";

        // ���ȫ����建������
        private static readonly int _MAX_CACHE_COUNT_FULLSCREEN = 4;

        // ���������建������
        private static readonly int _MAX_CACHE_COUNT_FLOATING = 3;

        // ���ȫ����建������
        private static readonly int _MAX_CACHE_COUNT_OVERLAY = 3;

        // ��Ⱦ�㼶����ֵ
        private static readonly int _ORDER_STEP = 10;

        // UI���ڵ�
        public UIRoot Root { get; private set; }

        // UI�����
        public Canvas Stage
        {
            get
            {
                if (Root?.stage == null)
                {
                    GameLog.Error("UIManager�����Ϊ�գ�");
                }
                return Root?.stage;
            }
        }

        // UI���Ϊ��
        public Camera UICamera
        {
            get
            {
                if (Root?.uiCamera == null)
                {
                    GameLog.Error("UIManager���Ϊ�գ�");
                }
                return Root?.uiCamera;
            }
        }

        // UI�����ļ�
        private UIProfile _uIProfile;

        // ȫ����建��
        private List<UIPanel> _fullScreenCache;
        // ������建��
        private List<UIPanel> _floatingCache;
        // ȫ����建��
        private List<UIPanel> _overLayCache;


        // ��ǰ��פ����
        private EUIPanelDefine _curResident;
        // ȫ�����򿪶���
        private List<EUIPanelDefine> _fullScreenOpen;
        // �������򿪶���
        private List<EUIPanelDefine> _floatingOpen;
        // ȫ�����򿪶���
        private List<EUIPanelDefine> _overLayOpen;
        // �Ѵ򿪵����
        private Dictionary<EUIPanelDefine, UIPanel> _openPanel;

        // ��פ����Ƿ�ɼ�
        private bool _residentVisiable;

        /// <summary>
        /// ���ϲ�ȫ�����
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
        /// ����ʱ
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // ��ʼ��UI
            _uIProfile = AddressableLoader.Instance.InstantiateScriptableObject<UIProfile>(_UIPROFILE_ADDRESS);
            if (_uIProfile == null)
            {
                GameLog.Error("��ʼ��UIManagerʧ�ܣ� ---> δ��ȷ���������ļ���");
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
                GameLog.Error("��ʼ��UIManagerʧ�ܣ�---> �޷�����UIRoot�����");
                return;
            }

            if (Root.stage == null)
            {
                GameLog.Error("��ʼ��UIManagerʧ�ܣ�---> δ����Stage�����");
                return;
            }

            if (Root.uiCamera == null)
            {
                GameLog.Error("��ʼ��UIManagerʧ�ܣ�---> δ������������");
                return;
            }

            // �����¼�
            FrameEvent.Instance.Listen(EFrameEventID.LoadResidentPanel, OnLoadResidentPanel);
        }

        /// <summary>
        /// ����ʱ
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


        #region ������

        /// <summary>
        /// ����嵯������
        /// </summary>
        /// <returns></returns>
        private UIPanel PopPanelTop(UIPanelConfig config, List<EUIPanelDefine> openList)
        {
            if (openList == null)
            {
                return null;
            }

            UIPanel panel = null;
            //���UI�Ѿ��򿪣���ֻˢ��һ����Ⱦ˳��
            if (openList.Contains(config.define))
            {
                openList.Remove(config.define);
                openList.Add(config.define);
                panel = _openPanel[config.define];
            }
            return panel;
        }

        /// <summary>
        /// �ӻ�����ȡ�����
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
        /// ����������
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private UIPanel CreatePanel(UIPanelConfig config)
        {
            UIPanel panel = AddressableLoader.Instance.InstantiateUI(config.address, Type.GetType(config.scriptType)) as UIPanel;
            if (panel == null)
            {
                GameLog.Error($"�������ʧ�ܣ�---> ��Դ{config.address}�޷�����ΪUIPanel��");
                return null;
            }

            panel.Config = config;
            panel.Create();
            panel.transform.SetParent(Stage.transform);
            return panel;
        }

        /// <summary>
        /// ȡ�����
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
        /// ִ�д����
        /// </summary>
        /// <returns></returns>
        private UIPanel ExcuteOpenPanel(UIPanelConfig config, DataList args = null)
        {
            UIPanel panel = null;

            // ��������ִ������߼�
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
                // ȡ��һ��
                panel = FetchPanel(config);
                if (panel == null)
                {
                    GameLog.Error($"�����ʧ�ܣ�---> �޷���ȷ�������{config.define}");
                    return null;
                }
                _openPanel[config.define] = panel;
            }

            // ���������
            switch (config.type)
            {
                // ��ȫ��
                case EUIPanelType.FullScreen:
                    // ���س�פ���
                    if (_residentVisiable && _curResident != EUIPanelDefine.Invalid)
                    {
                        _openPanel[_curResident]?.Hide();
                        _residentVisiable = false;
                    }

                    // ������һ��ȫ�����
                    if (TopFullScreen != EUIPanelDefine.Invalid)
                    {
                        _openPanel[TopFullScreen]?.Hide();
                    }

                    // ����򿪶���
                    _fullScreenOpen.Add(config.define);

                    break;

                // ������
                case EUIPanelType.Floating:
                    _floatingOpen.Add(config.define);
                    break;

                case EUIPanelType.OverLay:
                    _overLayOpen.Add(config.define);
                    break;
            }

            panel.Open(args);
            args?.Dispose();
            //ˢ�²㼶
            RefreshPanelSortingOrder();

            return panel;
        }

        /// <summary>
        /// ִ�йر����
        /// </summary>
        private void ExcuteClosePanel(EUIPanelDefine define)
        {
            UIPanel panel = _openPanel[define];
            panel.Close();
            _openPanel.Remove(define);

            // ִ�л������
            //�������
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
                //����UI
                case EUIPanelType.Floating:
                    {
                        cacheList = _floatingCache;
                        cacheFull = _floatingCache.Count >= _MAX_CACHE_COUNT_FLOATING;
                        break;
                    }
                //ȫ��UI
                case EUIPanelType.OverLay:
                    {
                        cacheList = _overLayCache;
                        cacheFull = _overLayCache.Count >= _MAX_CACHE_COUNT_OVERLAY;
                        break;
                    }
            }

            if (cacheList != null)
            {
                // �������ˣ������ٵ�һ��
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
        /// ˢ�������Ⱦ�㼶
        /// </summary>
        private void RefreshPanelSortingOrder()
        {
            //Ĭ�ϲ㼶��1��ʼ
            int order = 1;

            //ˢ�³�פUI
            if (_curResident != EUIPanelDefine.Invalid)
            {
                SetPanelSortingOrder(_curResident, order);
                order += _ORDER_STEP;
            }

            //ˢ��ȫ��UI
            if (TopFullScreen != EUIPanelDefine.Invalid)
            {
                SetPanelSortingOrder(TopFullScreen, order);
                order += _ORDER_STEP;
            }

            //ˢ������UI
            foreach (EUIPanelDefine define in _floatingOpen)
            {
                SetPanelSortingOrder(define, order);
                order += _ORDER_STEP;
            }

            //ˢ��ȫ��UI
            foreach (EUIPanelDefine define in _overLayOpen)
            {
                SetPanelSortingOrder(define, order);
                order += _ORDER_STEP;
            }
        }

        /// <summary>
        /// ����������Ⱦ�㼶
        /// </summary>
        /// <param name="define"></param>
        /// <param name="order"></param>
        private void SetPanelSortingOrder(EUIPanelDefine define, int order)
        {
            _openPanel[define]?.SetSortingOrder(order);
        }

        /// <summary>
        /// �����(���ɲ�����פ���)
        /// </summary>
        /// <param name="define"></param>
        public UIPanel OpenPanel(EUIPanelDefine define, DataList args = null)
        {
            UIPanel panel = null;

            if (!_uIProfile.TryGetConfig(define, out UIPanelConfig config))
            {
                GameLog.Error($"�����ʧ�ܣ�����-> δ�ҵ����{define}������á�");
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
        /// ��ʾ��פ���
        /// </summary>
        /// <returns></returns>
        public UIPanel ShowResientPanel(EUIPanelDefine define)
        {
            UIPanel panel = null;
            if (_curResident == EUIPanelDefine.Invalid || _curResident != define)
            {
                GameLog.Error("��ʾ��פ���ʧ�ܣ�---> ��ǰ��פ���{_curResisdent}��Ч��");
                return panel;
            }

            // ��ʾ��פ���
            panel = _openPanel[_curResident];
            if (!_residentVisiable)
            {
                panel.Show();
                _residentVisiable = true;
            }

            // ���ȫ��UI���ر�
            if (_fullScreenOpen.Count > 0)
            {
                // �Ӻ���ǰ�ر�
                for (int i = _fullScreenOpen.Count - 1; i >= 0; --i)
                {
                    ExcuteClosePanel(define);
                }
                _fullScreenOpen.Clear();
            }
            return panel;
        }

        /// <summary>
        /// �ر����(���ɲ�����פ���)
        /// </summary>
        /// <param name="define"></param>
        public void ClosePanel(EUIPanelDefine define)
        {
            // ��ȡ����
            if (!_uIProfile.TryGetConfig(define, out UIPanelConfig config) || define == EUIPanelDefine.Invalid)
            {
                return;
            }

            // ��ִ�г�פ���
            if (config.type == EUIPanelType.Resident)
            {
                return;
            }

            // ִ�йر��߼�
            ExcuteClosePanel(define);

            // ����رպ�������
            switch (config.type)
            {
                case EUIPanelType.FullScreen:

                    _fullScreenOpen.Remove(define);

                    // �Ƿ������һ��ȫ��UI��������ʾ
                    if (_fullScreenOpen.Count >= 1)
                    {
                        EUIPanelDefine lastFullScreen = _fullScreenOpen[_fullScreenOpen.Count - 1];
                        _openPanel[lastFullScreen]?.Show();
                    }
                    // ��ʾ��פUI
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
            //ˢ�²㼶
            RefreshPanelSortingOrder();
        }

        #endregion

        #region �¼��ص�

        /// <summary>
        /// ���س�פ����
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

            // ��������
            if (!_uIProfile.TryGetConfig(define, out UIPanelConfig config))
            {
                return;
            }

            UIPanel panel = null;

            // ������ͬ��פ����
            if (_curResident != EUIPanelDefine.Invalid && _curResident == define)
            {
                // ֱ��չʾ
                ShowResientPanel(_curResident);
                return;
            }

            // �ȴ���
            panel = CreatePanel(config);
            if (panel == null)
            {
                GameLog.Error($"�򿪳�פ���ʧ�ܣ� ---> δ��ȷ���س�פ���{config.define}��");
                return;
            }

            // ���򿪲�ͬ��פ����
            if (_curResident != EUIPanelDefine.Invalid)
            {
                // �ر���һ��
                UIPanel lastResident = _openPanel[_curResident];
                _openPanel.Remove(_curResident);
                GameObject.Destroy(lastResident.gameObject);
                _curResident = EUIPanelDefine.Invalid;
                _residentVisiable = false;
            }

            // ��
            _openPanel.Add(config.define, panel);
            panel.Open();
            _curResident = config.define;
            _residentVisiable = true;
        }

        #endregion


    }

}
