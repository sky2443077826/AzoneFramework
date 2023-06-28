using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace AzoneFramework
{
    /// <summary>
    /// 可寻址资产加载类
    /// </summary>
    public class AddressableLoader : Singleton<AddressableLoader>
    {
        private static readonly string ADDRESSABLE_SUB_ASSET_FORMAT = "{0}[{1}]";

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

        // 资产缓存
        private Dictionary<string, AssetReference> _assetCache;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // 预申请一定数量的内存空间，避免频繁扩容
            _assetCache = new Dictionary<string, AssetReference>(10000);

        }

        /// <summary>
        /// 销毁时 
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// 获取子资产地址
        /// </summary>
        /// <returns></returns>
        private string GetSubAssetAddress(string parentAddress, string subAssetName)
        {
            return string.Format(ADDRESSABLE_SUB_ASSET_FORMAT, parentAddress, subAssetName);
        }

        #region 资产管理

        /// <summary>
        /// 加载资产
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
                GameLog.Error($"资产加载错误！---> 资产{address}，错误原因:{handle.OperationException}");
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
        /// 加载资产
        /// 慎用，此接口不会增加资产引用计数！若使用需要自行管理引用计数。
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

        /// <summary>
        /// 加载精灵图资产
        /// </summary>
        /// <param name="altasAddress"></param>
        /// <param name="spriteName"></param>
        public Sprite LoadSprite(string altasAddress, string spriteName)
        {
            string spriteAddress = GetSubAssetAddress(altasAddress, spriteName);
            AssetReference assetRef = LoadAsset(spriteAddress);
            if (assetRef == null)
            {
                return null;
            }

            // 转换为精灵图集
            Sprite sprite = assetRef.Handle.Result as Sprite;
            if (sprite == null)
            {
                GameLog.Error($"加载sprite失败！---> 资产：{spriteAddress}不能转换为sprite类型。");
            }

            return sprite;
        }

        /// <summary>
        /// 卸载精灵图资产
        /// </summary>
        /// <param name="altasAddress"></param>
        /// <param name="spriteName"></param>
        public void UnLoadSprite(string altasAddress, string spriteName)
        {
            string spriteAddress = GetSubAssetAddress(altasAddress, spriteName);
            if (!_assetCache.TryGetValue(spriteAddress, out AssetReference assetReference))
            {
                return;
            }

            assetReference?.SubCount();
        }
        #endregion

    }
}
