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

        // 缓存Transform组件
        protected Transform _cacheTrans;
        
        /// <summary>
        /// 加载的精灵图缓存
        /// </summary>
        private Dictionary<string, Sprite> _spriteCaches;


        #region 生命周期

        /// <summary>
        /// 当加载
        /// </summary>
        /// <param name="address"></param>
        public void OnLoad(string address)
        {
            Address = address;
            _cacheTrans = transform;
            _spriteCaches = new Dictionary<string, Sprite>();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestroy()
        {
            OnDispose();
            AddressableLoader.Instance.ReleaseUI(Address);
        }

        /// <summary>
        /// 创建
        /// </summary>
        public void Create()
        {
            OnCreate();
        }

        /// <summary>
        /// 创建时
        /// </summary>
        protected virtual void OnCreate() { }

        /// <summary>
        /// 销毁时
        /// </summary>
        protected virtual void OnDispose() { }

        #endregion
    }
}
