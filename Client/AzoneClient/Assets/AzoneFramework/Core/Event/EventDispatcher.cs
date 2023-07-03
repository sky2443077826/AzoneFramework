using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// �¼��ַ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventDispatcher<T> :Singleton<EventDispatcher<T>> where T : Enum
    {
        // ������
        public delegate void Listener(T id, DataList dataList);

        // �¼���
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
        /// ����
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
        /// �Ƴ�
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
        /// �ɷ��¼�
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
