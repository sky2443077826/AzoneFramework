using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ����������
    /// </summary>
    public enum EControllerDefine
    {
        Invalid = 0,

        // ��Ϸ���������
        GameWorld = 1,
    }

    /// <summary>
    /// ����������
    /// </summary>
    [Serializable]
    public struct ControllerConfig
    {
        [Header("����������")]
        public EControllerDefine define;

        [Header("�ű�����")]
        public string scriptType;
    }

    [CreateAssetMenu(fileName = "ControllerProfile", menuName = "AzoneFramework/Profile/ControllerProfile", order = 1)]
    public class ControllerProfile : ScriptableObjectBase
    {
        // ��������б�
        [Header("����������")]
        public List<ControllerConfig> configs;

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
                GameLog.Error("���������ô���---> �����������ļ�Ϊ�ջ�����Ч���ݡ�");
                return;
            }

            for (int i = 0; i < configs.Count; i++)
            {
                ControllerConfig curConfig = configs[i];
                if (curConfig.define == EControllerDefine.Invalid)
                {
                    GameLog.Error($"���������ô���---> �����������ļ��е�{i}��Ԫ��Ϊ��Ч���ݡ�");
                    continue;
                }

                for (int j = i + 1; j < configs.Count; j++)
                {
                    if (curConfig.define == configs[j].define)
                    {
                        GameLog.Error($"���������ô���---> �����������ļ��е�{i}����{j}Ԫ���ظ���");
                    }
                }
            }
        }


        /// <summary>
        /// ��ȡ����������
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
        /// �Ƿ����
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
