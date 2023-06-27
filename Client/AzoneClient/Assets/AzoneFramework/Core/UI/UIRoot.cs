using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// UI���ڵ�
    /// </summary>
    public class UIRoot : MonoBehaviour
    {
        // ��������֤Ψһ
        private static int _count;

        [Header("�����")]
        public Canvas stage;

        [Header("UI������")]
        public Camera uiCamera;

        private void Awake()
        {
            if (_count > 0)
            {
                Destroy(gameObject);
            }
            // �־ñ���
            DontDestroyOnLoad(gameObject);
            _count++;

            FrameEvent.Instance.Dispatch(EFrameEventID.UIModuleInitSuccess);
        }
    }
}
