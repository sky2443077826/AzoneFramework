using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using AzoneFramework.Controller;

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
        StoreManager.Instance.CreateNewSave("快乐小神仙", eGender.Male);
    }
}
