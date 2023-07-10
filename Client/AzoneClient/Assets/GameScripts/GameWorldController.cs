using AzoneFramework;
using AzoneFramework.Controller;
using AzoneFramework.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�����ܿ�����
/// </summary>
public class GameWorldController : ControllerBase
{
    internal override void OnCreate()
    {
        base.OnCreate();
    }

    internal override void OnDispose()
    {
        base.OnDispose();
    }

    /// <summary>
    /// ������Ϸ����
    /// </summary>
    public void EnterGameWorld()
    {
        SceneLoader.Instance.EnterScene(ESceneDefine.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}


