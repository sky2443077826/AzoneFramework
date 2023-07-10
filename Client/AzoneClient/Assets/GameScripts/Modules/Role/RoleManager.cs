using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using System;

/// <summary>
/// ��ɫ������
/// </summary>
public class RoleManager : Singleton<RoleManager>
{
    /// <summary>
    /// ����
    /// </summary>
    public ulong MainRolUID { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }

    /// <summary>
    /// ���ý�ɫ
    /// </summary>
    /// <param name="role"></param>
    public void SetMainRole(ulong roleUID)
    {
        if (MainRolUID != 0)
        {
            GameLog.Error("��������ʧ�ܣ�---> �Ѵ��ڽ�ɫ������ɾ����");
            return;
        }

        if (roleUID == 0 || !ObjectManager.Instance.HasObject(roleUID))
        {
            GameLog.Error($"��������ʧ�ܣ�---> �����ڽ�ɫ��{roleUID}��");
            return;
        }

        MainRolUID = roleUID;
    }

    /// <summary>
    /// ɾ�����ǽ�ɫ
    /// </summary>
    public void RemoveMainRole()
    {
        if (MainRolUID == 0)
        {
            return;
        }
        ObjectManager.Instance.DestoryObject(MainRolUID);
        MainRolUID = 0;
    }

    /// <summary>
    /// ��ȡ�������ݶ���
    /// </summary>
    private RoleObject GetMainRoleObject()
    {
        if (!ObjectManager.Instance.TryGetObject(MainRolUID, out RoleObject roleObject) || roleObject == null)
        {
            return null;
        }

        return roleObject;
    }
}
