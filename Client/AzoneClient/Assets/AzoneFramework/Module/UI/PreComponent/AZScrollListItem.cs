using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AzoneFramework.UI
{
    /// <summary>
    /// 滚动列表基础元素
    /// </summary>
    public class AZScrollListItem : UIComponent
    {
        /// <summary>
        /// 父列表
        /// </summary>
        public AZScrollList ParentList { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }


    }
}


