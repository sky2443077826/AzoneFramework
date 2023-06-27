using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    [Serializable]
    public class SceneBase
    {
        // 进度上限
        public static readonly float MAX_LOAD_PROGRESS = 1f;

        // 逻辑进度起始
        public static readonly float START_LOGIC_LOAD_PROGRESS = 0.8f;
        
        // 逻辑进度条长度
        public static readonly float LOGIC_LOAD_PROGRESS = MAX_LOAD_PROGRESS - START_LOGIC_LOAD_PROGRESS;

        // 场景配置
        public SceneConfig config;


        private float _progress;

        /// <summary>
        /// 加载进度
        /// </summary>
        public float Progress
        {
            get => _progress;

            set
            {
                _progress = value;
                // 广播场景加载进度条改变
                DataList dataList = DataList.Get();
                FrameEvent.Instance.Dispatch(EFrameEventID.OnSceneLoading, dataList.AddInt((int)config.define).AddFloat(_progress));
                dataList.Dispose();
            }
        }

        /// <summary>
        /// 可展示
        /// </summary>
        public bool Show { get; private set; }

        /// <summary>
        /// 加载开始时
        /// </summary>
        public virtual void OnLoadStart()
        {
            // 派发场景加载开始事件
            DataList dataList = DataList.Get();
            FrameEvent.Instance.Dispatch(EFrameEventID.OnLoadSceneStart, dataList.AddInt((int)config.define));
            dataList.Dispose();
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        public virtual void OnLoadEnd()
        {
            // 派发场景加载结束事件
            DataList dataList = DataList.Get();
            FrameEvent.Instance.Dispatch(EFrameEventID.OnSceneLoadEnd, dataList.AddInt((int)config.define));
            dataList.Dispose();
        }

        /// <summary>
        /// 展示前处理逻辑部分，此函数需要执行20%的进度条。
        /// 子类若重写，则必须重新写加载20%的逻辑。
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
        /// 展示
        /// </summary>
        /// <returns></returns>
        public virtual void OnShow()
        {
            Show = true;

            // 派发场景加载之后事件
            DataList dataList = DataList.Get();
            FrameEvent.Instance.Dispatch(EFrameEventID.AfterSceneLoad, dataList.AddInt((int)config.define));
            dataList.Dispose();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void OnDispose()
        {
            Show = false;
            Progress = 0;
            config = default(SceneConfig);
        }
    }
}
