using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AzoneFramework.UI
{
    /// <summary>
    /// UI自定义组件基类
    /// </summary>
    public class UIComponent : UIBase
    {
        /// <summary>
        /// 子组件
        /// </summary>
        protected Dictionary<string, UIComponent> _childComponents;

        // 组件
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
        /// 读取所有子组件
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
                        GameLog.Warning($"UI自定义组件{name}中存在重名组件{name}。");
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

