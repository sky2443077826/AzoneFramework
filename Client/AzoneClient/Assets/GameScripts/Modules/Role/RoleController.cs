using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using System;
using AzoneFramework.Controller;

/// <summary>
/// ��ɫ�����������ݣ�
/// </summary>
public class RoleController : ControllerBase
{
    /// <summary>
    /// ����
    /// </summary>
    public ulong MainRolUID { get; private set; }

    internal override void OnCreate()
    {
        base.OnCreate();

        ObjectFactory.Instance.AddCreatedEvent(RoleCreated);
    }

    internal override void OnDispose()
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

    /// <summary>
    /// ��ɫ�����ص�
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void RoleCreated(eObjectType type, DataList args)
    {
        if (type != eObjectType.Role)
        {
            return;
        }

        SetMainRole(args.ReadULong(0));
    }
}
