using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ����UI����Ļ���
    /// </summary>
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
            AddressableLoader.Instance.ReleaseUI(Address);
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
    }
}
