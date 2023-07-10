//////////////////////////////////////////////////////////////////////////
/// File:   DataDispather.cs
/// Date:   2023/04/20
/// Desc:   �������ݸı�ַ�
/// Author: z9y
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using AzoneFramework;

// �ص������б�  function-list
using PropChangedCallbackList = System.Collections.Generic.List<PropChangedFunc>;
using RecordChangedCallbackList = System.Collections.Generic.List<RecordChangedFunc>;

// ���Իص�ί�����ͻص�
public delegate void PropChangedFunc(ulong UID, DataList args);
/// <summary>
/// ���ı�ص�ί������
/// </summary>
public delegate void RecordChangedFunc(ulong UID, DataList args);

/// <summary>
/// �ص�������
/// </summary>
class DataDispatcher : Singleton<DataDispatcher>
{
    private Dictionary<eObjectType, Dictionary<string, SortedList<int, PropChangedCallbackList>>> _propCallbacks;
    private Dictionary<eObjectType, Dictionary<string, SortedList<int, RecordChangedCallbackList>>> _recordCallbacks;

    protected override void OnCreate()
    {
        base.OnCreate();

        _propCallbacks = new Dictionary<eObjectType, Dictionary<string, SortedList<int, PropChangedCallbackList>>>();
        _recordCallbacks = new Dictionary<eObjectType, Dictionary<string, SortedList<int, RecordChangedCallbackList>>>();

        ObjectManager.Instance.ListenPropertyChanged(OnPropertyChagned);
        ObjectManager.Instance.ListenRecordChanged(OnRecordChanged);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        ObjectManager.Instance.CancelPropertyChanged(OnPropertyChagned);
        ObjectManager.Instance.CancelRecordChanged(OnRecordChanged);
    }

    /// <summary>
    /// ע�����Իص�
    /// </summary>
    /// <param name="type"></param>
    /// <param name="order"></param>
    /// <param name=""></param>
    public void RegisterPropCallback(eObjectType type, string propName, PropChangedFunc func, int order = 0)
    {
        // �Ƿ�ע�����
        if (!_propCallbacks.ContainsKey(type))
        {
            _propCallbacks.Add(type, new Dictionary<string, SortedList<int, PropChangedCallbackList>>());
        }

        // �Ƿ�ע������
        Dictionary<string, SortedList<int, PropChangedCallbackList>> prop2sortedFuncList = _propCallbacks[type];
        if (!prop2sortedFuncList.ContainsKey(propName))
        {
            prop2sortedFuncList.Add(propName, new SortedList<int, PropChangedCallbackList>());
        }

        // �Ƿ�ע�����ȼ�
        SortedList<int, PropChangedCallbackList> sortedFuncList = prop2sortedFuncList[propName];
        if (!sortedFuncList.ContainsKey(order))
        {
            sortedFuncList.Add(order, new PropChangedCallbackList());
        }

        // ע�ᵽ�����б���
        PropChangedCallbackList funcList = sortedFuncList[order];

        funcList.Add(func);
    }

    /// <summary>
    /// �Ƴ�ָ�����͵����Իص�����
    /// </summary>
    /// <param name="type"></param>
    /// <param name="propName"></param>
    /// <param name="func"></param>
    public void RemovePropCallback(eObjectType type, string propName, PropChangedFunc func)
    {
        if (!_propCallbacks.ContainsKey(type) || func == null || string.IsNullOrEmpty(propName))
        {
            return;
        }

        Dictionary<string, SortedList<int, PropChangedCallbackList>> prop2sortedFuncList = _propCallbacks[type];
        if (!prop2sortedFuncList.ContainsKey(propName))
        {
            return;
        }

        SortedList<int, PropChangedCallbackList> sortedFuncList = prop2sortedFuncList[propName];

        foreach (KeyValuePair<int, PropChangedCallbackList> skv in sortedFuncList)
        {
            foreach (var cb in skv.Value)
            {
                if (cb == func)
                {
                    skv.Value.Remove(cb);
                }
            }
        }
    }

    /// <summary>
    /// ���Իص�
    /// </summary>
    /// <param name="args">�����б�</param>
    private void OnPropertyChagned(DataList args)
    {
        // ��������
        eObjectType objType = (eObjectType)args.ReadInt(0);
        // uid
        ulong uid = args.ReadULong(1);
        // ������
        string propName = args.ReadString(2);

        if (!_propCallbacks.ContainsKey(objType))
        {
            // û��ע�����
            return;
        }

        Dictionary<string, SortedList<int, PropChangedCallbackList>> prop2sortedFuncList = _propCallbacks[objType];
        if (!prop2sortedFuncList.ContainsKey(propName))
        {
            // û��ע��������
            return;
        }

        // �������Իص�
        SortedList<int, PropChangedCallbackList> sortedFuncList = prop2sortedFuncList[propName];

        // ��������
        DataList info = DataList.Get();
        info.Append(args, 3, args.Count);

        foreach (KeyValuePair<int, PropChangedCallbackList> kv in sortedFuncList)
        {
            foreach (PropChangedFunc func in kv.Value)
            {
                func?.Invoke(uid, info);
            }
        }

        DataList.Recyle(info);
    }

    /// <summary>
    /// ע����ص�
    /// </summary>
    /// <param name="type">��������</param>
    /// <param name="recName">�������</param>
    /// <param name="func">�ص�����</param>
    /// <param name="order">���ȼ�</param>
    public void RegisterRecordCallback(eObjectType type, string recName, RecordChangedFunc func, int order = 0)
    {
        // �Ƿ�ע�����
        if (!_recordCallbacks.ContainsKey(type))
        {
            _recordCallbacks.Add(type, new Dictionary<string, SortedList<int, RecordChangedCallbackList>>());
        }

        // �Ƿ�ע������
        Dictionary<string, SortedList<int, RecordChangedCallbackList>> record2sortedFuncList = _recordCallbacks[type];
        if (!record2sortedFuncList.ContainsKey(recName))
        {
            record2sortedFuncList.Add(recName, new SortedList<int, RecordChangedCallbackList>());
        }

        // �Ƿ�ע�����ȼ�
        SortedList<int, RecordChangedCallbackList> sortedFuncList = record2sortedFuncList[recName];
        if (!sortedFuncList.ContainsKey(order))
        {
            sortedFuncList.Add(order, new RecordChangedCallbackList());
        }

        // ע�ᵽ�����б���
        RecordChangedCallbackList funcList = sortedFuncList[order];

        funcList.Add(func);
    }

    /// <summary>
    /// �Ƴ����ص�
    /// </summary>
    /// <param name="type">��������</param>
    /// <param name="recName">�������</param>
    /// <param name="func">�ص�����</param>
    public void RemoveRecordCallback(eObjectType type, string recName, RecordChangedFunc func)
    {
        if (!_recordCallbacks.ContainsKey(type) || func == null || string.IsNullOrEmpty(recName))
        {
            return;
        }

        Dictionary<string, SortedList<int, RecordChangedCallbackList>> rec2sortedFuncList = _recordCallbacks[type];
        if (!rec2sortedFuncList.ContainsKey(recName))
        {
            return;
        }

        SortedList<int, RecordChangedCallbackList> sortedFuncList = rec2sortedFuncList[recName];

        foreach (KeyValuePair<int, RecordChangedCallbackList> skv in sortedFuncList)
        {
            foreach (var cb in skv.Value)
            {
                if (cb == func)
                {
                    skv.Value.Remove(cb);
                }
            }
        }
    }

    /// <summary>
    /// ִ�б��ص�
    /// </summary>
    /// <param name="args"></param>
    private void OnRecordChanged(DataList args)
    {
        // ��������
        eObjectType objType = (eObjectType)args.ReadInt(0);
        // uid
        ulong uid = args.ReadULong(1);
        // �����
        string recName = args.ReadString(2);

        if (!_recordCallbacks.ContainsKey(objType))
        {
            // û��ע�����
            return;
        }

        Dictionary<string, SortedList<int, RecordChangedCallbackList>> rec2sortedFuncList = _recordCallbacks[objType];
        if (!rec2sortedFuncList.ContainsKey(recName))
        {
            // û��ע��������
            return;
        }

        // ������ص�
        SortedList<int, RecordChangedCallbackList> sortedFuncList = rec2sortedFuncList[recName];
        foreach (KeyValuePair<int, RecordChangedCallbackList> kv in sortedFuncList)
        {
            foreach (RecordChangedFunc func in kv.Value)
            {
                func?.Invoke(uid, args);
            }
        }
    }
}