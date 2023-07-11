using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using AzoneFramework.Scene;
using AzoneFramework.Controller;

/// <summary>
/// ������
/// </summary>
public class MainScene : SceneBase 
{
    // ����������
    private MainController _mainCtrl;

    /// <summary>
    /// �������ʱ
    /// </summary>
    public override void OnLoadEnd()
    {
        base.OnLoadEnd();

        _mainCtrl = ControllerManager.Instance.CreateController<MainController>(EControllerDefine.Main);
        if (_mainCtrl == null)
        {
            GameLog.Error($"��������ʧ�ܣ�---> ������{config.define}��������������{EControllerDefine.Main}��ʧ�ܡ�");
        }
    }

    /// <summary>
    /// ���ٳ���ʱ
    /// </summary>
    public override void OnDispose()
    {
        if (_mainCtrl != null)
        {
            _mainCtrl = null;
            ControllerManager.Instance.DestoryController(EControllerDefine.Main);
        }

        base.OnDispose();
    }
}
