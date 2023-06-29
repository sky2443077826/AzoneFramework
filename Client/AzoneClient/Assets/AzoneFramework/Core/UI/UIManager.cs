using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
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
        private EUIPanelDefine _curResisdent;
        // ȫ�����򿪶���
        private List<EUIPanelDefine> _fullScreenOpen;
        // �������򿪶���
        private List<EUIPanelDefine> _floatingOpen;
        // ȫ�����򿪶���
        private List<EUIPanelDefine> _overLayOpen;
        // �Ѵ򿪵����
        private Dictionary<EUIPanelDefine, UIPanel> _openPanel;

        // ��פ����Ƿ�ɼ�
        private bool _resisdentVisiable;

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



            Root = Resources.Load<GameObject>(_UIROOT_NAME).GetComponent<UIRoot>();
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
        }

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private T CreatePanel<T>(UIPanelConfig config) where T : UIPanel
        {
            T panel = AddressableLoader.Instance.InstantiateUI<T>(config.address);
            if (panel == null)
            {
                GameLog.Error($"�������ʧ�ܣ�---> ��Դ{config.address}δ��ȷ���ء�");
                return null;
            }

            panel.Create();
            panel.transform.SetParent(Root.transform);
            return panel;
        }

        /// <summary>
        /// ִ�д����
        /// </summary>
        /// <returns></returns>
        private UIPanel ExcuteOpenPanel(UIPanelConfig config)
        {
            UIPanel panel = null;

            // ��������ִ������߼�
            switch (config.type)
            {
                // �򿪳�פ���
                case EUIPanelType.Resident:

                    if (_curResisdent == EUIPanelDefine.Invalid || _curResisdent != config.define)
                    {
                        GameLog.Error("�����ʧ�ܣ�---> ��ǰ��פ���{_curResisdent}��Ч��");
                        return panel;
                    }
                    // ��ʾ��פ���
                    panel = _openPanel[_curResisdent];
                    if (!_resisdentVisiable)
                    {
                        panel.Show();
                        _resisdentVisiable = true;
                    }

                    // ���ȫ��UI���ر�
                    if (_fullScreenOpen.Count > 0)
                    {
                        // �Ӻ���ǰ�ر�
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
            }
        }

        /// <summary>
        /// �����
        /// </summary>
        /// <param name="define"></param>
        public UIPanel OpenPanel(EUIPanelDefine define)
        {
            UIPanel panel = null;

            if (!_uIProfile.TryGetConfig(define, out UIPanelConfig config))
            {
                GameLog.Error($"�����ʧ�ܣ�����-> δ�ҵ����{define}������á�");
                return panel;
            }



            return panel;
        }
    }

}
