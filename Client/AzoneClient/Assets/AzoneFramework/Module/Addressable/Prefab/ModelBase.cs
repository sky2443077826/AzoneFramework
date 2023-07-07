using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework.Addressable
{
    /// <summary>
    /// ģ�ͻ���
    /// ���д�AB�ʲ��м��ص�ģ�Ͷ�Ӧ�ü̳д���
    /// </summary>
    public class ModelBase : MonoBehaviour, IAddresableObect
    {
        /// <summary>
        /// �ʲ���ַ
        /// </summary>
        public string Address { get; set; }

        protected Transform _cacheTrans;

        void OnDestroy()
        {
            DestoryInstance();
        }

        /// <summary>
        /// ��������ʱ
        /// </summary>
        /// <param name="address"></param>
        public void OnCreate(string address)
        {
            Address = address;
            _cacheTrans = transform;
        }

        /// <summary>
        /// ����ʵ��
        /// </summary>
        public void DestoryInstance()
        {
            AddressableLoader.Instance.ReleaseModel(Address);
        }
    }
}
