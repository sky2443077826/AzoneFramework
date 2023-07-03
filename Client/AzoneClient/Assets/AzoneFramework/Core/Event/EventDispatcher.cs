using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 事件分发器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventDispatcher<T> :Singleton<EventDispatcher<T>> where T : Enum
    {
        // 监听者
        public delegate void Listener(T id, DataList dataList);

        // 事件池
        private Dictionary<T, Listener> _eventDict;

        protected override void OnCreate()
        {
            base.OnCreate();

            _eventDict = new Dictionary<T, Listener>(100);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _eventDict?.Clear();
        }

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="listener"></param>
        public void Listen(T eventID, Listener listener)
        {
            if (!_eventDict.ContainsKey(eventID))
            {
                _eventDict.Add(eventID, listener);
            }

            _eventDict[eventID] -= listener;
            _eventDict[eventID] += listener;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="listener"></param>
        public void Remove(T eventID, Listener listener)
        {
            if (!_eventDict.ContainsKey(eventID))
            {
                return;
            }

            _eventDict[eventID] -= listener;
            if (_eventDict[eventID] == null && _eventDict.ContainsKey(eventID))
            {
                _eventDict.Remove(eventID);
            }
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="evenID"></param>
        /// <param name="dataList"></param>
        public void Dispatch(T evenID, DataList dataList = null)
        {
            if (!_eventDict.TryGetValue(evenID, out Listener listener))
            {
                return;
            }

            listener?.Invoke(evenID, dataList);
        }
    }
}
