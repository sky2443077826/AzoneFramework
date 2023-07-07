using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework.Addressable
{
    /// <summary>
    /// 模型基类
    /// 所有从AB资产中加载的模型都应该继承此类
    /// </summary>
    public class ModelBase : MonoBehaviour, IAddresableObect
    {
        /// <summary>
        /// 资产地址
        /// </summary>
        public string Address { get; set; }

        protected Transform _cacheTrans;

        void OnDestroy()
        {
            DestoryInstance();
        }

        /// <summary>
        /// 当被创建时
        /// </summary>
        /// <param name="address"></param>
        public void OnCreate(string address)
        {
            Address = address;
            _cacheTrans = transform;
        }

        /// <summary>
        /// 销毁实例
        /// </summary>
        public void DestoryInstance()
        {
            AddressableLoader.Instance.ReleaseModel(Address);
        }
    }
}
