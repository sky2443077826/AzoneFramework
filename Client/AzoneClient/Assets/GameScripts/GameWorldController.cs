using AzoneFramework;
using AzoneFramework.Controller;
using AzoneFramework.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏世界总控制器
/// </summary>
public class GameWorldController : ControllerBase
{
    // 角色控制器
    public RoleController roleController;

    internal override void OnCreate()
    {
        base.OnCreate();

        // 初始化核心模块
        InitCoreModule();

        // 初始化角色控制器
        roleController = ControllerManager.Instance.CreateController<RoleController>(EControllerDefine.Role);
    }

    internal override void OnDispose()
    {
        base.OnDispose();

        // 释放核心模块
        DisposeCoreModule();
    }

    /// <summary>
    /// 初始化核心模块
    /// </summary>
    private void InitCoreModule()
    {
        ConfigManager.Instance.Create();                    //配置管理器
        DataUtility.Instance.Create();                      //数据工具类
        ObjectManager.Instance.Create();                    //数据对象管理器
        StoreManager.Instance.Create();                     //存档管理器
    }

    /// <summary>
    /// 释放核心模块
    /// </summary>
    private void DisposeCoreModule()
    {
        ConfigManager.Instance.Dispose();                    //配置管理器
        DataUtility.Instance.Dispose();                      //数据工具类
        ObjectManager.Instance.Dispose();                    //数据对象管理器
        StoreManager.Instance.Dispose();                     //存档管理器
    }

    /// <summary>
    /// 进入游戏世界
    /// </summary>
    public void EnterGameWorld()
    {
        SceneLoader.Instance.EnterScene(ESceneDefine.StartScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}


