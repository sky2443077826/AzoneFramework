using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using AzoneFramework.Controller;
using AzoneFramework.Scene;

/// <summary>
/// 创角控制器
/// </summary>
public class CreateRoleController : ControllerBase
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
    /// 创键角色
    /// </summary>
    public void CreateRole()
    {
        if (!StoreManager.Instance.CreateNewSave("快乐小神仙", eGender.Male))
        {
            GameLog.Error("创角失败！");
            return;
        }

        // 进入主场景
        SceneLoader.Instance.EnterScene(ESceneDefine.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
