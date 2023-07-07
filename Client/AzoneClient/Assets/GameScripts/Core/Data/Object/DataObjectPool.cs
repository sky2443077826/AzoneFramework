//////////////////////////////////////////////////////////////////////////
/// File:   DataPool.cs
/// Date:   2023/04/11
/// Desc:   data对象池
/// Author: z9y
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace AzoneFramework
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class DataPool : Singleton<DataPool>
    {
        /// <summary>
        /// 对象池，ID -> pool
        /// </summary>
        private Dictionary<int, Queue<IDataObject>> _dataPools;

        protected override void OnCreate()
        {
            _dataPools = new Dictionary<int, Queue<IDataObject>>();
        }

        /// <summary>
        /// 销毁自身
        /// </summary>
        protected override void OnDispose()
        {
            foreach (var queue in _dataPools.Values)
            {
                foreach (var data in queue)
                {
                    data.Dispose();
                }
                queue.Clear();
            }

            _dataPools.Clear();
            _dataPools = null;
        }

        /// <summary>
        /// 根据配置ID获取相应的队列
        /// </summary>
        /// <param name="configID"></param>
        /// <param name="queue"></param>
        private Queue<IDataObject> GetQueueByConfigID(int configID)
        {
            if (!ConfigManager.Instance.HasConfig(configID))
            {
                return null;
            }

            if (!_dataPools.TryGetValue(configID, out Queue<IDataObject> queue))
            {
                queue = new Queue<IDataObject>(GameConstant.kMaxPoolCacheCount);
            }

            return queue;
        }

        /// <summary>
        /// 取出对象
        /// </summary>
        /// <returns></returns>
        public T FetchObject<T>(int config) where T : class, IDataObject, new()
        {
            Queue<IDataObject> queue = GetQueueByConfigID(config);
            if (queue == null)
            {
                GameLog.Error($"没有发现配置ID[{config}]，对象池无法取出对象!");
                return null;
            }

            T obj = null;
            if (queue.Count > 0)
            {
                obj = queue.Dequeue() as T;
                if (obj == null)
                {
                    GameLog.Error($"Mono对象[{config}]，无法转换成类型[{typeof(T)}]!");
                    return null;
                }
            }

            if (obj == null)
            {
                // 创建对象
                obj = new T();
            }

            return obj;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <returns></returns>
        public void ReleaseObject(IDataObject obj)
        {
            if (obj == null)
            {
                return;
            }

            obj.Dispose();

            Queue<IDataObject> queue = GetQueueByConfigID(obj.ConfigID);
            if (queue != null && queue.Count < GameConstant.kMaxPoolCacheCount)
            {
                queue.Enqueue(obj);
                return;
            }
        }
    }

}
