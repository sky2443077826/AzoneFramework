using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AzoneFramework
{
    /// <summary>
    /// 游戏启动入口
    /// </summary>
    public class GameBoot : MonoBehaviour
    {
        [Header("日志等级")]
        public ELogLevel logLevel;

        [Header("日志是否导出(非编辑器)")]
        public bool isSaveLog;

        private void Start()
        {

#if UNITY_EDITOR
            // 编辑器不输出日志
            isSaveLog = false;
#endif

            // 初始化日志系统
            GameLog.Init(logLevel, isSaveLog);
            // 初始化资源加载器
            AssetLoader.Instance.Create();

            LaunchGame();
        }

        /// <summary>
        /// 启动游戏
        /// </summary>
        private void LaunchGame()
        {
            GameLog.Normal("===游戏启动===");

            PrefabBase go1 = AssetLoader.Instance.InstantiatePrefab<PrefabBase>("Assets/Cube.prefab");
            PrefabBase go2 = AssetLoader.Instance.InstantiatePrefab<PrefabBase>("Assets/Cube.prefab");
            PrefabBase go3 = AssetLoader.Instance.InstantiatePrefab<PrefabBase>("Assets/Cube.prefab");
            PrefabBase go4 = AssetLoader.Instance.InstantiatePrefab<PrefabBase>("Assets/Cube.prefab");

            Destroy(go1.gameObject);
            Destroy(go2.gameObject);
            Destroy(go3.gameObject);
            Destroy(go4.gameObject);
        }
    }
}

