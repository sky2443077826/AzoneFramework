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
                    Instance.ReleaseAsset(Address);
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
        /// �ͷ�����
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
        /// �ͷ��ʲ�
        /// ���ǵ���LoadAsset�������ص�Asset���ҽ��������ù�������Ӧ��ʹ�ô˷���ֱ���ͷ�Asset
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

        #region ģ�͹���

        /// <summary>
        /// ʵ����ģ��
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public T InstantiateModel<T>(string address, Transform parent = null) where T : ModelBase
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

            ModelBase modelBase = gameObject.GetOrAddComponent<T>();
            modelBase.OnCreate(address);
            assetRef.AddCount();

            return modelBase as T;
        }

        /// <summary>
        /// �ͷ�ģ��ʵ��
        /// </summary>
        /// <param name="address"></param>
        public void ReleaseModel(string address)
        {
            ReleaseRefrence(address);
        }

        #endregion

        #region ����ͼ����

        /// <summary>
        /// ���ؾ���ͼ�ʲ�
        /// </summary>
        /// <param name="altasAddress"></param>
        /// <param name="spriteName"></param>
        public Sprite LoadSprite(string altasAddress, string spriteName)
        {
            string spriteAddress = StringUtility.GetSubAssetAddress(altasAddress, spriteName);
            return LoadSprite(spriteAddress);
        }

        /// <summary>
        /// ���ؾ���ͼ�ʲ�
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
            string spriteAddress = StringUtility.GetSubAssetAddress(altasAddress, spriteName);
            ReleaseAsset(spriteAddress);
        }

        /// <summary>
        /// ж�ؾ���ͼ�ʲ�
        /// </summary>
        /// <param name="address"></param>
        public void UnLoadSprite(string address)
        {
            ReleaseRefrence(address);
        }

        #endregion

        #region UI����

        /// <summary>
        /// ʵ����UI����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        public UIBase InstantiateUI(string address, Type scriptType = null)
        {
            if (string.IsNullOrEmpty(address))
            {
                GameLog.Error("ʵ����UIʧ�ܣ�---> �ʲ���ַ��������Ϊ�ա�");
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
                GameLog.Error($"ʵ����UIʧ�ܣ�---> �ʲ���{address}����ʵ����ΪGameObject��");
                return null;
            }


            if (scriptType == null)
            {
                scriptType = typeof(UIBase);
            }

            UIBase uiBase = gameObject.GetOrAddComponent(scriptType) as UIBase;
            if (uiBase == null)
            {
                GameLog.Error($"ʵ����UIʧ�ܣ�---> �ʲ���{address}����������{scriptType.Name}��");
                return null;
            }

            uiBase.OnLoad(address);
            assetRef.AddCount();

            return uiBase;
        }

        /// <summary>
        /// �ͷ�UI����
        /// </summary>
        /// <param name="address"></param>
        public void ReleaseUI(string address)
        {
            ReleaseRefrence(address);
        }

        #endregion

        #region �ű����ݶ������

        /// <summary>
        /// ʵ����ScriptableObject
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public T InstantiateScriptableObject<T>(string address) where T : ScriptableObjectBase
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
        /// �ͷ�ScriptableObject
        /// </summary>
        /// <param name="address"></param>
        public void ReleaseScriptableObject(string address)
        {
            ReleaseRefrence(address);
        }

        #endregion
    }
}
