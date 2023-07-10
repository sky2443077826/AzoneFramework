using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;

/// <summary>
/// 角色数据对象
/// </summary>
public class RoleObject : DataObject
{

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="configID"></param>
    /// <returns></returns>
    public override bool Init(int configID)
    {
        if (!base.Init(configID))
        {
            return false;
        }

        return true;
    }


}