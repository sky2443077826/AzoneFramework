using AzoneFramework.Addressable;
using AzoneFramework.Scene;
using AzoneFramework.UI;
using System;
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
            // 初始化事件派发器(框架和游戏)
            FrameEvent.Instance.Create();
            FrameEvent.Instance.Listen(EFrameEventID.UIModuleInitSuccess, OnUIModuleInitSuccess);

            // 创建GameMono单例
            GameMonoRoot.CreateInstance();
            // 创建游戏世界
            GameMonoRoot.Instance.AddMonoSingleton(GameWorld.Create());

            // 初始化资源加载器
            AddressableLoader.Instance.Create();
            // 初始化场景加载器
            SceneLoader.Instance.Create();
            // 初始化UI管理器
            UIManager.Instance.Create();
        }

        /// <summary>
        /// 启动游戏
        /// </summary>
        private void LaunchGame()
        {
            GameLog.Normal("===游戏启动===");
            GameWorld.Instance.EnterGameWorld();
        }

        /// <summary>
        /// 当UI模块加载完成
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataList"></param>
        private void OnUIModuleInitSuccess(EFrameEventID id, DataList dataList)
        {
            LaunchGame();
        }
    }
}

