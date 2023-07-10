using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework.Controller
{
    /// <summary>
    /// ����������
    /// </summary>
    public class ControllerBase
    {
        /// <summary>
        /// ����ʱ
        /// </summary>
        internal virtual void OnCreate()
        {
            // ���ĸ�����Ϣ
            GameMonoRoot.Instance.AddOnUpdate(OnUpdate);
            GameMonoRoot.Instance.AddOnFixedUpdate(OnFixedUpdate);
            GameMonoRoot.Instance.AddOnLateUpdate(OnLateUpdate);
        }

        /// <summary>
        /// ����ʱ
        /// </summary>
        internal virtual void OnDispose()
        {
            // �Ƴ�������Ϣ
            GameMonoRoot.Instance.RemoveOnUpdate(OnUpdate);
            GameMonoRoot.Instance.RemoveOnFixedUpdate(OnFixedUpdate);
            GameMonoRoot.Instance.RemoveOnLateUpdate(OnLateUpdate);
        }

        protected virtual void OnLateUpdate() { }

        protected virtual void OnFixedUpdate() { }

        protected virtual void OnUpdate() { }
    }
}
