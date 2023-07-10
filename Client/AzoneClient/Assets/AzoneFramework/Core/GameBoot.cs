using AzoneFramework.Addressable;
using AzoneFramework.Controller;
using AzoneFramework.Scene;
using AzoneFramework.UI;
using System;
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
            // ��ʼ���¼��ɷ���(��ܺ���Ϸ)
            FrameEvent.Instance.Create();
            FrameEvent.Instance.Listen(EFrameEventID.UIModuleInitSuccess, OnUIModuleInitSuccess);

            // ����GameMono����
            GameMonoRoot.CreateInstance();
            // ��ʼ����Դ������
            AddressableLoader.Instance.Create();
            // ������������
            ControllerManager.Instance.Create();
            // ��ʼ������������
            SceneLoader.Instance.Create();
            // ��ʼ��UI������
            UIManager.Instance.Create();
        }

        /// <summary>
        /// ������Ϸ
        /// </summary>
        private void LaunchGame()
        {
            GameLog.Normal("===��Ϸ����===");
            // ������Ϸ���������
            GameWorldController worldController = ControllerManager.Instance.CreateController<GameWorldController>(EControllerDefine.GameWorld);
            worldController.EnterGameWorld();
        }

        /// <summary>
        /// ��UIģ��������
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataList"></param>
        private void OnUIModuleInitSuccess(EFrameEventID id, DataList dataList)
        {
            LaunchGame();
        }
    }
}

