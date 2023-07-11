using AzoneFramework;
using AzoneFramework.Controller;
using AzoneFramework.UI;
using AzoneFramework.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 启动场景控制器
/// </summary>
public class StartController : ControllerBase
{
    // 创角控制器
    CreateRoleController _createRoleController;

    internal override void OnCreate()
    {
        base.OnCreate();

        // 创建创角控制器
        _createRoleController = ControllerManager.Instance.CreateController<CreateRoleController>(EControllerDefine.CreateRole);
    }

    internal override void OnDispose()
    {
        base.OnDispose();
    }

    /// <summary>
    /// 创建新存档
    /// </summary>
    public void CreateNewSave()
    {
        if (_createRoleController == null)
        {
            if (ControllerManager.Instance.ContainController(EControllerDefine.CreateRole))
            {
                _createRoleController = ControllerManager.Instance.GetController<CreateRoleController>(EControllerDefine.CreateRole);
            }
            else
            {
                _createRoleController = ControllerManager.Instance.CreateController<CreateRoleController>(EControllerDefine.CreateRole);
            }
        }

        _createRoleController.CreateRole();
    }

    /// <summary>
    /// 进入主世界
    /// </summary>
    public void EnterMainWorld()
    {
        SceneLoader.Instance.EnterScene(ESceneDefine.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
