using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace AzoneFramework
{
    /// <summary>
    /// 场景加载类
    /// </summary>
    public class SceneController : Singleton<SceneController>
    {
        // 场景配置资产地址
        private static readonly string sceneProfileAddress = "SceneProfile";

        // 场景配置
        private SceneProfile _sceneProfile;

        // 已加载的场景
        private List<ESceneDefine> _loadedScenes;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // 加载场景配置文件
            _sceneProfile = AddressableLoader.Instance.InstantiateScriptableObject<SceneProfile>(sceneProfileAddress);
            if (_sceneProfile == null)
            {
                GameLog.Error($"创建SceneController失败！---> 无法加载场景配置文件{sceneProfileAddress}。");
                return;
            }

            _loadedScenes = new List<ESceneDefine>(4);
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
            

            if (_loadedScenes.Contains(define))
            {

                return;
            }
        }

        /// <summary>
        /// 异步加载场景
        /// 为了避免加载场景导致主线程卡住
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadSceneAsync()
        {
            yield break;
        }


        private void UnLoadScene(ESceneDefine define)
        {
            if (define == ESceneDefine.Invalid)
            {
                GameLog.Error($"卸载场景失败！---> 无效的场景{define}。");
                return;
            }

            if (!_sceneProfile.TryGetSceneConfig(define, out SceneConfig config))
            {
                GameLog.Error($"卸载场景失败！---> 未配置场景{define}的数据。");
                return;
            }

            if (config.isAddressbale)
            {
            }
            else
            {

            }
        }
    }
}
