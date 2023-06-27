using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 单例类
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

        #region 生命周期

        protected virtual void OnCreate()
        {
            GameLog.Normal($"===模块{typeof(T).Name}启动===");
        }

        protected virtual void OnDispose()
        {
            GameLog.Normal($"===模块{typeof(T).Name}销毁===");
        }

        #endregion

        /// <summary>
        /// 创建单例
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
        /// 销毁单例
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
