using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AzoneFramework
{
    /// <summary>
    /// 可寻址资产加载类
    /// </summary>
    public class AddressableLoader : Singleton<AddressableLoader>
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
                    AddressableLoader.Instance.UnloadAsset(Address);
                }
            }
        }

        /// <summary>
        /// 场景引用类
        /// </summary>
        internal class SceneReference
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

            public SceneReference(string address, AsyncOperationHandle handle)
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
                    AddressableLoader.Instance.UnloadAsset(Address);
                }
            }
        }

        // 资产缓存
        private Dictionary<string, AssetReference> _assetCache;

        // 场景缓存
        private Dictionary<string, SceneReference> _sceneCache;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // 预申请一定数量的内存空间，避免频繁扩容
            _assetCache = new Dictionary<string, AssetReference>(10007);

            // 预申请一定数量的内存空间，避免频繁扩容
            _sceneCache = new Dictionary<string, SceneReference>(19);
        }

        /// <summary>
        /// 销毁时 
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        #region 资产管理

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
        public void UnloadAsset(string address)
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
        /// 实例化ScriptableObject
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public T InstantiateScriptableObject<T>(string address) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(address))
            {
                GameLog.Error("实例化ScriptableObject失败！---> 资产地址名不可以为空。");
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

            ScriptableObjectBase sObjectBase = Object.Instantiate(obj) as ScriptableObjectBase;
            if (sObjectBase == null)
            {
                GameLog.Error($"实例化ScriptableObject失败！---> 资产：{address}不能实例化为ScriptableObject。");
                return null;
            }

            sObjectBase.OnCreate(address);
            assetRef.AddCount();

            return sObjectBase as T;
        }

        /// <summary>
        /// 销毁实例
        /// </summary>
        /// <param name="address"></param>
        public void DestroyInstance(string address)
        {
            if (!_assetCache.TryGetValue(address, out AssetReference assetReference))
            {
                return;
            }

            assetReference?.SubCount();
        }

        #endregion

        #region 场景管理

        /// <summary>
        /// 加载场景(异步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private IEnumerator LoadScene(string address, LoadSceneMode mode)
        {
            if (_sceneCache.TryGetValue(address, out SceneReference sceneRef))
            {
                yield break;
            }

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(address, mode);
            yield return handle;

            if (handle.OperationException != null)
            {
                GameLog.Error($"场景加载错误！---> 错误原因:{handle.OperationException}");
                yield break;
            }

            sceneRef = new SceneReference(address, handle);
            _sceneCache.Add(address, sceneRef);
        }

        /// <summary>
        /// 卸载c
        /// </summary>
        /// <param name="address"></param>
        public void UnLoadScene(string address)
        {
            if (!_assetCache.TryGetValue(address, out AssetReference assetReference))
            {
                return;
            }

            Addressables.Release(assetReference.Handle);
            _assetCache.Remove(address);
        }

        #endregion
    }
}
