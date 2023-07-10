using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework.Controller
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    public class ControllerBase
    {
        /// <summary>
        /// 创建时
        /// </summary>
        internal virtual void OnCreate()
        {
            // 订阅更新消息
            GameMonoRoot.Instance.AddOnUpdate(OnUpdate);
            GameMonoRoot.Instance.AddOnFixedUpdate(OnFixedUpdate);
            GameMonoRoot.Instance.AddOnLateUpdate(OnLateUpdate);
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        internal virtual void OnDispose()
        {
            // 移除更新消息
            GameMonoRoot.Instance.RemoveOnUpdate(OnUpdate);
            GameMonoRoot.Instance.RemoveOnFixedUpdate(OnFixedUpdate);
            GameMonoRoot.Instance.RemoveOnLateUpdate(OnLateUpdate);
        }

        protected virtual void OnLateUpdate() { }

        protected virtual void OnFixedUpdate() { }

        protected virtual void OnUpdate() { }
    }
}
