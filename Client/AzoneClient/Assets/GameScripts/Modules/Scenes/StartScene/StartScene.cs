using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using AzoneFramework.Scene;
using AzoneFramework.Controller;

/// <summary>
/// 启动场景
/// </summary>
public class StartScene : SceneBase 
{
    // 启动控制器
    private StartController _starCtrl;

    /// <summary>
    /// 加载完成时
    /// </summary>
    public override void OnLoadEnd()
    {
        base.OnLoadEnd();

        _starCtrl = ControllerManager.Instance.CreateController<StartController>(EControllerDefine.Start);
        if (_starCtrl == null)
        {
            GameLog.Error($"场景加载失败！---> 场景【{config.define}】创建控制器【{EControllerDefine.Start}】失败。");
        }
    }

    /// <summary>
    /// 销毁场景时
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
