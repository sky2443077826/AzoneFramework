using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using AzoneFramework;

namespace AzoneFramework.UI
{
    /* UI输入委托 */
    public delegate void UIBaseEvent(UIEvent sender, BaseEventData data);
    public delegate void UIPointerEvent(UIEvent sender, PointerEventData data);
    public delegate void UIAxisEvent(UIEvent sender, AxisEventData data);

    /// <summary>
    /// UI事件触发类
    /// </summary>
    public class UIEvent : EventTrigger
    {
        /*
         * UI事件 
         */
        public event UIPointerEvent onBeginDrag = null;
        public event UIBaseEvent onCancel = null;
        public event UIBaseEvent onDeselect = null;
        public event UIPointerEvent onDrag = null;
        public event UIPointerEvent onDrop = null;
        public event UIPointerEvent onEndDrag = null;
        public event UIPointerEvent onInitializePotentialDrag = null;
        public event UIAxisEvent onMove = null;
        public event UIPointerEvent onClick = null;
        public event UIPointerEvent onDown = null;
        public event UIPointerEvent onEnter = null;
        public event UIPointerEvent onExit = null;
        public event UIPointerEvent onUp = null;
        public event UIPointerEvent onScroll = null;
        public event UIBaseEvent onSelect = null;
        public event UIBaseEvent onUpdateSelect = null;
        public event UIBaseEvent onSubmit = null;

        /// <summary>
        /// 获取UI事件触发器（没有则添加一个）
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static UIEvent GetEvent(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            UIEvent uiEvent = gameObject.GetOrAddComponent<UIEvent>();
            return uiEvent;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke(this, eventData);
        }

        public override void OnCancel(BaseEventData eventData)
        {
            onCancel?.Invoke(this, eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            onDeselect?.Invoke(this, eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(this, eventData);
        }

        public override void OnDrop(PointerEventData eventData)
        {
            onDrop?.Invoke(this, eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(this, eventData);
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            onInitializePotentialDrag?.Invoke(this, eventData);
        }

        public override void OnMove(AxisEventData eventData)
        {
            onMove?.Invoke(this, eventData);

        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(this, eventData);

        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            onDown?.Invoke(this, eventData);

        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            onEnter?.Invoke(this, eventData);

        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            onExit?.Invoke(this, eventData);

        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            onUp?.Invoke(this, eventData);

        }

        public override void OnScroll(PointerEventData eventData)
        {
            onScroll?.Invoke(this, eventData);

        }

        public override void OnSelect(BaseEventData eventData)
        {
            onSelect?.Invoke(this, eventData);

        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            onUpdateSelect?.Invoke(this, eventData);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            onSubmit?.Invoke(this, eventData);
        }
    }
}


