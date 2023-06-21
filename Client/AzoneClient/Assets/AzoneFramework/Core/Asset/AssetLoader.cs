using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AzoneFramework
{

    /// <summary>
    /// 资产加载类
    /// </summary>
    public class AssetLoader : Singleton<AssetLoader>
    {
        /// <summary>
        /// 资产引用类
        /// </summary>
        internal class AssetReference
        {
            /// <summary>
            /// 引用计数
            /// </summary>
            public int RefCount { get; private set; }

            /// <summary>
            /// 资源地址
            /// </summary>
            public string Address { get; private set; }

            /// <summary>
            /// 资源操作句柄
            /// </summary>
            public AsyncOperationHandle Handle { get; }

            public AssetReference(string address, AsyncOperationHandle handle)
            {
                RefCount = 0;
                Address = address;
                Handle = handle;
            }

            public void AddCount()
            {
                RefCount++;
            }

            public void SubCount()
            {
                RefCount--;
                if (RefCount <= 0)
                {
                    AssetLoader.Instance.UnloadAsset(Address);
                }
            }
        }

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
        /// <returns></returns>
        private AssetReference LoadAsset(string address)
        {
            if (_assetCache.TryGetValue(address, out AssetReference assetRef))
            {
                return assetRef;
            }

            AsyncOperationHandle handle = Addressables.LoadAssetAsync<Object>(address);
            Object obj = handle.WaitForCompletion() as Object;
            if (handle.OperationException != null || obj == null)
            {
                GameLog.Error($"资产加载错误！---> 错误原因:{handle.OperationException}");
                return null;
            }

            assetRef = new AssetReference(address, handle);
            _assetCache.Add(address, assetRef);
            return assetRef;
        }

        /// <summary>
        /// 卸载资产
        /// </summary>
        /// <param name="address"></param>
        private void UnloadAsset(string address)
        {
            if (!_assetCache.TryGetValue(address, out AssetReference assetReference))
            {
                return;
            }

            Addressables.Release(assetReference.Handle);
            _assetCache.Remove(address);
        }

        /// <summary>
        /// 加载资产(异步)，此接口不会增加资产引用计数！
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string address) where T : Object
        {
            AssetReference assetReference = LoadAsset(address);
            if (assetReference == null)
            {
                return null;
            }

            T tObject = assetReference.Handle.Result as T;
            if (tObject == null)
            {
                GameLog.Error($"加载资产错误！---> 无法转换资产：{address}至类型：{typeof(T).Name}。");
            }

            return tObject;
        }

        /// <summary>
        /// 实例化预制体
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public T InstantiatePrefab<T>(string address, Transform parent = null) where T : PrefabBase
        {
            if (string.IsNullOrEmpty(address))
            {
                GameLog.Error("实例化预制体失败！---> 资产地址名不可以为空。");
                return null;
            }

            AssetReference assetRef = LoadAsset(address);
            if (assetRef == null)
            {
                return null;
            }

            Object obj = assetRef.Handle.Result as Object;
            if (obj == null)
            {
                return null;
            }

            GameObject gameObject = Object.Instantiate(obj, parent) as GameObject;
            if (gameObject == null)
            {
                GameLog.Error($"实例化预制体失败！---> 资产：{address}不能实例化为GameObject。");
                return null;
            }

            PrefabBase prefabBase = gameObject.GetOrAddComponent<T>();
            prefabBase.OnCreate(address);
            assetRef.AddCount();

            return prefabBase as T;
        }

        /// <summary>
        /// 销毁预制体
        /// </summary>
        /// <param name="address"></param>
        public void DestroyPrefab(string address)
        {
            if (!_assetCache.TryGetValue(address, out AssetReference assetReference))
            {
                return;
            }

            assetReference?.SubCount();
        }
    }
}
