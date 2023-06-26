using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    public class UIManager : Singleton<UIManager>
    {
        private static readonly string UIROOT_NAME = "UIRoot";

        // UI���ڵ�
        public UIRoot Root { get; private set; }

        // UI�����
        public Canvas Stage 
        {
            get
            {
                if (Root?.stage == null)
                {
                    GameLog.Error("UIManager�����Ϊ�գ�");
                }
                return Root?.stage;
            }
        }

        // UI���Ϊ��
        public Camera UICamera
        {
            get
            {
                if (Root?.uiCamera == null)
                {
                    GameLog.Error("UIManager���Ϊ�գ�");
                }
                return Root?.uiCamera;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            // ��ʼ��UI
            Root = Resources.Load<GameObject>(UIROOT_NAME).GetComponent<UIRoot>();
            if (Root == null)
            {
                GameLog.Error("��ʼ��UIManagerʧ�ܣ�---> �޷�����UIRoot�����");
                return;
            }

            if (Root.stage == null)
            {
                GameLog.Error("��ʼ��UIManagerʧ�ܣ�---> δ����Stage�����");
                return;
            }

            if (Root.uiCamera == null)
            {
                GameLog.Error("��ʼ��UIManagerʧ�ܣ�---> δ������������");
                return;
            }

        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}
