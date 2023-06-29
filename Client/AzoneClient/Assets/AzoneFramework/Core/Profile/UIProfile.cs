using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// UI��嶨��
    /// </summary>
    public enum EUIPanelDefine 
    {
        Invalid = 0,

        LoadingPanel = 1,       // ���ؽ���
        StartPanel = 2,         // ��ʼ����
    }

    /// <summary>
    /// UI�������
    /// </summary>
    public enum EUIPanelType
    {
        /*
         * ��פ����
         * ���壺���泡���󶨣����泡��һ�𴴽����١�������������һ����
         * ��Ⱦ�㼶����͡�
         * ������ƣ��ޡ�
         */
        Resident = 0,

        /*
         * ȫ������
         * ���壺������Ļ���ý���֮�²���������������档
         * ��Ⱦ�㼶��˳����ڳ�פUI������ȫ��UIʱ����פUI�Զ����ء�
         * ���ع��򣺿�ͬʱ�򿪶��ȫ������ֻ��ʾ���򿪵ġ�֮ǰ�򿪵�ȫ��UI������ʾջ�С�
         *         �ر�ȫ������ʱ������ʾջ���������һ��ȫ�����棬����򿪳�פ���档
         *         ͬʱ���������ƣ���������ر�������ʾ��ȫ�����档
         *         ֱ�ӻص���פ���棬�����ȫ������ʾջ�����ر����е�ȫ������
         * ������ƣ����رյ�ȫ��������뻺��أ�������������������ġ�
         * 
         */
        FullScreen = 1,

        /*
         * ��������
         * ���壺��ʱ��������������֮�ϡ�
         * ��Ⱦ�㼶�����ڳ�פ��ȫ�����档ͬ�ఴ��˳�����У�Խ��Ĳ㼶Խ�ߡ�
         * ���ع��򣺴򿪻�ر�ʱ����Ӱ���κ��������档�����ƴ�������ע�⼰ʱ�رա�
         * ������ƣ�ͬ�ϡ�
         */
        Floating = 2,

        /*
         * ȫ�ֽ���
         * ���壺�Ḳ�����������֮�ϡ�
         * ��Ⱦ�㼶����ߡ�ͬ�ఴ��˳�����У�Խ��Ĳ㼶Խ�ߡ�
         * ���ع��򣺴򿪻�ر�ʱ����Ӱ���κ��������档�����ƴ�������ע�⼰ʱ�رա�
         * ������ƣ�ͬ�ϡ�
         */
        OverLay = 3,
    }

    /// <summary>
    /// ��������
    /// </summary>
    [Serializable]
    public struct UIPanelConfig
    {
        [Header("UI����")]
        public EUIPanelDefine define;

        [Header("UI��Դ��ַ")]
        public string address;

        [Header("�ű�����")]
        public string scriptType;

        [Header("�������")]
        public EUIPanelType type;
    }

    [CreateAssetMenu(fileName = "UIProfile", menuName = "AzoneFramework/Profile/UIProfile", order = 1)]
    public class UIProfile : ScriptableObjectBase
    {
        // ��������б�
        [Header("UI�������")]
        public List<UIPanelConfig> panelConfigs;

        private void Awake()
        {
            //����Ƿ�����ظ�����Ч����
            CheckDataValid();
        }

        /// <summary>
        /// ���������Ч��
        /// </summary>
        private void CheckDataValid()
        {
            if (panelConfigs == null || panelConfigs.Count == 0)
            {
                GameLog.Error("UI���ô���---> UI�����ļ�Ϊ�ջ�����Ч���ݡ�");
                return;
            }

            for (int i = 0; i < panelConfigs.Count; i++)
            {
                UIPanelConfig curConfig = panelConfigs[i];
                if (curConfig.define == EUIPanelDefine.Invalid)
                {
                    GameLog.Error($"UI���ô���---> UI�����ļ��е�{i}��Ԫ��Ϊ��Ч���ݡ�");
                    continue;
                }

                for (int j = i + 1; j < panelConfigs.Count; j++)
                {
                    if (curConfig.define == panelConfigs[j].define)
                    {
                        GameLog.Error($"UI���ô���---> UI�����ļ��е�{i}����{j}Ԫ���ظ���");
                    }
                }
            }
        }


        /// <summary>
        /// ��ȡUI�������
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
        /// �Ƿ����
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
