using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AzoneFramework.UI
{
    /// <summary>
    /// �Զ���UI�����ѭ�������б�
    /// </summary>
    public class AZScrollList : UIComponent
    {
        // �������
        private ScrollRect _scrollRect;

        // Ԫ���б�ʵ���������ģ�
        private List<AZScrollListItem> _itemInstances;

        /// <summary>
        /// Ԫ������(�߼��ϵ�)
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

            // ���ع������
            _scrollRect = GetComponent<ScrollRect>();
            if (_scrollRect == null)
            {
                GameLog.Error("ѭ�������б���ʧ�ܣ�---> δ�ҵ��������ScrollRect��");
                return;
            }

            _itemInstances = new List<AZScrollListItem>();
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
    }
}


