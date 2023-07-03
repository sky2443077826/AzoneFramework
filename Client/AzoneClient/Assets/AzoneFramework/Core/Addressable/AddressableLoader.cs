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
                    Instance.ReleaseAsset(Address);
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
        /// 释放引用
        /// </summary>
        /// <param name="address"></param>
        private void ReleaseRefrence(string address)
        {
            if (!_assetCache.TryGetValue(address, out AssetReference assetReference))
            {
                return;
            }

            assetReference?.SubCount();
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
        /// 释放资产
        /// 除非调用LoadAsset方法加载的Asset，且进行了引用管理，否则不应该使用此方法直接释放Asset
        /// </summary>
        /// <param name="address"></param>
        public void ReleaseAsset(string address)
        {
            if (!_assetCache.TryGetValue(address, out AssetReference assetReference))
            {
                return;
            }

            Addressables.Release(assetReference.Handle);
            _assetCache.Remove(address);
        }

        #endregion

        #region 模型管理

        /// <summary>
        /// 实例化模型
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public T InstantiateModel<T>(string address, Transform parent = null) where T : ModelBase
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

            ModelBase modelBase = gameObject.GetOrAddComponent<T>();
            modelBase.OnCreate(address);
            assetRef.AddCount();

            return modelBase as T;
        }

        /// <summary>
        /// 释放模型实例
        /// </summary>
        /// <param name="address"></param>
        public void ReleaseModel(string address)
        {
            ReleaseRefrence(address);
        }

        #endregion

        #region 精灵图管理

        /// <summary>
        /// 加载精灵图资产
        /// </summary>
        /// <param name="altasAddress"></param>
        /// <param name="spriteName"></param>
        public Sprite LoadSprite(string altasAddress, string spriteName)
        {
            string spriteAddress = StringUtility.GetSubAssetAddress(altasAddress, spriteName);
            return LoadSprite(spriteAddress);
        }

        /// <summary>
        /// 加载精灵图资产
        /// </summary>
        /// <param name="altasAddress"></param>
        /// <param name="spriteName"></param>
        public Sprite LoadSprite(string spriteAddress)
        {
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
            string spriteAddress = StringUtility.GetSubAssetAddress(altasAddress, spriteName);
            ReleaseAsset(spriteAddress);
        }

        /// <summary>
        /// 卸载精灵图资产
        /// </summary>
        /// <param name="address"></param>
        public void UnLoadSprite(string address)
        {
            ReleaseRefrence(address);
        }

        #endregion

        #region UI管理

        /// <summary>
        /// 实例化UI对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        public UIBase InstantiateUI(string address, Type scriptType = null)
        {
            if (string.IsNullOrEmpty(address))
            {
                GameLog.Error("实例化UI失败！---> 资产地址名不可以为空。");
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

            GameObject gameObject = Object.Instantiate(obj) as GameObject;
            if (gameObject == null)
            {
                GameLog.Error($"实例化UI失败！---> 资产：{address}不能实例化为GameObject。");
                return null;
            }


            if (scriptType == null)
            {
                scriptType = typeof(UIBase);
            }

            UIBase uiBase = gameObject.GetOrAddComponent(scriptType) as UIBase;
            if (uiBase == null)
            {
                GameLog.Error($"实例化UI失败！---> 资产：{address}不能添加组件{scriptType.Name}。");
                return null;
            }

            uiBase.OnLoad(address);
            assetRef.AddCount();

            return uiBase;
        }

        /// <summary>
        /// 释放UI对象
        /// </summary>
        /// <param name="address"></param>
        public void ReleaseUI(string address)
        {
            ReleaseRefrence(address);
        }

        #endregion

        #region 脚本数据对象管理

        /// <summary>
        /// 实例化ScriptableObject
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public T InstantiateScriptableObject<T>(string address) where T : ScriptableObjectBase
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
        /// 释放ScriptableObject
        /// </summary>
        /// <param name="address"></param>
        public void ReleaseScriptableObject(string address)
        {
            ReleaseRefrence(address);
        }

        #endregion
    }
}
