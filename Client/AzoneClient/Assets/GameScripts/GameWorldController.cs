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
    // ��ɫ������
    public RoleController roleController;

    internal override void OnCreate()
    {
        base.OnCreate();

        // ��ʼ������ģ��
        InitCoreModule();

        // ��ʼ����ɫ������
        roleController = ControllerManager.Instance.CreateController<RoleController>(EControllerDefine.Role);
    }

    internal override void OnDispose()
    {
        base.OnDispose();

        // �ͷź���ģ��
        DisposeCoreModule();
    }

    /// <summary>
    /// ��ʼ������ģ��
    /// </summary>
    private void InitCoreModule()
    {
        ConfigManager.Instance.Create();                    //���ù�����
        DataUtility.Instance.Create();                      //���ݹ�����
        ObjectManager.Instance.Create();                    //���ݶ��������
        StoreManager.Instance.Create();                     //�浵������
    }

    /// <summary>
    /// �ͷź���ģ��
    /// </summary>
    private void DisposeCoreModule()
    {
        ConfigManager.Instance.Dispose();                    //���ù�����
        DataUtility.Instance.Dispose();                      //���ݹ�����
        ObjectManager.Instance.Dispose();                    //���ݶ��������
        StoreManager.Instance.Dispose();                     //�浵������
    }

    /// <summary>
    /// ������Ϸ����
    /// </summary>
    public void EnterGameWorld()
    {
        SceneLoader.Instance.EnterScene(ESceneDefine.StartScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}


