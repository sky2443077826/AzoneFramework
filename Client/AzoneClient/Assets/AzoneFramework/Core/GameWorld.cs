using AzoneFramework;
using AzoneFramework.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏世界总入口
/// </summary>
public class GameWorld : MonoSingleton<GameWorld>, IMonoSingleton
{
    public void OnCreate()
    {
    }

    public void OnFixedUpdate()
    {
    }

    public void OnLateUpdate()
    {
    }

    public void OnUpdate()
    {
    }

    public void OnDispose()
    {
    }

    /// <summary>
    /// 进入游戏世界
    /// </summary>
    public void EnterGameWorld()
    {
        SceneLoader.Instance.EnterScene(ESceneDefine.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
