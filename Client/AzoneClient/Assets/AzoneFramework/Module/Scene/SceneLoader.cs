using AzoneFramework.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AzoneFramework.Scene
{
    /// <summary>
    /// ����������
    /// </summary>
    public class SceneLoader : Singleton<SceneLoader>
    {
        // ���������ʲ���ַ
        private static readonly string _SCENEPROFILE_ADDRESS = "SceneProfile";

        // ��������
        private SceneProfile _sceneProfile;

        // �Ѽ��صĳ���
        private Dictionary<ESceneDefine, SceneBase> _loadedScenes;

        // ��ǰ���س���
        private SceneBase _currentLoadScene;

        // ��ǰ���ؾ��
        private AsyncOperationHandle<SceneInstance> _curHandle;

        // ��ǰ���ز���
        private AsyncOperation _operation;

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // ���س��������ļ�
            _sceneProfile = AddressableLoader.Instance.InstantiateScriptableObject<SceneProfile>(_SCENEPROFILE_ADDRESS);
            if (_sceneProfile == null)
            {
                GameLog.Error($"����SceneControllerʧ�ܣ�---> �޷����س��������ļ�{_SCENEPROFILE_ADDRESS}��");
                return;
            }

            _loadedScenes = new Dictionary<ESceneDefine, SceneBase>(4);
        }

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            // ж�س��������ļ�
            if (_sceneProfile != null)
            {
                Object.Destroy(_sceneProfile);
                _sceneProfile = null;
            }
        }

        /// <summary>
        /// ���볡��
        /// </summary>
        /// <param name="define"></param>
        /// <param name="mode"></param>
        public void EnterScene(ESceneDefine define, LoadSceneMode mode)
        {
            if (define == ESceneDefine.Invalid)
            {
                GameLog.Error($"���볡��ʧ�ܣ�---> ��Ч�ĳ���{define}��");
                return;
            }

            if (!_sceneProfile.TryGetSceneConfig(define, out SceneConfig config))
            {
                GameLog.Error($"���볡��ʧ�ܣ�---> δ���ó���{define}�����ݡ�");
                return;
            }
            
            if (_loadedScenes.ContainsKey(define))
            {
                GameLog.Error($"���볡��ʧ�ܣ�---> ����{define}�Ѵ��ڡ�");
                return;
            }

            if (mode == LoadSceneMode.Single)
            {
                // Ԥж�����г���
                foreach (var scene in _loadedScenes.Values)
                {
                    scene.OnDispose();
                }
                _loadedScenes.Clear();
            }

            // ������������
            Type scriptType = Type.GetType(config.scriptType);
            _currentLoadScene = Activator.CreateInstance(scriptType) as SceneBase;
            if (_currentLoadScene == null)
            {
                GameLog.Error($"���볡��ʧ�ܣ�---> ����{define}�ű�����{config.scriptType}����");
                return;
            }
            _currentLoadScene.config = config;
            _currentLoadScene.OnLoadStart();


            // ������������Э��
            GameMonoRoot.Instance.StartCoroutine(LoadSceneAsync(config, mode));
        }

        /// <summary>
        /// �첽���س���
        /// Ϊ�˱�����س����������߳̿�ס
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadSceneAsync(SceneConfig config, LoadSceneMode mode)
        {
            float simulateProgress = 0;
            float realProgress = 0;

            if (config.isAddressbale)
            {
                _curHandle = Addressables.LoadSceneAsync(config.SceneName, mode, false);
            }
            else
            {
                _operation = SceneManager.LoadSceneAsync(config.SceneName, mode);
                _operation.allowSceneActivation = false;
            }

            while (true)
            {
                realProgress = GetSceneLoadProgress(config.isAddressbale);
                while (realProgress - simulateProgress > 0.1f)
                {
                    simulateProgress += 0.01f;
                    _currentLoadScene.Progress = simulateProgress * 0.8f;
                    yield return Yielder.endOfFrame;
                }

                if (simulateProgress > 1f)
                {
                    simulateProgress = 1;
                }

                _currentLoadScene.Progress = simulateProgress * 0.8f;
                if (IsSceneLoadEnd(config.isAddressbale))
                {
                    break;
                }
                yield return Yielder.endOfFrame;
            }

            // �ȴ���������
            yield return SetSceneActive(config.isAddressbale);
            // ִ��һ��gc����������
            GC.Collect();
            _currentLoadScene.OnLoadEnd();
            yield return Yielder.fixedUpdate;

            // ����չʾ֮ǰ����һЩ��Ҫ���߼�
            yield return _currentLoadScene.BeforeShow();
            yield return Yielder.fixedUpdate;

            _currentLoadScene.OnShow();
            if (_currentLoadScene.Show)
            {
                _loadedScenes[config.define] = _currentLoadScene;
                _currentLoadScene = null;
                _operation = null;
                _curHandle = default;

                yield break;
            }
        }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        /// <returns></returns>
        private float GetSceneLoadProgress(bool isAddressable)
        {
            if (isAddressable)
            {
                return _curHandle.GetDownloadStatus().Percent;             // 0 - 1
            }
            else
            {
                return _operation.progress * 10 / 9;        // 0 - 0.9
            }
        }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        /// <returns></returns>
        private bool IsSceneLoadEnd(bool isAddressable)
        {
            if (isAddressable)
            {
                return _curHandle.IsDone;
            }
            else
            {
                return _operation.progress >= 0.9f;
            }
        }

        /// <summary>
        /// �����
        /// </summary>
        /// <param name="addressable"></param>
        /// <returns></returns>
        private IEnumerator SetSceneActive(bool addressable)
        {
            if (addressable)
            {
                yield return _curHandle.Result.ActivateAsync();
            }
            else
            {
                _operation.allowSceneActivation = true;
                yield return Yielder.endOfFrame;
            }
        }
    }
}
