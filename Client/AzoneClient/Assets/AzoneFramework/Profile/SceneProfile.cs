using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ��������
    /// </summary>
    public enum ESceneDefine
    {
        Invalid = 0,

        // ��������
        StartScene = 1,
        // ������
        MainScene = 2,
    }

    /// <summary>
    /// ��������
    /// </summary>
    [Serializable]
    public struct SceneConfig
    {
        [Header("��������")]
        public ESceneDefine define;

        [Header("������Դ��")]
        public string SceneName;

        [Header("�ű�����")]
        public string scriptType;

        [Header("�Ƿ�")]
        public bool isAddressbale;

        [Header("��פ���")]
        public EUIPanelDefine residentPanel;
    }

    /// <summary>
    /// ���������ļ�
    /// </summary>
    [CreateAssetMenu(fileName = "SceneProfile", menuName = "AzoneFramework/Profile/SceneProfile", order = 0)]
    public class SceneProfile : ScriptableObjectBase
    {
        //�����б�
        public List<SceneConfig> configs;

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
            if (configs == null || configs.Count == 0)
            {
                GameLog.Error("�������ô���---> ���������ļ�Ϊ�ջ�����Ч���ݡ�");
                return;
            }

            for (int i = 0; i < configs.Count; i++)
            {
                SceneConfig curConfig = configs[i];
                if (curConfig.define == ESceneDefine.Invalid)
                {
                    GameLog.Error($"�������ô���---> ���������ļ��е�{i}��Ԫ��Ϊ��Ч���ݡ�");
                    continue;
                }

                for (int j = i + 1; j < configs.Count; j++)
                {
                    if (curConfig.define == configs[j].define)
                    {
                        GameLog.Error($"�������ô���---> ���������ļ��е�{i}����{j}Ԫ���ظ���");
                    }
                }
            }
        }

        /// <summary>
        /// ��ȡScene����
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
        /// ��ȡScene����
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
