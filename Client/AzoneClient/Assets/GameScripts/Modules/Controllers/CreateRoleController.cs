using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using AzoneFramework.Controller;
using AzoneFramework.Scene;

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
        if (!StoreManager.Instance.CreateNewSave("����С����", eGender.Male))
        {
            GameLog.Error("����ʧ�ܣ�");
            return;
        }

        // ����������
        SceneLoader.Instance.EnterScene(ESceneDefine.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
