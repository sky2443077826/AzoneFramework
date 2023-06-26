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
                // TODO 广播增加进度事件
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
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        public virtual void OnLoadEnd()
        {

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
