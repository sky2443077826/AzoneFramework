using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 框架事件
    /// </summary>
    public enum EFrameEventID
    {
        Invalid     = 0,

        /* 场景事件 1 - 99 */
        OnLoadSceneStart = 1,       //开始加载场景
        OnSceneLoading,             //场景加载中
        OnSceneLoadEnd,             //场景加载完成
        AfterSceneLoad,             //场景加载后

        /* UI事件 101 - 199 */
        UIModuleInitSuccess = 101,      // UI模块初始化成功
    }

    public class FrameEvent : EventDispatcher<EFrameEventID> { }
}
