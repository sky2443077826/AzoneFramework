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
    /// �Զ���UI�����ѭ�������б�
    /// </summary>
    public class AZScrollList : UIComponent
    {
        // �������
        private ScrollRect _scrollRect;
        // ����
        private RectTransform _content;
        // ���Ӳ������
        private GridLayoutGroup _gridLayoutGroup;
        // �������
        private RectTransform _viewPort;

        // ���������������
        private float _perfectedContentWidth;
        // ��������߶ȿ��
        private float _perfectedContentHeight;
        // ԭʼPadding
        private RectOffset _originPadding;

        // �Ӷ���ʵ���������ģ�
        private List<AZScrollListChild> _childs;
        
        [Header("�Ӷ���ģ��")]
        [SerializeField]
        private AZScrollListChild _childModel;

        [Header("Ԫ������(�߼��ϵ�)")]
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

            // ���ع������
            _scrollRect = GetComponent<ScrollRect>();
            if (_scrollRect == null)
            {
                GameLog.Error("ѭ�������б���ʧ�ܣ�---> δ�ҵ��������ScrollRect��");
                return;
            }

            _content = _scrollRect?.content;
            if (_content == null)
            {
                GameLog.Error("ѭ�������б���ʧ�ܣ�---> δ�ҵ��������ScrollRect��Content��");
                return;
            }

            _gridLayoutGroup = _content?.GetComponent<GridLayoutGroup>();
            if (_gridLayoutGroup == null)
            {
                GameLog.Error("ѭ�������б���ʧ�ܣ�---> δ�ҵ��������ScrollRect��Content��GridLayoutGroup�����");
                return;
            }

            _viewPort = _scrollRect?.viewport;
            if (_viewPort == null)
            {
                GameLog.Error("ѭ�������б���ʧ�ܣ�---> δ�ҵ��������ScrollRect��ViewPort");
                return;
            }

            if (_childModel == null)
            {
                GameLog.Error("ѭ�������б���ʧ�ܣ�---> δ�ҵ�Ԫ��ģ�塣");
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
        /// ˢ���б�
        /// </summary>
        public void RefreshList()
        {
        }

        // ���¼������ݳߴ�
        private void RecalculateContentSize()
        {
            // �������������ê��
            //UIUtility.SetRecTransformAnchor(_content, _gridLayoutGroup.childAlignment);

            switch (_gridLayoutGroup.startAxis)
            {
                case Axis.Horizontal:

                    // �ȼ�����
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

                    // ����߶�
                    int preferredRows = Mathf.CeilToInt(_count / (float)preferredColumns - 0.001f);
                    _perfectedContentHeight = _originPadding.vertical + (_gridLayoutGroup.cellSize.y + _gridLayoutGroup.spacing.y) * preferredRows - _gridLayoutGroup.spacing.y;

                    break;

                case Axis.Vertical:

                    // �ȼ���߶�
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

                    // ������
                    preferredColumns = Mathf.CeilToInt(_count / (float)preferredRows - 0.001f);
                    _perfectedContentWidth = _originPadding.horizontal + (_gridLayoutGroup.cellSize.x + _gridLayoutGroup.spacing.x) * preferredColumns - _gridLayoutGroup.spacing.x;

                    break;
            }

            // ����
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _perfectedContentWidth);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _perfectedContentHeight);
        }

        // �����Ӷ���
        private AZScrollListChild CreateChild()
        {
            AZScrollListChild newItem = GameObject.Instantiate(_childModel.gameObject).GetComponent<AZScrollListChild>();
            if (newItem == null)
            {
                return null;
            }
            // ��ʼ��
            newItem.Init(_childs.Count - 1, this);
            _childs.Add(newItem);

            return newItem;
        }
    }
}


