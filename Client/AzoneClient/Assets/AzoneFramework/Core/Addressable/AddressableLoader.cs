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
    /// ��Ѱַ�ʲ�������
    /// </summary>
    public class AddressableLoader : Singleton<AddressableLoader>
    {
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

        /// <summary>
        /// ����������
        /// </summary>
        internal class SceneReference
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

        // �ʲ�����
        private Dictionary<string, AssetReference> _assetCache;

        // ��������
        private Dictionary<string, SceneReference> _sceneCache;

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // Ԥ����һ���������ڴ�ռ䣬����Ƶ������
            _assetCache = new Dictionary<string, AssetReference>(10007);

            // Ԥ����һ���������ڴ�ռ䣬����Ƶ������
            _sceneCache = new Dictionary<string, SceneReference>(19);
        }

        /// <summary>
        /// ����ʱ 
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        #region �ʲ�����

        /// <summary>
        /// �����ʲ�(�첽)
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
                GameLog.Error($"�ʲ����ش���---> ����ԭ��:{handle.OperationException}");
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
        /// �����ʲ�(�첽)���˽ӿڲ��������ʲ����ü�����
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

        #endregion

        #region ��������

        /// <summary>
        /// ���س���(�첽)
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
                GameLog.Error($"�������ش���---> ����ԭ��:{handle.OperationException}");
                yield break;
            }

            sceneRef = new SceneReference(address, handle);
            _sceneCache.Add(address, sceneRef);
        }

        /// <summary>
        /// ж��c
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
