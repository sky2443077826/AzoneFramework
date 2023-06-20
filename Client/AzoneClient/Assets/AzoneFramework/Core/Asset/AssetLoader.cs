using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AzoneFramework
{
    /// <summary>
    /// 资产引用类
    /// </summary>
    internal class AssetReference
    {
        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount { get; set; }

        /// <summary>
        /// 资源操作句柄
        /// </summary>
        public AsyncOperationHandle Handle { get; }

        public AssetReference(AsyncOperationHandle handle)
        {
            RefCount = 0;
            Handle = handle;
        }

        public void AddCount()
        {

        }

        public void SubCount()
        {

        }
    }

    /// <summary>
    /// 资产加载类
    /// </summary>
    public class AssetLoader : Singleton<AssetLoader>
    {
        // 资产缓存
        private Dictionary<string, AssetReference> _assetCache;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // 预申请一定数量的内存空间，避免频繁扩容
            _assetCache = new Dictionary<string, AssetReference>(10007);
        }

        /// <summary>
        /// 销毁时 
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// 加载资产(异步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T LoadAsset<T>(string address) where T : UnityEngine.Object
        {
            if (_assetCache.TryGetValue(address, out AssetReference assetRef))
            {
                return assetRef.Handle.Result as T;
            }

            AsyncOperationHandle handle = Addressables.LoadAssetAsync<T>(address);
            T tObj = handle.WaitForCompletion() as T;
            if (handle.OperationException != null)
            {
                GameLog.Error($"资产加载错误！---> 错误原因:{handle.OperationException}");
                return null;
            }

            return tObj;
        }
    }
}
