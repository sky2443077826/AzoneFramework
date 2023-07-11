using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using System;
using AzoneFramework.Controller;

/// <summary>
/// 角色管理器（数据）
/// </summary>
public class RoleController : ControllerBase
{
    /// <summary>
    /// 主角
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
    /// 设置角色
    /// </summary>
    /// <param name="role"></param>
    public void SetMainRole(ulong roleUID)
    {
        if (MainRolUID != 0)
        {
            GameLog.Error("设置主角失败！---> 已存在角色，请先删除。");
            return;
        }

        if (roleUID == 0 || !ObjectManager.Instance.HasObject(roleUID))
        {
            GameLog.Error($"设置主角失败！---> 不存在角色：{roleUID}。");
            return;
        }

        MainRolUID = roleUID;
    }

    /// <summary>
    /// 删除主角角色
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
    /// 获取主角数据对象
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
    /// 角色创建回调
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
