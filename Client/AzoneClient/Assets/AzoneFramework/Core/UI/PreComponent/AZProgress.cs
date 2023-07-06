using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AzoneFramework
{
    /// <summary>
    /// UI组件-进度条
    /// </summary>
    public class AZProgress : UIComponent, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    { 
        /*
         * 输入事件
         */
        public event PointerEvent onClick;
        public event PointerEvent onPointerEnter;
        public event PointerEvent onPointerExit;

        [Header("外框组件")]
        public Image imgFrame;

        [Header("进度条区域")]
        public RectTransform barArea;

        [Header("进度条组件")]
        public Image imgBar;

        [Header("外框颜色")]
        [SerializeField]
        private Color _frameColor = Color.white;
        public Color FrameColor
        {
            get => _frameColor;
            set
            {
                SetFrameColor(value);
            }
        }

        [Header("进度条颜色")]
        [SerializeField]
        private Color _barColor = Color.blue;
        public Color BarColor
        {
            get => _barColor;
            set
            {
                SetBarColor(value);
            }
        }     
        
        [Header("最大值")]
        [SerializeField]
        private float _max = 100;
        public float Max
        {
            get => _max;
            set
            {
                _max = value;
                RefreshBar();
            }
        }

        [Header("当前值")]
        [SerializeField]
        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                RefreshBar();
            }
        }

        [Header("是否为竖直进度条")]
        [SerializeField]
        private bool _isVertical;

        /// <summary>
        /// 进度条精灵图
        /// </summary>
        public Sprite BarSprite
        {
            get
            {
                return imgBar?.sprite;
            }

            set
            {
                if (imgBar !=null)
                {
                    imgBar.sprite = value;
                }
            }
        }

        /// <summary>
        /// 外框精灵图
        /// </summary>
        public Sprite FrameSprite
        {
            get
            {
                return imgFrame?.sprite;
            }

            set
            {
                if (imgFrame != null)
                {
                    imgFrame.sprite = value;
                }
            }
        }

        /// <summary>
        /// 进度条水平长度
        /// </summary>
        public float ProGressWidth 
        {
            get => CacheRecTrans.rect.width;

            set
            {
                CacheRecTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
            }
        }

        /// <summary>
        /// 进度条竖直长度
        /// </summary>
        public float ProGressHeight
        {
            get => CacheRecTrans.rect.height;

            set
            {
                CacheRecTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            RefreshProgress();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// 刷新进度条
        /// </summary>
        private void RefreshProgress()
        {
            SetBarColor(_barColor);
            SetFrameColor(_frameColor);
            SetDirection(_isVertical);
        }

        /// <summary>
        /// 刷新进度条长度显示
        /// </summary>
        private void RefreshBar()
        {
            if (imgBar == null || barArea == null)
            {
                return;
            }

            _max = _max < 0 ? 0 : _max;
            _value = _value > _max ? _max : _value < 0 ? 0 : _value;

            if (_isVertical)
            {
                imgBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, barArea.rect.height * (_value / _max));
                imgBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barArea.rect.width);
            }
            else
            {
                imgBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barArea.rect.width * (_value / _max));
                imgBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, barArea.rect.height);
            }
        }

        /// <summary>
        /// 设置进度条方向
        /// </summary>
        private void SetDirection(bool isVertical)
        {
            _isVertical = isVertical;
            RefreshBar();
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetBarColor(Color color)
        {
            if (imgBar == null)
            {
                return;
            }

            imgBar.color = _barColor = color;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetFrameColor(Color color)
        {
            if (imgFrame == null)
            {
                return;
            }

            imgFrame.color = _frameColor = color;
        }




#if UNITY_EDITOR
        private void OnValidate()
        {
            RefreshProgress();
        }
#endif

        #region UI输入事件

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(eventData, this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke(eventData, this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke(eventData, this);
        }

        #endregion



    }
}


