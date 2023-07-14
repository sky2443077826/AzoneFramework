using AzoneFramework.Addressable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework.UI
{
    /// <summary>
    /// ����UI����Ļ���
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIBase : MonoBehaviour
    {
        // �ʲ���ַ
        public string Address { get; set; }

        // ����Transform���
        protected Transform _cacheTrans;
        
        /// <summary>
        /// ���صľ���ͼ����
        /// </summary>
        private Dictionary<string, Sprite> _spriteCaches;


        #region ��������

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="address"></param>
        public void OnLoad(string address)
        {
            Address = address;
            _cacheTrans = transform;
            _spriteCaches = new Dictionary<string, Sprite>();
        }

        /// <summary>
        /// ����
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
        /// ����
        /// </summary>
        public void Create()
        {
            OnCreate();
        }

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected virtual void OnCreate() { }

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected virtual void OnDispose() { }

        #endregion

        /// <summary>
        /// ���ؾ���ͼ
        /// </summary>
        /// <param name="spriteAddress"></param>
        protected Sprite LoadSprite(string spriteAddress)
        {
            // �ӻ����м���
            if (_spriteCaches.TryGetValue(spriteAddress, out Sprite sprite))
            {
                return sprite;
            }

            // ���ʲ��м���
            sprite = AddressableLoader.Instance.LoadSprite(spriteAddress);
            if (sprite != null)
            {
                // ���뻺��
                _spriteCaches[spriteAddress] = sprite;
            }

            return sprite;
        }

        /// <summary>
        /// �ͷ����м��صľ���ͼ��Դ
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
