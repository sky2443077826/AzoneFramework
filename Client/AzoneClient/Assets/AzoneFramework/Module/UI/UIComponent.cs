using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AzoneFramework.UI
{
    /// <summary>
    /// UI�Զ����������
    /// </summary>
    public class UIComponent : UIBase
    {
        /// <summary>
        /// �����
        /// </summary>
        protected Dictionary<string, UIComponent> _childComponents;

        // ���
        public RectTransform CacheRecTrans { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            CacheRecTrans = GetComponent<RectTransform>();
            ReadChildComponents();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// ��ȡ���������
        /// </summary>
        private void ReadChildComponents()
        {
            UIComponent[] componentArray = GetComponentsInChildren<UIComponent>(true);
            if (componentArray != null)
            {
                _childComponents = new Dictionary<string, UIComponent>(componentArray.Length);
                foreach (var ui in componentArray)
                {
                    if (ui == null)
                    {
                        continue;
                    }
                    string name = ui.name;
                    if (_childComponents.ContainsKey(name))
                    {
                        GameLog.Warning($"UI�Զ������{name}�д����������{name}��");
                        continue;
                    }
                    _childComponents[ui.gameObject.name] = ui;
                }
            }
            else
            {
                _childComponents = new Dictionary<string, UIComponent>();
            }
        }
    }
}

