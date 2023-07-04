using AzoneFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加载界面
/// </summary>
public class LoadingPanel : UIPanel
{
    // 加载进度条
    public UIComProgress progLoading;


    #region 生命周期

    #endregion

    protected override void OnPanelCreate()
    {
    }

    protected override void OnPanelOpen()
    {
    }

    protected override void OnPanelShow()
    {
        FrameEvent.Instance.Listen(EFrameEventID.OnSceneLoading, OnSceneLoading);
    }

    protected override void OnPanelHide()
    {
        FrameEvent.Instance.Remove(EFrameEventID.OnSceneLoading, OnSceneLoading);
    }

    protected override void OnPanelClose()
    {
    }

    protected override void OnPanelDestroy()
    {
    }

    /// <summary>
    /// 场景加载中
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dataList"></param>
    private void OnSceneLoading(EFrameEventID id, DataList dataList)
    {
        if (id != EFrameEventID.OnSceneLoading || dataList == null)
        {
            return;
        }

        float progress = dataList.ReadFloat(1);
        progLoading.Value = progress;
    }
}
