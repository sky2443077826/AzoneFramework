using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AzoneFramework
{
    /// <summary>
    /// Mono单例
    /// </summary>
    public interface IMonoSingleton 
    {
        void OnCreate();
        void OnUpdate();
        void OnFixedUpdate();
        void OnLateUpdate();
        void OnDispose();
    }

    /// <summary>
    /// Mono单例类
    /// </summary>
    public class MonoSingleton<T> where T : class, IMonoSingleton
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameLog.Error($"Mono单例调用失败！---> {typeof(T).Name}未初始化。");
                }

                return _instance;
            }
        }

        /// <summary>
        /// 创建单例
        /// </summary>
        public static T Create()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = Activator.CreateInstance<T>();
            _instance.OnCreate();
            return _instance;
        }

        /// <summary>
        /// 销毁单例
        /// </summary>
        public static void Dispose()
        {
            if (_instance != null)
            {
                _instance.OnDispose();
                _instance = null;
            }
        }
    }
}
