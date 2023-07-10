using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    public class GameMonoRoot : MonoBehaviour
    {

        /// <summary>
        /// ����
        /// </summary>
        public static GameMonoRoot Instance { get; private set; }

        /// <summary>
        /// mono������
        /// </summary>
        private List<IMonoSingleton> _singletons;

        /* ��Ϸ�����¼� */ 
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
        /// ��������
        /// </summary>
        /// <returns></returns>
        public static bool CreateInstance()
        {
            if (Instance != null)
            {
                GameLog.Error("GameMonoRoot�������ظ�������");
                return false;
            }

            Instance = new GameObject(typeof(GameMonoRoot).Name).GetOrAddComponent<GameMonoRoot>();
            DontDestroyOnLoad(Instance.gameObject);
            return true;
        }

        /// <summary>
        /// ���ٵ���
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

        #region �¼�����

        public void AddOnFixedUpdate(Action action)
        {
            _onFixedUpdate -= action;        //ȥ��
            _onFixedUpdate += action;
        }

        public void AddOnUpdate(Action action)
        {
            _onUpdate -= action;        //ȥ��
            _onUpdate += action;
        }

        public void AddOnLateUpdate(Action action)
        {
            _onLateUpdate -= action;        //ȥ��
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

        #region ��������

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
