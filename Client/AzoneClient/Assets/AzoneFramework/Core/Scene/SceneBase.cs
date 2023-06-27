using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    [Serializable]
    public class SceneBase
    {
        // ��������
        public static readonly float MAX_LOAD_PROGRESS = 1f;

        // �߼�������ʼ
        public static readonly float START_LOGIC_LOAD_PROGRESS = 0.8f;
        
        // �߼�����������
        public static readonly float LOGIC_LOAD_PROGRESS = MAX_LOAD_PROGRESS - START_LOGIC_LOAD_PROGRESS;

        // ��������
        public SceneConfig config;


        private float _progress;

        /// <summary>
        /// ���ؽ���
        /// </summary>
        public float Progress
        {
            get => _progress;

            set
            {
                _progress = value;
                // �㲥�������ؽ������ı�
                DataList dataList = DataList.Get();
                FrameEvent.Instance.Dispatch(EFrameEventID.OnSceneLoading, dataList.AddInt((int)config.define).AddFloat(_progress));
                dataList.Dispose();
            }
        }

        /// <summary>
        /// ��չʾ
        /// </summary>
        public bool Show { get; private set; }

        /// <summary>
        /// ���ؿ�ʼʱ
        /// </summary>
        public virtual void OnLoadStart()
        {
            // �ɷ��������ؿ�ʼ�¼�
            DataList dataList = DataList.Get();
            FrameEvent.Instance.Dispatch(EFrameEventID.OnLoadSceneStart, dataList.AddInt((int)config.define));
            dataList.Dispose();
        }

        /// <summary>
        /// �������
        /// </summary>
        public virtual void OnLoadEnd()
        {
            // �ɷ��������ؽ����¼�
            DataList dataList = DataList.Get();
            FrameEvent.Instance.Dispatch(EFrameEventID.OnSceneLoadEnd, dataList.AddInt((int)config.define));
            dataList.Dispose();
        }

        /// <summary>
        /// չʾǰ�����߼����֣��˺�����Ҫִ��20%�Ľ�������
        /// ��������д�����������д����20%���߼���
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator BeforeShow()
        {
            while (Progress < 1)
            {
                Progress += 0.01f;
                if (Progress > 1)
                {
                    Progress = 1;
                }
                yield return Yielder.endOfFrame;
            }
        }

        /// <summary>
        /// չʾ
        /// </summary>
        /// <returns></returns>
        public virtual void OnShow()
        {
            Show = true;

            // �ɷ���������֮���¼�
            DataList dataList = DataList.Get();
            FrameEvent.Instance.Dispatch(EFrameEventID.AfterSceneLoad, dataList.AddInt((int)config.define));
            dataList.Dispose();
        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void OnDispose()
        {
            Show = false;
            Progress = 0;
            config = default(SceneConfig);
        }
    }
}
