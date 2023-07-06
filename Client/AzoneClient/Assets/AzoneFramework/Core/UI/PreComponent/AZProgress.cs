using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AzoneFramework
{
    /// <summary>
    /// UI���-������
    /// </summary>
    public class AZProgress : UIComponent, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    { 
        /*
         * �����¼�
         */
        public event PointerEvent onClick;
        public event PointerEvent onPointerEnter;
        public event PointerEvent onPointerExit;

        [Header("������")]
        public Image imgFrame;

        [Header("����������")]
        public RectTransform barArea;

        [Header("���������")]
        public Image imgBar;

        [Header("�����ɫ")]
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

        [Header("��������ɫ")]
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
        
        [Header("���ֵ")]
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

        [Header("��ǰֵ")]
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

        [Header("�Ƿ�Ϊ��ֱ������")]
        [SerializeField]
        private bool _isVertical;

        /// <summary>
        /// ����������ͼ
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
        /// �����ͼ
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
        /// ������ˮƽ����
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
        /// ��������ֱ����
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
        /// ˢ�½�����
        /// </summary>
        private void RefreshProgress()
        {
            SetBarColor(_barColor);
            SetFrameColor(_frameColor);
            SetDirection(_isVertical);
        }

        /// <summary>
        /// ˢ�½�����������ʾ
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
        /// ���ý���������
        /// </summary>
        private void SetDirection(bool isVertical)
        {
            _isVertical = isVertical;
            RefreshBar();
        }

        /// <summary>
        /// ������ɫ
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
        /// ������ɫ
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

        #region UI�����¼�

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


