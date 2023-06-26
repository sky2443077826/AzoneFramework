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
        [Header("根面板")]
        public Canvas stage;

        [Header("UI相机组件")]
        public Camera uiCamera;
    }
}
