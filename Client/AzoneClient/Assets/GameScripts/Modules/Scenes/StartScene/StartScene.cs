using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using AzoneFramework.Scene;
using AzoneFramework.Controller;

/// <summary>
/// ��������
/// </summary>
public class StartScene : SceneBase 
{
    // ����������
    private StartController _starCtrl;

    /// <summary>
    /// �������ʱ
    /// </summary>
    public override void OnLoadEnd()
    {
        base.OnLoadEnd();

        _starCtrl = ControllerManager.Instance.CreateController<StartController>(EControllerDefine.Start);
        if (_starCtrl == null)
        {
            GameLog.Error($"��������ʧ�ܣ�---> ������{config.define}��������������{EControllerDefine.Start}��ʧ�ܡ�");
        }
    }

    /// <summary>
    /// ���ٳ���ʱ
    /// </summary>
    public override void OnDispose()
    {
        if (_starCtrl != null)
        {
            _starCtrl = null;
            ControllerManager.Instance.DestoryController(EControllerDefine.Start);
        }

        base.OnDispose();
    }
}
