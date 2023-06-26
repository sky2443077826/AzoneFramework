using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    public class UIManager : Singleton<UIManager>
    {
        private static readonly string UIROOT_NAME = "UIRoot";

        // UI根节点
        public UIRoot Root { get; private set; }

        // UI根面板
        public Canvas Stage 
        {
            get
            {
                if (Root?.stage == null)
                {
                    GameLog.Error("UIManager根面板为空！");
                }
                return Root?.stage;
            }
        }

        // UI相机为空
        public Camera UICamera
        {
            get
            {
                if (Root?.uiCamera == null)
                {
                    GameLog.Error("UIManager相机为空！");
                }
                return Root?.uiCamera;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            // 初始化UI
            Root = Resources.Load<GameObject>(UIROOT_NAME).GetComponent<UIRoot>();
            if (Root == null)
            {
                GameLog.Error("初始化UIManager失败！---> 无法加载UIRoot组件。");
                return;
            }

            if (Root.stage == null)
            {
                GameLog.Error("初始化UIManager失败！---> 未配置Stage组件。");
                return;
            }

            if (Root.uiCamera == null)
            {
                GameLog.Error("初始化UIManager失败！---> 未配置相机组件。");
                return;
            }

        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}
