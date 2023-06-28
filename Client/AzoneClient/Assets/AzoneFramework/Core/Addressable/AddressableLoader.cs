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
    /// ��Ѱַ�ʲ�������
    /// </summary>
    public class AddressableLoader : Singleton<AddressableLoader>
    {
        private static readonly string ADDRESSABLE_SUB_ASSET_FORMAT = "{0}[{1}]";

        /// <summary>
        /// �ʲ�������
        /// </summary>
        internal class AssetReference
        {
            /// <summary>
            /// ���ü���
            /// </summary>
            public int RefCount { get; private set; }

            /// <summary>
            /// ��Դ��ַ
            /// </summary>
            public string Address { get; private set; }

            /// <summary>
            /// ��Դ�������
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

        // �ʲ�����
        private Dictionary<string, AssetReference> _assetCache;

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // Ԥ����һ���������ڴ�ռ䣬����Ƶ������
            _assetCache = new Dictionary<string, AssetReference>(10000);

        }

        /// <summary>
        /// ����ʱ 
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// ��ȡ���ʲ���ַ
        /// </summary>
        /// <returns></returns>
        private string GetSubAssetAddress(string parentAddress, string subAssetName)
        {
            return string.Format(ADDRESSABLE_SUB_ASSET_FORMAT, parentAddress, subAssetName);
        }

        #region �ʲ�����

        /// <summary>
        /// �����ʲ�
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
                GameLog.Error($"�ʲ����ش���---> �ʲ�{address}������ԭ��:{handle.OperationException}");
                return null;
            }

            assetRef = new AssetReference(address, handle);
            _assetCache.Add(address, assetRef);
            return assetRef;
        }

        /// <summary>
        /// ж���ʲ�
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
        /// �����ʲ�
        /// ���ã��˽ӿڲ��������ʲ����ü�������ʹ����Ҫ���й������ü�����
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
                GameLog.Error($"�����ʲ�����---> �޷�ת���ʲ���{address}�����ͣ�{typeof(T).Name}��");
            }

            return tObject;
        }

        /// <summary>
        /// ʵ����Ԥ����
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public T InstantiatePrefab<T>(string address, Transform parent = null) where T : PrefabBase
        {
            if (string.IsNullOrEmpty(address))
            {
                GameLog.Error("ʵ����Ԥ����ʧ�ܣ�---> �ʲ���ַ��������Ϊ�ա�");
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
                GameLog.Error($"ʵ����Ԥ����ʧ�ܣ�---> �ʲ���{address}����ʵ����ΪGameObject��");
                return null;
            }

            PrefabBase prefabBase = gameObject.GetOrAddComponent<T>();
            prefabBase.OnCreate(address);
            assetRef.AddCount();

            return prefabBase as T;
        }

        /// <summary>
        /// ʵ����ScriptableObject
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public T InstantiateScriptableObject<T>(string address) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(address))
            {
                GameLog.Error("ʵ����ScriptableObjectʧ�ܣ�---> �ʲ���ַ��������Ϊ�ա�");
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
                GameLog.Error($"ʵ����ScriptableObjectʧ�ܣ�---> �ʲ���{address}����ʵ����ΪScriptableObject��");
                return null;
            }

            sObjectBase.OnCreate(address);
            assetRef.AddCount();

            return sObjectBase as T;
        }

        /// <summary>
        /// ����ʵ��
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
        /// ���ؾ���ͼ�ʲ�
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

            // ת��Ϊ����ͼ��
            Sprite sprite = assetRef.Handle.Result as Sprite;
            if (sprite == null)
            {
                GameLog.Error($"����spriteʧ�ܣ�---> �ʲ���{spriteAddress}����ת��Ϊsprite���͡�");
            }

            return sprite;
        }

        /// <summary>
        /// ж�ؾ���ͼ�ʲ�
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
