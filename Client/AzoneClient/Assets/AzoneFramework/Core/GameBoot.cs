using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AzoneFramework
{
    /// <summary>
    /// ��Ϸ�������
    /// </summary>
    public class GameBoot : MonoBehaviour
    {
        [Header("��־�ȼ�")]
        public ELogLevel logLevel;

        [Header("��־�Ƿ񵼳�(�Ǳ༭��)")]
        public bool isSaveLog;

        private void Start()
        {

#if UNITY_EDITOR
            // �༭���������־
            isSaveLog = false;
#endif

            // ��ʼ����־ϵͳ
            GameLog.Init(logLevel, isSaveLog);
            // ��ʼ����Դ������
            AssetLoader.Instance.Create();

            LaunchGame();
        }

        /// <summary>
        /// ������Ϸ
        /// </summary>
        private void LaunchGame()
        {
            GameLog.Normal("===��Ϸ����===");
        }
    }
}

