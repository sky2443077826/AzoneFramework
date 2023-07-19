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
    /// 滚动列表渲染函数
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <param name="child"></param>
    public delegate void ListItemRender(int itemIndex, AZScrollListChild child);

    /// <summary>
    /// 点击滚动列表元素
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <param name="child"></param>
    public delegate void ClickListItem(int itemIndex, AZScrollListChild child);

    /// <summary>
    /// 自定义UI组件：循环滚动列表
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class AZScrollList : MonoBehaviour
    {
        // 轴向
        public enum Axis
        {
            // 水平方向
            Horizontal = 0,
            // 垂直方向
            Vertical = 1,
        }

        // 滚动组件
        private ScrollRect _scrollRect;
        // 内容
        private RectTransform _content;
        // UI
        private UIEvent uiEevent;

        // 内容区域完美宽度
        private float _perfectedContentWidth;
        // 内容区域高度宽度
        private float _perfectedContentHeight;

        // 内容区域高度行数
        private int _perfectedContentRow;
        // 内容区域完美列数
        private int _perfectedContentColumn;

        // 展示行数
        private int _showRow;
        // 展示列数
        private int _showColumn;

        // 展示开始行
        private int _showStartRow;
        // 展示开始列
        private int _showStartColumn;

        [Header("边距")]
        [SerializeField] protected RectOffset _padding = new RectOffset();

        [Header("元素流向")]
        [SerializeField] private Axis _startAxis;

        [Header("行数")]
        [SerializeField] private int _rowCount;

        [Header("列数")]
        [SerializeField] private int _columnCount;

        [Header("间距")]
        [SerializeField] private Vector2 _spacing;

        [Header("子对象大小")]
        [SerializeField] private Vector2 _cellSize;

        [Header("子对象模板")]
        [SerializeField] private AZScrollListChild _childModel;

        [Header("元素数量(逻辑上的)")]
        [SerializeField] private int _count;
        /// <summary>
        /// 元素数量
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


        // 真实行高
        private float _RowHeight => _spacing.y + _cellSize.y;
        // 真实列宽
        private float _ColumnWidth => _spacing.x + _cellSize.x;

        // 是否达到列尾
        public bool AtFinalColumn => _showStartColumn + _showColumn - 1 >= _perfectedContentColumn;

        // 是否达到行尾
        public bool AtFinalRow => _showStartRow + _showRow - 1 >= _perfectedContentRow;

        // 是否触发滚动变化
        private bool _scrollChangeEnable = true;


        // 子对象实体列表
        private List<AZScrollListChild> _childs;
        private int[] _itemIndexToChild;
        private int[] _childIndexToItem;

        /// <summary>
        /// 矩形变换组件
        /// </summary>
        public RectTransform CahceRecTrans { get; private set; }

        /// <summary>
        /// 子对象更新事件
        /// </summary>
        public ListItemRender itemRender;

        /// <summary>
        /// 点击子对象
        /// </summary>
        public ClickListItem onClickListItem;

        private void Awake()
        {
            CahceRecTrans = GetComponent<RectTransform>();
            uiEevent = UIEvent.GetEvent(gameObject);

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

            if (_childModel == null)
            {
                GameLog.Error("循环滚动列表创建失败！---> 未找到元素模板。");
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

        // 根据视图大小生成子对象
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

        // 重新计算合适的子对象容量
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

        // 重新计算内容尺寸
        private void RecalculateContentSize()
        {
            // 设置内容组件的锚点
            //UIUtility.SetRecTransformAnchor(_content, _gridLayoutGroup.childAlignment);

            switch (_startAxis)
            {
                case Axis.Horizontal:

                    // 先计算宽度
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
                    // 计算高度
                    _perfectedContentRow = Mathf.CeilToInt(_count / (float)_perfectedContentColumn - 0.001f);
                    _perfectedContentHeight = _padding.vertical + (_cellSize.y + _spacing.y) * _perfectedContentRow - _spacing.y;

                    break;

                case Axis.Vertical:

                    // 先计算高度
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
                    // 计算宽度
                    _perfectedContentColumn = Mathf.CeilToInt(_count / (float)_perfectedContentRow - 0.001f);
                    _perfectedContentWidth = _padding.horizontal + (_cellSize.x + _spacing.x) * _perfectedContentColumn - _spacing.x;

                    break;
            }

            // 设置
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _perfectedContentWidth);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _perfectedContentHeight);
        }

        // 重新计算列表内容
        private void RecalculateContent(bool isForceChange = false)
        {
            // 计算竖直变化
            if (_scrollRect.vertical)
            {
                int curShowFirstRow = (int)((_content.localPosition.y - _padding.top) / _RowHeight);
                isForceChange |= curShowFirstRow != _showStartRow;
                _showStartRow = curShowFirstRow;
            }

            // 计算水平变化
            if (_scrollRect.horizontal)
            {
                // 判断是否滚动距离差超过一列
                int curShowFirsColumn = (int)((-_content.localPosition.x - _padding.left) / _ColumnWidth);
                isForceChange |= curShowFirsColumn != _showStartColumn;
                _showStartColumn = curShowFirsColumn;
            }

            UpdateChild(isForceChange);
        }

        // 获取元素位置
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

        // 更新子对象
        private void UpdateChild(bool change = false)
        {
            foreach (var child in _childs)
            {
                if (child == null)
                {
                    continue;
                }

                // 获取转换的数据索引
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
                    // 获取元素真实位置
                    if (!GetItemPos(itemIndex, out Vector3 itemPos))
                    {
                        GameLog.Error($"滚动列表更新子对象失败！---> 对象【{gameObject.name}】无法获取元素【{itemIndex}】的位置。");
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

        // 创建子对象
        private AZScrollListChild CreateChild()
        {
            AZScrollListChild newItem = GameObject.Instantiate(_childModel.gameObject, _content).GetComponent<AZScrollListChild>();
            if (newItem == null)
            {
                return null;
            }
            // 初始化
            newItem.Init(_childs.Count, this);
            _childs.Add(newItem);

            return newItem;
        }

        // 销毁子对象
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

        // 给子对象设置元素索引
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
        /// 滚动时
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
        /// 当子对象被点击时
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
        /// 刷新列表(外部调用)
        /// 按照当前元素数量和布局，重新计算一次列表
        /// </summary>
        public void RefreshList()
        {
            RecalculateContent();
        }

        /// <summary>
        /// 获取数据索引
        /// </summary>
        /// <param name="childIndex"></param>
        /// <returns>-1 为不存在</returns>
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
        /// 获取数据索引
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>-1 为不存在</returns>
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

        #region 测试函数

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


