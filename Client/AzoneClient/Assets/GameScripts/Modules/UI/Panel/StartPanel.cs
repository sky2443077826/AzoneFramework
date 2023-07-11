using AzoneFramework;
using AzoneFramework.Controller;
using AzoneFramework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartPanel : UIPanel
{
    public Button btnReadSave;
    public Button btnNewGame;

    protected override void OnPanelCreate()
    {
        base.OnPanelCreate();
    }

    protected override void OnPanelOpen()
    {
        base.OnPanelOpen();

        // 监听UI事件
        UIEvent.GetEvent(btnNewGame.gameObject).onClick += OnClickNewGame;
    }

    protected override void OnPanelShow()
    {
        base.OnPanelShow();
    }

    protected override void OnPanelHide()
    {
        base.OnPanelHide();
    }

    protected override void OnPanelClose()
    {
        base.OnPanelClose();

        // 监听UI事件
        UIEvent.GetEvent(btnNewGame.gameObject).onClick -= OnClickNewGame;
    }

    protected override void OnPanelDestroy()
    {
        base.OnPanelDestroy();
    }


    #region UI事件回调

    /// <summary>
    /// 点击新游戏游戏时
    /// </summary>
    private void OnClickNewGame(UIEvent sender, PointerEventData data)
    {
        StartController startController = ControllerManager.Instance.GetController<StartController>(EControllerDefine.Start);
        if (startController == null)
        {
            return;
        }

        startController.CreateNewSave();
    }

    #endregion

}
