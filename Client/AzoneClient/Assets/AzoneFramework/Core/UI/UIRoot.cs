using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// UI根节点
    /// </summary>
    public class UIRoot : MonoBehaviour
    {
        // 数量，保证唯一
        private static int _count;

        [Header("根面板")]
        public Canvas stage;

        [Header("UI相机组件")]
        public Camera uiCamera;

        private void Awake()
        {
            if (_count > 0)
            {
                Destroy(gameObject);
            }
            // 持久保持
            DontDestroyOnLoad(gameObject);
            _count++;

            FrameEvent.Instance.Dispatch(EFrameEventID.UIModuleInitSuccess);
        }
    }
}
