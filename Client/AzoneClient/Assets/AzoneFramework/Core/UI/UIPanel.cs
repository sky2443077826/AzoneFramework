using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// UI������
    /// �������Ӧ�ü̳д���
    /// </summary>
    public class UIPanel : UIBase
    {
        /// <summary>
        /// �ر�λ�á�
        /// ����屻�ر�ʱ���ƶ�����Ұ֮�⡣����CPU������
        /// </summary>
        private static readonly Vector3 _CLOSE_POS = new Vector3(99999, 99999, 0);

        /// <summary>
        /// ��ʾ����
        /// </summary>
        private DataList _openArgs;

        /// <summary>
        /// �������
        /// </summary>
        public UIPanelConfig Config { get; private set; }


        #region ��������

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnCreate()
        {
            OnPanelCreate();
        }

        /// <summary>
        /// ����ʱ
        /// </summary>
        protected override void OnDispose()
        {
            OnPanelDestroy();
        }

        /// <summary>
        /// ��崴��
        /// </summary>
        protected virtual void OnPanelCreate()
        {

        }

        /// <summary>
        /// ����
        /// </summary>
        protected virtual void OnPanelOpen()
        {

        }

        /// <summary>
        /// �����ʾ
        /// </summary>
        protected virtual void OnPanelShow()
        {

        }

        /// <summary>
        /// �������
        /// </summary>
        protected virtual void OnPanelHide()
        {

        }

        /// <summary>
        /// ���ر�
        /// </summary>
        protected virtual void OnPanelClose()
        {

        }

        /// <summary>
        /// �������
        /// </summary>
        protected virtual void OnPanelDestroy()
        {

        }

        #endregion

        /// <summary>
        /// ��
        /// </summary>
        public void Open(DataList args)
        {
            _openArgs = args.Copy();
            Show();
        }

        /// <summary>
        /// չʾ
        /// </summary>
        public void Show()
        {
            OnPanelShow();
            _cacheTrans.localPosition = Vector3.zero;
        }

        /// <summary>
        /// ����
        /// </summary>
        public void Hide()
        {
            _cacheTrans.localPosition = _CLOSE_POS;
            OnPanelHide();
        }

        /// <summary>
        /// �ر�
        /// </summary>
        public void Close()
        {
            Hide();
            OnPanelClose();

            _openArgs?.Dispose();
            _openArgs = null;
        }
    }
}