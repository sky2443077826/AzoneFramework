using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    public class ScriptableObjectBase : ScriptableObject, IAddresableObect
    {
        /// <summary>
        /// ×Ê²úµØÖ·
        /// </summary>
        public string Address { get; set; }

        private void OnDestroy()
        {
            DestoryInstance();
        }


        public void OnCreate(string address)
        {
            Address = address;
        }

        public void DestoryInstance()
        {
            AddressableLoader.Instance.DestroyInstance(Address);
        }
    }
}
