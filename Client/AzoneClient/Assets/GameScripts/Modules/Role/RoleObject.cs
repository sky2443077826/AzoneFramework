using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;

/// <summary>
/// ��ɫ���ݶ���
/// </summary>
public class RoleObject : DataObject
{

    /// <summary>
    /// ��ʼ��
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