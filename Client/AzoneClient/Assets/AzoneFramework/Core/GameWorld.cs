using AzoneFramework;
using AzoneFramework.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ���������
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
    /// ������Ϸ����
    /// </summary>
    public void EnterGameWorld()
    {
        SceneLoader.Instance.EnterScene(ESceneDefine.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
