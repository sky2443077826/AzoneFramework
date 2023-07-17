using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

namespace AzoneFramework.UI
{
    /// <summary>
    /// 自定义UI组件：循环滚动列表
    /// </summary>
    public class AZScrollList : UIComponent
    {
        // 滚动组件
        private ScrollRect _scrollRect;
        // 内容
        private RectTransform _content;
        // 格子布局组件
        private GridLayoutGroup _gridLayoutGroup;
        // 容器组件
        private RectTransform _viewPort;

        // 内容区域完美宽度
        private float _perfectedContentWidth;
        // 内容区域高度宽度
        private float _perfectedContentHeight;
        // 原始Padding
        private RectOffset _originPadding;

        // 子对象（实例化出来的）
        private List<AZScrollListChild> _childs;
        
        [Header("子对象模板")]
        [SerializeField]
        private AZScrollListChild _childModel;

        [Header("元素数量(逻辑上的)")]
        [SerializeField]
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

                RecalculateContentSize();
                RefreshList();
            }
        }

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

            _content = _scrollRect?.content;
            if (_content == null)
            {
                GameLog.Error("循环滚动列表创建失败！---> 未找到滚动组件ScrollRect的Content。");
                return;
            }

            _gridLayoutGroup = _content?.GetComponent<GridLayoutGroup>();
            if (_gridLayoutGroup == null)
            {
                GameLog.Error("循环滚动列表创建失败！---> 未找到滚动组件ScrollRect的Content的GridLayoutGroup组件。");
                return;
            }

            _viewPort = _scrollRect?.viewport;
            if (_viewPort == null)
            {
                GameLog.Error("循环滚动列表创建失败！---> 未找到滚动组件ScrollRect的ViewPort");
                return;
            }

            if (_childModel == null)
            {
                GameLog.Error("循环滚动列表创建失败！---> 未找到元素模板。");
                return;
            }
            _childModel.gameObject.SetActive(false);

            _childs = new List<AZScrollListChild>();

            _originPadding = new RectOffset(_gridLayoutGroup.padding.left, _gridLayoutGroup.padding.right, _gridLayoutGroup.padding.top, _gridLayoutGroup.padding.bottom);

            RecalculateContentSize();
            
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

        // 重新计算内容尺寸
        private void RecalculateContentSize()
        {
            // 设置内容组件的锚点
            //UIUtility.SetRecTransformAnchor(_content, _gridLayoutGroup.childAlignment);

            switch (_gridLayoutGroup.startAxis)
            {
                case Axis.Horizontal:

                    // 先计算宽度
                    int preferredColumns = 0;
                    if (_gridLayoutGroup.constraint == Constraint.FixedColumnCount)
                    {
                        preferredColumns = _gridLayoutGroup.constraintCount;
                    }
                    else if (_gridLayoutGroup.constraint == Constraint.FixedRowCount)
                    {
                        preferredColumns = Mathf.CeilToInt(_count / (float)_gridLayoutGroup.constraintCount - 0.001f);
                    }
                    else
                    {
                        preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(_count));
                    }
                    _perfectedContentWidth = _originPadding.horizontal + (_gridLayoutGroup.cellSize.x + _gridLayoutGroup.spacing.x) * preferredColumns - _gridLayoutGroup.spacing.x;

                    // 计算高度
                    int preferredRows = Mathf.CeilToInt(_count / (float)preferredColumns - 0.001f);
                    _perfectedContentHeight = _originPadding.vertical + (_gridLayoutGroup.cellSize.y + _gridLayoutGroup.spacing.y) * preferredRows - _gridLayoutGroup.spacing.y;

                    break;

                case Axis.Vertical:

                    // 先计算高度
                    preferredRows = 0;
                    if (_gridLayoutGroup.constraint == Constraint.FixedColumnCount)
                    {
                        preferredRows = Mathf.CeilToInt(_count / (float)_gridLayoutGroup.constraintCount - 0.001f);
                    }
                    else if (_gridLayoutGroup.constraint == Constraint.FixedRowCount)
                    {
                        preferredRows = _gridLayoutGroup.constraintCount;
                    }
                    else
                    {
                        preferredRows = Mathf.CeilToInt(Mathf.Sqrt(_count));
                    }
                    _perfectedContentHeight = _originPadding.vertical + (_gridLayoutGroup.cellSize.y + _gridLayoutGroup.spacing.y) * preferredRows - _gridLayoutGroup.spacing.y;

                    // 计算宽度
                    preferredColumns = Mathf.CeilToInt(_count / (float)preferredRows - 0.001f);
                    _perfectedContentWidth = _originPadding.horizontal + (_gridLayoutGroup.cellSize.x + _gridLayoutGroup.spacing.x) * preferredColumns - _gridLayoutGroup.spacing.x;

                    break;
            }

            // 设置
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _perfectedContentWidth);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _perfectedContentHeight);
        }

        // 创建子对象
        private AZScrollListChild CreateChild()
        {
            AZScrollListChild newItem = GameObject.Instantiate(_childModel.gameObject).GetComponent<AZScrollListChild>();
            if (newItem == null)
            {
                return null;
            }
            // 初始化
            newItem.Init(_childs.Count - 1, this);
            _childs.Add(newItem);

            return newItem;
        }
    }
}


