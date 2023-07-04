using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// UI面板基类
    /// 所有面板应该继承此类
    /// </summary>
    public class UIPanel : UIBase
    {
        /// <summary>
        /// 关闭位置。
        /// 当面板隐藏时，移动到视野之外。避免CPU开销。
        /// </summary>
        private static readonly Vector3 _CLOSE_POS = new Vector3(99999, 99999, 0);

        /// <summary>
        /// 显示参数
        /// </summary>
        private DataList _openArgs;

        /// <summary>
        /// 画布组件
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// 面板配置
        /// </summary>
        public UIPanelConfig Config { get; private set; }


        #region 生命周期

        /// <summary>
        /// 创建时
        /// </summary>
        protected override void OnCreate()
        {
            _canvas = GetComponent<Canvas>();
            if (_canvas == null)
            {
                GameLog.Error($"创建UI面板失败！---> 面板{Config.define}未找到Canvas组件。");
                return;
            }

            OnPanelCreate();
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        protected override void OnDispose()
        {
            OnPanelDestroy();
        }

        /// <summary>
        /// 面板创建
        /// </summary>
        protected virtual void OnPanelCreate() { }

        /// <summary>
        /// 面板打开
        /// </summary>
        protected virtual void OnPanelOpen() { }

        /// <summary>
        /// 面板显示
        /// </summary>
        protected virtual void OnPanelShow() { }

        /// <summary>
        /// 面板隐藏
        /// </summary>
        protected virtual void OnPanelHide() { }

        /// <summary>
        /// 面板关闭
        /// </summary>
        protected virtual void OnPanelClose() { }

        /// <summary>
        /// 面板销毁
        /// </summary>
        protected virtual void OnPanelDestroy() { }

        #endregion

        /// <summary>
        /// 打开
        /// </summary>
        public void Open(DataList args = null)
        {
            if (args != null)
            {
                _openArgs = args.Copy();
            }
            Show();
        }

        /// <summary>
        /// 展示
        /// </summary>
        public void Show()
        {
            OnPanelShow();
            _cacheTrans.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            _cacheTrans.localPosition = _CLOSE_POS;
            OnPanelHide();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            Hide();
            OnPanelClose();

            _openArgs?.Dispose();
            _openArgs = null;
        }

        /// <summary>
        /// 设置渲染层级
        /// </summary>
        /// <param name="order"></param>
        public void SetSortingOrder(int order)
        {
            _canvas.sortingOrder = order;
        }
    }
}
