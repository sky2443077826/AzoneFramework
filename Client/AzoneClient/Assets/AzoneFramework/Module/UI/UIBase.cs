using AzoneFramework.Addressable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework.UI
{
    /// <summary>
    /// 所有UI组件的基类
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
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

            if (Address != null)
            {
                ReleaseAllSprite();
                AddressableLoader.Instance.ReleaseUI(Address);
            }
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

        /// <summary>
        /// 加载精灵图
        /// </summary>
        /// <param name="spriteAddress"></param>
        protected Sprite LoadSprite(string spriteAddress)
        {
            // 从缓存中加载
            if (_spriteCaches.TryGetValue(spriteAddress, out Sprite sprite))
            {
                return sprite;
            }

            // 从资产中加载
            sprite = AddressableLoader.Instance.LoadSprite(spriteAddress);
            if (sprite != null)
            {
                // 存入缓存
                _spriteCaches[spriteAddress] = sprite;
            }

            return sprite;
        }

        /// <summary>
        /// 释放所有加载的精灵图资源
        /// </summary>
        protected void ReleaseAllSprite()
        {
            if (_spriteCaches == null || _spriteCaches.Count <= 0)
            {
                return;
            }

            foreach (var address in _spriteCaches.Keys)
            {
                AddressableLoader.Instance.UnLoadSprite(address);
            }

            _spriteCaches.Clear();
        }
    }
}
