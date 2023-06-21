using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    public class GameMonoRoot : MonoBehaviour
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static GameMonoRoot Instance { get; private set; }

        private List<IMonoSingleton> _singletons;

        private void Awake()
        {
            _singletons = new List<IMonoSingleton>();
        }


        /// <summary>
        /// 创建单例
        /// </summary>
        /// <returns></returns>
        public static bool CreateInstance()
        {
            if (Instance != null)
            {
                GameLog.Error("GameMonoRoot不可以重复创建！");
                return false;
            }

            Instance = new GameObject(typeof(GameMonoRoot).Name).GetOrAddComponent<GameMonoRoot>();
            return true;
        }

        /// <summary>
        /// 销毁单例
        /// </summary>
        public static void Destroy()
        {
            if (Instance != null)
            {
                return;
            }

            GameObject.Destroy(Instance.gameObject);
            Instance = null;
            return;
        }
    }
}
