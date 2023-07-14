using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AzoneFramework.UI
{
    /// <summary>
    /// 自定义UI组件：循环滚动列表
    /// </summary>
    public class AZScrollList : UIComponent
    {
        // 滚动组件
        private ScrollRect _scrollRect;

        // 元素列表（实例化出来的）
        private List<AZScrollListItem> _itemInstances;

        /// <summary>
        /// 元素数量(逻辑上的)
        /// </summary>
        private int _count;
        public int Count 
        {
            get => _count;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                _count = value;
            }
        }

        public int leftPadding;
        public int rightPadding;
        public int topPadding;
        public int downPadding;

        protected override void OnCreate()
        {
            base.OnCreate();

            // 加载滚动组件
            _scrollRect = GetComponent<ScrollRect>();
            if (_scrollRect == null)
            {
                GameLog.Error("循环滚动列表创建失败！---> 未找到滚动组件ScrollRect。");
                return;
            }

            _itemInstances = new List<AZScrollListItem>();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshList()
        {
            
        }
    }
}


