using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// Ԥ�������
    /// ���д�AB�ʲ��м��ص�Ԥ���嶼Ӧ�ü̳д���
    /// </summary>
    public class PrefabBase : MonoBehaviour, IAddresableObect
    {
        /// <summary>
        /// �ʲ���ַ
        /// </summary>
        public string Address { get; set; }

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
        }

        /// <summary>
        /// ����ʵ��
        /// </summary>
        public void DestoryInstance()
        {
            AddressableLoader.Instance.DestroyInstance(Address);
        }
    }
}
