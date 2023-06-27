using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ������
    /// </summary>
    public class Singleton<T>  where T : class
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Activator.CreateInstance<T>();
                }

                return _instance;
            }
        }

        private bool _created;

        #region ��������

        protected virtual void OnCreate()
        {
            GameLog.Normal($"===ģ��{typeof(T).Name}����===");
        }

        protected virtual void OnDispose()
        {
            GameLog.Normal($"===ģ��{typeof(T).Name}����===");
        }

        #endregion

        /// <summary>
        /// ��������
        /// </summary>
        public void Create()
        {
            if (_created)
            {
                return;
            }

            OnCreate();
        }

        /// <summary>
        /// ���ٵ���
        /// </summary>
        public void Dispose()
        {
            if (!_created)
            {
                return;
            }

            _instance = null;
            _created = false;
            OnDispose();
        }
    }
}
