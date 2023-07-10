using System;
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

        /// <summary>
        /// mono单例类
        /// </summary>
        private List<IMonoSingleton> _singletons;

        /* 游戏更新事件 */ 
        private event Action _onFixedUpdate;
        private event Action _onUpdate;
        private event Action _onLateUpdate;

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(this.gameObject);
            }

            _singletons = new List<IMonoSingleton>();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _singletons.Count; i++)
            {
                if (_singletons[i] != null)
                {
                    _singletons[i].OnFixedUpdate();
                }
            }

            _onFixedUpdate?.Invoke();
        }

        private void Update()
        {
            for (int i = 0; i < _singletons.Count; i++)
            {
                if (_singletons[i] != null)
                {
                    _singletons[i].OnUpdate();
                }
            }
            _onUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _singletons.Count; i++)
            {
                if (_singletons[i] != null)
                {
                    _singletons[i].OnLateUpdate();
                }
            }

            _onLateUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            if (_singletons != null)
            {
                foreach (var monoSingleton in _singletons)
                {
                    monoSingleton?.OnDispose();
                }
                _singletons.Clear();
                _singletons = null;
            }
            _onFixedUpdate = null;
            _onLateUpdate = null;
            _onUpdate = null;
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
            DontDestroyOnLoad(Instance.gameObject);
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

        #region 事件处理

        public void AddOnFixedUpdate(Action action)
        {
            _onFixedUpdate -= action;        //去重
            _onFixedUpdate += action;
        }

        public void AddOnUpdate(Action action)
        {
            _onUpdate -= action;        //去重
            _onUpdate += action;
        }

        public void AddOnLateUpdate(Action action)
        {
            _onLateUpdate -= action;        //去重
            _onLateUpdate += action;
        }

        public void RemoveOnFixedUpdate(Action action)
        {
            if (_onFixedUpdate != null)
            {
                _onFixedUpdate -= action;
            }
        }

        public void RemoveOnUpdate(Action action)
        {
            if (_onUpdate != null)
            {
                _onUpdate -= action;
            }
        }

        public void RemoveOnLateUpdate(Action action)
        {
            if (_onLateUpdate != null)
            {
                _onLateUpdate -= action;
            }
        }

        #endregion

        #region 单例管理

        public void AddMonoSingleton(IMonoSingleton monoSingleton)
        {
            if (_singletons.Contains(monoSingleton))
            {
                return;
            }

            _singletons.Add(monoSingleton);
        }

        public void RemoveMonoSingleton(IMonoSingleton monoSingleton)
        {
            _singletons.Remove(monoSingleton);
        }

        #endregion
    }
}
