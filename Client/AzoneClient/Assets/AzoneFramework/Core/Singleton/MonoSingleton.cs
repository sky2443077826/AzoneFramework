using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AzoneFramework
{
    /// <summary>
    /// Mono����
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
    /// Mono������
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
                    GameLog.Error($"Mono��������ʧ�ܣ�---> {typeof(T).Name}δ��ʼ����");
                }

                return _instance;
            }
        }

        /// <summary>
        /// ��������
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
        /// ���ٵ���
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
