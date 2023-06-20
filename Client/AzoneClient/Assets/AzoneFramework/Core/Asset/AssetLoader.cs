using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AzoneFramework
{
    /// <summary>
    /// �ʲ�������
    /// </summary>
    internal class AssetReference
    {
        /// <summary>
        /// ���ü���
        /// </summary>
        public int RefCount { get; set; }

        /// <summary>
        /// ��Դ�������
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
    /// �ʲ�������
    /// </summary>
    public class AssetLoader : Singleton<AssetLoader>
    {
        // �ʲ�����
        private Dictionary<string, AssetReference> _assetCache;

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // Ԥ����һ���������ڴ�ռ䣬����Ƶ������
            _assetCache = new Dictionary<string, AssetReference>(10007);
        }

        /// <summary>
        /// ����ʱ 
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// �����ʲ�(�첽)
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
                GameLog.Error($"�ʲ����ش���---> ����ԭ��:{handle.OperationException}");
                return null;
            }

            return tObj;
        }
    }
}
