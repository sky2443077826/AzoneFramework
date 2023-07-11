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
    /// 场景加载类
    /// </summary>
    public class SceneLoader : Singleton<SceneLoader>
    {
        // 场景配置资产地址
        private static readonly string _SCENEPROFILE_ADDRESS = "SceneProfile";

        // 场景配置
        private SceneProfile _sceneProfile;

        // 已加载的场景
        private Dictionary<ESceneDefine, SceneBase> _loadedScenes;

        // 当前加载场景
        private SceneBase _currentLoadScene;

        // 当前加载句柄
        private AsyncOperationHandle<SceneInstance> _curHandle;

        // 当前加载操作
        private AsyncOperation _operation;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // 加载场景配置文件
            _sceneProfile = AddressableLoader.Instance.InstantiateScriptableObject<SceneProfile>(_SCENEPROFILE_ADDRESS);
            if (_sceneProfile == null)
            {
                GameLog.Error($"创建SceneController失败！---> 无法加载场景配置文件{_SCENEPROFILE_ADDRESS}。");
                return;
            }

            _loadedScenes = new Dictionary<ESceneDefine, SceneBase>(4);
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            // 卸载场景配置文件
            if (_sceneProfile != null)
            {
                Object.Destroy(_sceneProfile);
                _sceneProfile = null;
            }
        }

        /// <summary>
        /// 进入场景
        /// </summary>
        /// <param name="define"></param>
        /// <param name="mode"></param>
        public void EnterScene(ESceneDefine define, LoadSceneMode mode)
        {
            if (define == ESceneDefine.Invalid)
            {
                GameLog.Error($"进入场景失败！---> 无效的场景{define}。");
                return;
            }

            if (!_sceneProfile.TryGetSceneConfig(define, out SceneConfig config))
            {
                GameLog.Error($"进入场景失败！---> 未配置场景{define}的数据。");
                return;
            }
            
            if (_loadedScenes.ContainsKey(define))
            {
                GameLog.Error($"进入场景失败！---> 场景{define}已存在。");
                return;
            }

            if (mode == LoadSceneMode.Single)
            {
                // 预卸载所有场景
                foreach (var scene in _loadedScenes.Values)
                {
                    scene.OnDispose();
                }
                _loadedScenes.Clear();
            }

            // 创建场景对象
            Type scriptType = Type.GetType(config.scriptType);
            _currentLoadScene = Activator.CreateInstance(scriptType) as SceneBase;
            if (_currentLoadScene == null)
            {
                GameLog.Error($"进入场景失败！---> 场景{define}脚本类型{config.scriptType}错误。");
                return;
            }
            _currentLoadScene.config = config;
            _currentLoadScene.OnLoadStart();


            // 开启场景加载协程
            GameMonoRoot.Instance.StartCoroutine(LoadSceneAsync(config, mode));
        }

        /// <summary>
        /// 异步加载场景
        /// 为了避免加载场景导致主线程卡住
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

            // 等待场景激活
            yield return SetSceneActive(config.isAddressbale);
            // 执行一次gc，清理垃圾
            GC.Collect();
            _currentLoadScene.OnLoadEnd();
            yield return Yielder.fixedUpdate;

            // 场景展示之前处理一些必要的逻辑
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
        /// 是否加载完成
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
        /// 是否加载完成
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
        /// 激活场景
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
