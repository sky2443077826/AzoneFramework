using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using AzoneFramework.Scene;
using AzoneFramework.Controller;

/// <summary>
/// 主场景
/// </summary>
public class MainScene : SceneBase 
{
    // 启动控制器
    private MainController _mainCtrl;

    /// <summary>
    /// 加载完成时
    /// </summary>
    public override void OnLoadEnd()
    {
        base.OnLoadEnd();

        _mainCtrl = ControllerManager.Instance.CreateController<MainController>(EControllerDefine.Main);
        if (_mainCtrl == null)
        {
            GameLog.Error($"场景加载失败！---> 场景【{config.define}】创建控制器【{EControllerDefine.Main}】失败。");
        }
    }

    /// <summary>
    /// 销毁场景时
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
