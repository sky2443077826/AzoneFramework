using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 所有UI组件的基类
    /// </summary>
    public class UIBase : MonoBehaviour
    {
        // 资产地址
        public string Address { get; set; }

        #region 生命周期
        
        /// <summary>
        /// 创建时
        /// </summary>
        public virtual void OnCreate()
        {

        }

        /// <summary>
        /// 弃置
        /// </summary>
        public virtual void OnDispose()
        {

        }


        #endregion




    }
}
