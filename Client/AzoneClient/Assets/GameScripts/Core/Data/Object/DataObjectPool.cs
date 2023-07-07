//////////////////////////////////////////////////////////////////////////
/// File:   DataPool.cs
/// Date:   2023/04/11
/// Desc:   data�����
/// Author: z9y
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace AzoneFramework
{
    /// <summary>
    /// �����
    /// </summary>
    public class DataPool : Singleton<DataPool>
    {
        /// <summary>
        /// ����أ�ID -> pool
        /// </summary>
        private Dictionary<int, Queue<IDataObject>> _dataPools;

        protected override void OnCreate()
        {
            _dataPools = new Dictionary<int, Queue<IDataObject>>();
        }

        /// <summary>
        /// ��������
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
        /// ��������ID��ȡ��Ӧ�Ķ���
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
        /// ȡ������
        /// </summary>
        /// <returns></returns>
        public T FetchObject<T>(int config) where T : class, IDataObject, new()
        {
            Queue<IDataObject> queue = GetQueueByConfigID(config);
            if (queue == null)
            {
                GameLog.Error($"û�з�������ID[{config}]��������޷�ȡ������!");
                return null;
            }

            T obj = null;
            if (queue.Count > 0)
            {
                obj = queue.Dequeue() as T;
                if (obj == null)
                {
                    GameLog.Error($"Mono����[{config}]���޷�ת��������[{typeof(T)}]!");
                    return null;
                }
            }

            if (obj == null)
            {
                // ��������
                obj = new T();
            }

            return obj;
        }

        /// <summary>
        /// ���ն���
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
