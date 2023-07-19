using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

namespace AzoneFramework.UI
{
    /// <summary>
    /// �����б���Ⱦ����
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <param name="child"></param>
    public delegate void ListItemRender(int itemIndex, AZScrollListChild child);

    /// <summary>
    /// ��������б�Ԫ��
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <param name="child"></param>
    public delegate void ClickListItem(int itemIndex, AZScrollListChild child);

    /// <summary>
    /// �Զ���UI�����ѭ�������б�
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class AZScrollList : MonoBehaviour
    {
        // ����
        public enum Axis
        {
            // ˮƽ����
            Horizontal = 0,
            // ��ֱ����
            Vertical = 1,
        }

        // �������
        private ScrollRect _scrollRect;
        // ����
        private RectTransform _content;
        // UI
        private UIEvent uiEevent;

        // ���������������
        private float _perfectedContentWidth;
        // ��������߶ȿ��
        private float _perfectedContentHeight;

        // ��������߶�����
        private int _perfectedContentRow;
        // ����������������
        private int _perfectedContentColumn;

        // չʾ����
        private int _showRow;
        // չʾ����
        private int _showColumn;

        // չʾ��ʼ��
        private int _showStartRow;
        // չʾ��ʼ��
        private int _showStartColumn;

        [Header("�߾�")]
        [SerializeField] protected RectOffset _padding = new RectOffset();

        [Header("Ԫ������")]
        [SerializeField] private Axis _startAxis;

        [Header("����")]
        [SerializeField] private int _rowCount;

        [Header("����")]
        [SerializeField] private int _columnCount;

        [Header("���")]
        [SerializeField] private Vector2 _spacing;

        [Header("�Ӷ����С")]
        [SerializeField] private Vector2 _cellSize;

        [Header("�Ӷ���ģ��")]
        [SerializeField] private AZScrollListChild _childModel;

        [Header("Ԫ������(�߼��ϵ�)")]
        [SerializeField] private int _count;
        /// <summary>
        /// Ԫ������
        /// </summary>
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
                Array.Resize(ref _itemIndexToChild, _count);
                RecalculateContentSize();
                GenerateChildByViewPort();
                RecalculateContent();
            }
        }


        // ��ʵ�и�
        private float _RowHeight => _spacing.y + _cellSize.y;
        // ��ʵ�п�
        private float _ColumnWidth => _spacing.x + _cellSize.x;

        // �Ƿ�ﵽ��β
        public bool AtFinalColumn => _showStartColumn + _showColumn - 1 >= _perfectedContentColumn;

        // �Ƿ�ﵽ��β
        public bool AtFinalRow => _showStartRow + _showRow - 1 >= _perfectedContentRow;

        // �Ƿ񴥷������仯
        private bool _scrollChangeEnable = true;


        // �Ӷ���ʵ���б�
        private List<AZScrollListChild> _childs;
        private int[] _itemIndexToChild;
        private int[] _childIndexToItem;

        /// <summary>
        /// ���α任���
        /// </summary>
        public RectTransform CahceRecTrans { get; private set; }

        /// <summary>
        /// �Ӷ�������¼�
        /// </summary>
        public ListItemRender itemRender;

        /// <summary>
        /// ����Ӷ���
        /// </summary>
        public ClickListItem onClickListItem;

        private void Awake()
        {
            CahceRecTrans = GetComponent<RectTransform>();
            uiEevent = UIEvent.GetEvent(gameObject);

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

            if (_childModel == null)
            {
                GameLog.Error("ѭ�������б���ʧ�ܣ�---> δ�ҵ�Ԫ��ģ�塣");
                return;
            }
            _childModel.gameObject.SetActive(false);

            _childs = new List<AZScrollListChild>();
            _itemIndexToChild = new int[_count];
            _childIndexToItem = new int[0];

            _scrollRect.onValueChanged.AddListener(OnValueChange);

            itemRender = OnUpdateItem;
            uiEevent.onClick += OnClickItemCallBack;

            RecalculateContentSize();
            GenerateChildByViewPort();
            RecalculateContent(true);
        }

        private void OnDestroy()
        {
            _childs.Clear();
            itemRender = null;
            uiEevent.onClick -= OnClickItemCallBack;
        }

        // ������ͼ��С�����Ӷ���
        private void GenerateChildByViewPort()
        {
            int needCount = RecalculateChildCount();
            if (Count < needCount)
            {
                needCount = Count;
            }
            Array.Resize(ref _childIndexToItem, needCount);

            while (_childs.Count != needCount)
            {
                if (_childs.Count > needCount)
                {
                    DestroyChild();
                }
                else
                {
                    CreateChild();
                }
            }
        }

        // ���¼�����ʵ��Ӷ�������
        private int RecalculateChildCount()
        {
            _showRow = (int)Mathf.Ceil(CahceRecTrans.rect.height / _RowHeight) + 1;
            _showColumn = (int)Mathf.Ceil(CahceRecTrans.rect.width / _ColumnWidth) + 1;

            if (!(_showRow > 1 || _showColumn > 1))
            {
                return 0;
            }

            return _showRow * _showColumn;
        }

        // ���¼������ݳߴ�
        private void RecalculateContentSize()
        {
            // �������������ê��
            //UIUtility.SetRecTransformAnchor(_content, _gridLayoutGroup.childAlignment);

            switch (_startAxis)
            {
                case Axis.Horizontal:

                    // �ȼ�����
                    if (_columnCount > 0)
                    {
                        _perfectedContentColumn = _columnCount;
                        _perfectedContentWidth = _padding.horizontal + _ColumnWidth * _perfectedContentColumn - _spacing.x;
                    }
                    else
                    {
                        _perfectedContentWidth = _content.rect.width;
                        _perfectedContentColumn = (int)((_perfectedContentWidth - _padding.horizontal + _spacing.x) / _ColumnWidth);
                    }
                    // ����߶�
                    _perfectedContentRow = Mathf.CeilToInt(_count / (float)_perfectedContentColumn - 0.001f);
                    _perfectedContentHeight = _padding.vertical + (_cellSize.y + _spacing.y) * _perfectedContentRow - _spacing.y;

                    break;

                case Axis.Vertical:

                    // �ȼ���߶�
                    if (_rowCount > 0)
                    {
                        _perfectedContentRow = _rowCount;
                        _perfectedContentHeight = _padding.vertical + _RowHeight * _perfectedContentRow - _spacing.y;
                    }
                    else
                    {
                        _perfectedContentHeight = _content.rect.height;
                        _perfectedContentRow = (int)((_perfectedContentHeight - _padding.vertical + _spacing.y) / _RowHeight);
                    }
                    // ������
                    _perfectedContentColumn = Mathf.CeilToInt(_count / (float)_perfectedContentRow - 0.001f);
                    _perfectedContentWidth = _padding.horizontal + (_cellSize.x + _spacing.x) * _perfectedContentColumn - _spacing.x;

                    break;
            }

            // ����
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _perfectedContentWidth);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _perfectedContentHeight);
        }

        // ���¼����б�����
        private void RecalculateContent(bool isForceChange = false)
        {
            // ������ֱ�仯
            if (_scrollRect.vertical)
            {
                int curShowFirstRow = (int)((_content.localPosition.y - _padding.top) / _RowHeight);
                isForceChange |= curShowFirstRow != _showStartRow;
                _showStartRow = curShowFirstRow;
            }

            // ����ˮƽ�仯
            if (_scrollRect.horizontal)
            {
                // �ж��Ƿ����������һ��
                int curShowFirsColumn = (int)((-_content.localPosition.x - _padding.left) / _ColumnWidth);
                isForceChange |= curShowFirsColumn != _showStartColumn;
                _showStartColumn = curShowFirsColumn;
            }

            UpdateChild(isForceChange);
        }

        // ��ȡԪ��λ��
        private bool GetItemPos(int itemIndex, out Vector3 pos)
        {
            pos = UIManager.HIDE_POS;
            if (itemIndex < 0 || itemIndex >= _count)
            {
                return false;
            }

            int itemRow = 0;
            int itemColumn = 0;

            switch (_startAxis)
            {
                case Axis.Horizontal:
                    itemRow = itemIndex / _perfectedContentColumn;
                    itemColumn = itemIndex % _perfectedContentColumn;
                    break;

                case Axis.Vertical:
                    itemRow = itemIndex % _perfectedContentRow;
                    itemColumn = itemIndex / _perfectedContentRow;
                    break;
            }

            float localPosY = -_padding.top - itemRow * _RowHeight;
            float localPosX = _padding.left + itemColumn * _ColumnWidth;
            pos = new Vector3(localPosX, localPosY);
            return true;
        }

        // �����Ӷ���
        private void UpdateChild(bool change = false)
        {
            foreach (var child in _childs)
            {
                if (child == null)
                {
                    continue;
                }

                // ��ȡת������������
                int childIndex = child.Index;
                int itemIndex = GetItemIndexByChild(childIndex);
                if (itemIndex < 0 || itemIndex > _count - 1)
                {
                    child.CacheRecTrans.localPosition = UIManager.HIDE_POS;
                    SetChildItemIndex(child, -1);
                    continue;
                }

                if (change)
                {
                    // ��ȡԪ����ʵλ��
                    if (!GetItemPos(itemIndex, out Vector3 itemPos))
                    {
                        GameLog.Error($"�����б�����Ӷ���ʧ�ܣ�---> ����{gameObject.name}���޷���ȡԪ�ء�{itemIndex}����λ�á�");
                        continue;
                    }
                    Vector3 childLocalPos = itemPos;
                    childLocalPos.x = itemPos.x + child.CacheRecTrans.rect.width * (child.CacheRecTrans.pivot.x - 0);
                    childLocalPos.y = itemPos.y + child.CacheRecTrans.rect.height * (child.CacheRecTrans.pivot.y - 1);
                    child.CacheRecTrans.localPosition = childLocalPos;
                    SetChildItemIndex(child, itemIndex);
                    itemRender?.Invoke(itemIndex, child);
                }
            }
        }

        // �����Ӷ���
        private AZScrollListChild CreateChild()
        {
            AZScrollListChild newItem = GameObject.Instantiate(_childModel.gameObject, _content).GetComponent<AZScrollListChild>();
            if (newItem == null)
            {
                return null;
            }
            // ��ʼ��
            newItem.Init(_childs.Count, this);
            _childs.Add(newItem);

            return newItem;
        }

        // �����Ӷ���
        private void DestroyChild()
        {
            if (_childs.Count <= 0)
            {
                return;
            }

            AZScrollListChild child = _childs[_childs.Count - 1];
            _childs.RemoveAt(_childs.Count - 1);
            GameObject.Destroy(child);
        }

        // ���Ӷ�������Ԫ������
        private void SetChildItemIndex(AZScrollListChild child, int itemIndex)
        {
            if (child == null || itemIndex > _count - 1)
            {
                return;
            }

            child.ItemIndex = itemIndex;
            _childIndexToItem[child.Index] = itemIndex;
            if (itemIndex >= 0)
            {
                _itemIndexToChild[itemIndex] = child.Index;
            }
        }

        /// <summary>
        /// ����ʱ
        /// </summary>
        /// <param name="arg0"></param>
        private void OnValueChange(Vector2 arg0)
        {
            if (!_scrollChangeEnable)
            {
                return;
            }

            RecalculateContent();
        }

        /// <summary>
        /// ���Ӷ��󱻵��ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void OnClickItemCallBack(UIEvent sender, PointerEventData data)
        {
            if (data.dragging)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, data.position, UIManager.Instance.UICamera, out Vector2 localPoint))
            {
                return;
            }

            int itemIndex = GetItemIndexByLocalPos(localPoint);
            if (itemIndex < 0 || itemIndex > _count - 1)
            {
                return;
            }

            int childIndex = _itemIndexToChild[itemIndex];
            AZScrollListChild child = _childs[childIndex];
            if (child == null)
            {
                return;
            }

            onClickListItem?.Invoke(itemIndex, child);
        }

        /// <summary>
        /// ˢ���б�(�ⲿ����)
        /// ���յ�ǰԪ�������Ͳ��֣����¼���һ���б�
        /// </summary>
        public void RefreshList()
        {
            RecalculateContent();
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="childIndex"></param>
        /// <returns>-1 Ϊ������</returns>
        public int GetItemIndexByChild(int childIndex)
        {
            if (childIndex < 0 || childIndex > _childs.Count - 1)
            {
                return -1;
            }

            AZScrollListChild child = _childs[childIndex];
            if (child == null)
            {
                return -1;
            }

            int itemIndex = -1;
            int curRow = 0;
            int curColumn = 0;

            switch (_startAxis)
            {
                case Axis.Horizontal:
                    int curShowColumns = AtFinalColumn ? _showColumn - 1 : _showColumn;
                    curRow = _showStartRow + childIndex / curShowColumns;
                    curColumn = _showStartColumn + childIndex % curShowColumns;
                    itemIndex = curRow * _perfectedContentColumn + curColumn;
                    break;

                case Axis.Vertical:
                    int curShowRows = AtFinalRow ? _showRow - 1 : _showRow;
                    curRow = _showStartRow + childIndex % curShowRows;
                    curColumn = _showStartColumn + childIndex / curShowRows;
                    itemIndex = curRow + curColumn * _perfectedContentRow;
                    break;
            }

            return itemIndex;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>-1 Ϊ������</returns>
        public int GetItemIndexByLocalPos(Vector2 pos)
        {
            int itemIndex = -1;

            if (pos.x < 0 || pos.y > 0 || pos.x > _content.rect.width || pos.y < -_content.rect.height)
            {
                return itemIndex;
            }

            int row = (int)((-pos.y - _padding.top) / _RowHeight);
            int column = (int)((pos.x - _padding.left) / _ColumnWidth);
            switch (_startAxis)
            {
                case Axis.Horizontal:
                    itemIndex = row * _perfectedContentColumn + column;
                    break;
                case Axis.Vertical:
                    itemIndex = row + column * _perfectedContentRow;
                    break;
            }

            return itemIndex;
        }

        #region ���Ժ���

        private void OnUpdateItem(int itemIndex, AZScrollListChild child)
        {
            int childIndex = child.Index;
            TextMeshProUGUI text = child.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = childIndex.ToString();
            }
            Image image = child.GetComponent<Image>();
            if (image != null)
            {
                float rgb = itemIndex / 255f;
                image.color = new Color(1, rgb, rgb);
            }
        }

        #endregion


    }
}


