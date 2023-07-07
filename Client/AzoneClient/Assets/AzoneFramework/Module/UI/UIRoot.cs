using UnityEngine;

namespace AzoneFramework.UI
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
        }

        private void Start()
        {
            FrameEvent.Instance.Dispatch(EFrameEventID.UIModuleInitSuccess);
        }
    }
}
