using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using AzoneFramework.Controller;

/// <summary>
/// ���ǿ�����
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
    /// ������ɫ
    /// </summary>
    public void CreateRole()
    {
        StoreManager.Instance.CreateNewSave("����С����", eGender.Male);
    }
}
