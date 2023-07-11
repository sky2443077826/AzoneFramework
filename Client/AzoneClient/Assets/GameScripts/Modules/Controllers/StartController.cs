using AzoneFramework;
using AzoneFramework.Controller;
using AzoneFramework.UI;
using AzoneFramework.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������������
/// </summary>
public class StartController : ControllerBase
{
    // ���ǿ�����
    CreateRoleController _createRoleController;

    internal override void OnCreate()
    {
        base.OnCreate();

        // �������ǿ�����
        _createRoleController = ControllerManager.Instance.CreateController<CreateRoleController>(EControllerDefine.CreateRole);
    }

    internal override void OnDispose()
    {
        base.OnDispose();
    }

    /// <summary>
    /// �����´浵
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
    /// ����������
    /// </summary>
    public void EnterMainWorld()
    {
        SceneLoader.Instance.EnterScene(ESceneDefine.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
