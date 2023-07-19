using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AzoneFramework.UI
{
    /// <summary>
    /// 滚动列表基础元素
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class AZScrollListChild : MonoBehaviour
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 元素索引
        /// </summary>
        public int ItemIndex { get; set; }

        /// <summary>
        /// 父列表
        /// </summary>
        public AZScrollList ParentList { get; private set; }

        /// <summary>
        /// 方形变换组件
        /// </summary>
        public RectTransform CacheRecTrans { get; private set; }


        /// <summary>
        /// 初始化
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


