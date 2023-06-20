using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 预制体基类
    /// 所有从AB资产中加载的预制体都应该继承此类
    /// </summary>
    public class PrefabBase : MonoBehaviour
    {
        /// <summary>
        /// 资产地址
        /// </summary>
        public string Address { get; private set; }


        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

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
        }

        /// <summary>
        /// 销毁实例
        /// </summary>
        private void DestoryInstance()
        {
            AssetLoader.Instance.DestroyPrefab(Address);
        }
    }
}
