//////////////////////////////////////////////////////////////////////////
/// File:   DataDispather.cs
/// Date:   2023/04/20
/// Desc:   对象数据改变分发
/// Author: z9y
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using AzoneFramework;

// 回调函数列表  function-list
using PropChangedCallbackList = System.Collections.Generic.List<PropChangedFunc>;
using RecordChangedCallbackList = System.Collections.Generic.List<RecordChangedFunc>;

// 属性回调委托类型回调
public delegate void PropChangedFunc(ulong UID, DataList args);
/// <summary>
/// 表格改变回调委托类型
/// </summary>
public delegate void RecordChangedFunc(ulong UID, DataList args);

/// <summary>
/// 回调管理器
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
    /// 注册属性回调
    /// </summary>
    /// <param name="type"></param>
    /// <param name="order"></param>
    /// <param name=""></param>
    public void RegisterPropCallback(eObjectType type, string propName, PropChangedFunc func, int order = 0)
    {
        // 是否注册对象
        if (!_propCallbacks.ContainsKey(type))
        {
            _propCallbacks.Add(type, new Dictionary<string, SortedList<int, PropChangedCallbackList>>());
        }

        // 是否注册属性
        Dictionary<string, SortedList<int, PropChangedCallbackList>> prop2sortedFuncList = _propCallbacks[type];
        if (!prop2sortedFuncList.ContainsKey(propName))
        {
            prop2sortedFuncList.Add(propName, new SortedList<int, PropChangedCallbackList>());
        }

        // 是否注册优先级
        SortedList<int, PropChangedCallbackList> sortedFuncList = prop2sortedFuncList[propName];
        if (!sortedFuncList.ContainsKey(order))
        {
            sortedFuncList.Add(order, new PropChangedCallbackList());
        }

        // 注册到有序列表中
        PropChangedCallbackList funcList = sortedFuncList[order];

        funcList.Add(func);
    }

    /// <summary>
    /// 移除指定类型的属性回调函数
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
    /// 属性回调
    /// </summary>
    /// <param name="args">参数列表</param>
    private void OnPropertyChagned(DataList args)
    {
        // 对象属性
        eObjectType objType = (eObjectType)args.ReadInt(0);
        // uid
        ulong uid = args.ReadULong(1);
        // 属性名
        string propName = args.ReadString(2);

        if (!_propCallbacks.ContainsKey(objType))
        {
            // 没有注册对象
            return;
        }

        Dictionary<string, SortedList<int, PropChangedCallbackList>> prop2sortedFuncList = _propCallbacks[objType];
        if (!prop2sortedFuncList.ContainsKey(propName))
        {
            // 没有注册属性名
            return;
        }

        // 有序属性回调
        SortedList<int, PropChangedCallbackList> sortedFuncList = prop2sortedFuncList[propName];

        // 解析参数
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
    /// 注册表格回调
    /// </summary>
    /// <param name="type">对象类型</param>
    /// <param name="recName">表格名称</param>
    /// <param name="func">回调函数</param>
    /// <param name="order">优先级</param>
    public void RegisterRecordCallback(eObjectType type, string recName, RecordChangedFunc func, int order = 0)
    {
        // 是否注册对象
        if (!_recordCallbacks.ContainsKey(type))
        {
            _recordCallbacks.Add(type, new Dictionary<string, SortedList<int, RecordChangedCallbackList>>());
        }

        // 是否注册属性
        Dictionary<string, SortedList<int, RecordChangedCallbackList>> record2sortedFuncList = _recordCallbacks[type];
        if (!record2sortedFuncList.ContainsKey(recName))
        {
            record2sortedFuncList.Add(recName, new SortedList<int, RecordChangedCallbackList>());
        }

        // 是否注册优先级
        SortedList<int, RecordChangedCallbackList> sortedFuncList = record2sortedFuncList[recName];
        if (!sortedFuncList.ContainsKey(order))
        {
            sortedFuncList.Add(order, new RecordChangedCallbackList());
        }

        // 注册到有序列表中
        RecordChangedCallbackList funcList = sortedFuncList[order];

        funcList.Add(func);
    }

    /// <summary>
    /// 移除表格回调
    /// </summary>
    /// <param name="type">对象类型</param>
    /// <param name="recName">表格名称</param>
    /// <param name="func">回调函数</param>
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
    /// 执行表格回调
    /// </summary>
    /// <param name="args"></param>
    private void OnRecordChanged(DataList args)
    {
        // 对象属性
        eObjectType objType = (eObjectType)args.ReadInt(0);
        // uid
        ulong uid = args.ReadULong(1);
        // 表格名
        string recName = args.ReadString(2);

        if (!_recordCallbacks.ContainsKey(objType))
        {
            // 没有注册对象
            return;
        }

        Dictionary<string, SortedList<int, RecordChangedCallbackList>> rec2sortedFuncList = _recordCallbacks[objType];
        if (!rec2sortedFuncList.ContainsKey(recName))
        {
            // 没有注册属性名
            return;
        }

        // 有序表格回调
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