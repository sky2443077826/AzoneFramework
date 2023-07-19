using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AzoneFramework.UI
{
    /// <summary>
    /// �����б����Ԫ��
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class AZScrollListChild : MonoBehaviour
    {
        /// <summary>
        /// ����
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Ԫ������
        /// </summary>
        public int ItemIndex { get; set; }

        /// <summary>
        /// ���б�
        /// </summary>
        public AZScrollList ParentList { get; private set; }

        /// <summary>
        /// ���α任���
        /// </summary>
        public RectTransform CacheRecTrans { get; private set; }


        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init(int index, AZScrollList parentList)
        {
            Index = index;
            ParentList = parentList;
            CacheRecTrans = GetComponent<RectTransform>();
            gameObject.SetActive(true);
        }

        public void OnDestroy()
        {

        }

    }
}


