using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace AzoneFramework
{
    /// <summary>
    /// ����������
    /// </summary>
    public class SceneController : Singleton<SceneController>
    {
        // ���������ʲ���ַ
        private static readonly string sceneProfileAddress = "SceneProfile";

        // ��������
        private SceneProfile _sceneProfile;

        // �Ѽ��صĳ���
        private List<ESceneDefine> _loadedScenes;

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // ���س��������ļ�
            _sceneProfile = AddressableLoader.Instance.InstantiateScriptableObject<SceneProfile>(sceneProfileAddress);
            if (_sceneProfile == null)
            {
                GameLog.Error($"����SceneControllerʧ�ܣ�---> �޷����س��������ļ�{sceneProfileAddress}��");
                return;
            }

            _loadedScenes = new List<ESceneDefine>(4);
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
            

            if (_loadedScenes.Contains(define))
            {

                return;
            }
        }

        /// <summary>
        /// �첽���س���
        /// Ϊ�˱�����س����������߳̿�ס
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
                GameLog.Error($"ж�س���ʧ�ܣ�---> ��Ч�ĳ���{define}��");
                return;
            }

            if (!_sceneProfile.TryGetSceneConfig(define, out SceneConfig config))
            {
                GameLog.Error($"ж�س���ʧ�ܣ�---> δ���ó���{define}�����ݡ�");
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
