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

        #region ��������
        
        /// <summary>
        /// ����ʱ
        /// </summary>
        public virtual void OnCreate()
        {

        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void OnDispose()
        {

        }


        #endregion




    }
}
