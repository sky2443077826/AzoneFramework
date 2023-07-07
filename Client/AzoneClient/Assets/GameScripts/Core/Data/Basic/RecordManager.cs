using AzoneFramework;
using System.Collections.Generic;

/// <summary>
/// 表格管理器
/// </summary>
public class RecordManager
{
    // 表格缓存
    private Dictionary<string, Record> _recordList;

    /// <summary>
    /// 表格数量
    /// </summary>
    public int Count => _recordList.Count;

    public RecordManager()
    {
        _recordList = new Dictionary<string, Record>();
    }

    /// <summary>
    /// 销毁操作
    /// </summary>
    public void Dispose()
    {
        _recordList.Clear();
    }

    /// <summary>
    /// 是否存在一个表格
    /// </summary>
    /// <param name="recordName">表格名称</param>
    /// <returns></returns>
    public bool HasRecord(string recordName) => _recordList.ContainsKey(recordName);

    /// <summary>
    /// 添加一个表格
    /// </summary>
    /// <param name="recordName">表格名</param>
    /// <param name="record">表格</param>
    /// <returns>成功返回true，否则返回false</returns>
    public bool Add(string recordName, Record record)
    {
        // 是否存在表格
        if (_recordList.ContainsKey(recordName))
        {
            GameLog.Error("名为 {0} 的表格已经存在", recordName);
            return false;
        }
        _recordList.Add(recordName, record);
        return true;
    }

    /// <summary>
    /// 移除一个表格
    /// </summary>
    /// <param name="recordName">表格名</param>
    /// <returns>成功返回true，否则返回false</returns>
    public bool Remove(string recordName)
    {
        // 如果不存在
        if (!_recordList.ContainsKey(recordName))
        {
            return false;
        }
        _recordList.Remove(recordName);
        return true;
    }

    /// <summary>
    /// 拷贝属性到其他管理器
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public void CloneTo(ref RecordManager other)
    {
        foreach (KeyValuePair<string, Record> item in _recordList)
        {
            Record record = new Record(item.Value);
            other.Add(item.Key, record);
        }
    }

    /// <summary>
    /// 获取一个表格
    /// </summary>
    /// <param name="name">表格名</param>
    /// <returns>数据类型</returns>
    public Record GetRecord(string name)
    {
        if (!_recordList.ContainsKey(name))
        {
            GameLog.Error("不存在名为 {0} 的表格", name);
            return null;
        }
        return _recordList[name];
    }

    /// <summary>
    /// 获取指定的多个表格
    /// </summary>
    /// <param name="recordNameList">表格名列表</param>
    /// <param name="onlySaved">是否只有需要存储的表格</param>
    /// <returns></returns>
    public int GetRecordList(out List<string> records, bool onlySaved)
    {
        records = new List<string>();
        foreach (var kv in _recordList)
        {
            if (onlySaved)
            {
                if (kv.Value.Save)
                {
                    records.Add(kv.Key);
                }
            }
            else
            {
                // 所有属性
                records.Add(kv.Key);
            }
        }

        return records.Count;
    }
}
