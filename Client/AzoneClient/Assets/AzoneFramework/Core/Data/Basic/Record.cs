using AzoneFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表格操作
/// </summary>
public enum eRecordOperation
{
    Add = 0,        // 添加
    Del,            // 删除
    Change,         // 改变
}

/// <summary>
/// 表格列信息
/// </summary>
public struct RecordColumnInfo
{
    // 数据类型
    public eDataType type;
    // 标签
    public string tag;
}

/// <summary>
/// 游戏对象表格数据（基类）
/// </summary>
public class Record
{
    // 表格名称
    public string Name { get; }
    // 是否保存
    public bool Save { get; }
    // 当前表格数量
    public int RowCount { get; private set; }
    // 表格最大的数量
    public int MaxRowCount => _datas.GetLength(0);
    // 表格列数
    public int ColumnCount => _columnInfos.Length;

    // 数据存储容器
    protected VariableData[,] _datas;
    // 行信息
    protected RecordColumnInfo[] _columnInfos;
    // 数据行使用状态
    protected bool[] _usedRows;

    public Record(string name, int row, RecordColumnInfo[] columnInfos, bool save)
    {
        if (string.IsNullOrEmpty(name))
        {
            GameLog.Error($"初始化表格{name}失败。---> 表格名称不可以为空值！");
            return;
        }

        Name = name;
        Save = save;

        if (columnInfos == null || columnInfos.Length <= 0)
        {
            GameLog.Error($"初始化表格{name}失败。---> 列设置不合法！");
            return;
        }

        if (row <= 0)
        {
            GameLog.Error($"初始化表格{name}失败。---> 行数不合法！");
            return;
        }

        _datas = new VariableData[row, columnInfos.Length];
        _usedRows = new bool[row];
        _columnInfos = new RecordColumnInfo[columnInfos.Length];
        Array.Copy(columnInfos, _columnInfos, columnInfos.Length);
    }

    public Record(Record other)
    {
        Name = other.Name;
        Save = other.Save;
        _datas = new VariableData[other.MaxRowCount, other.ColumnCount];
        _usedRows = new bool[other.MaxRowCount];
        _columnInfos = new RecordColumnInfo[other.ColumnCount];

        Array.Copy(other._datas, _datas, other._datas.Length);
        Array.Copy(other._usedRows, _usedRows, _usedRows.Length);
        Array.Copy(other._columnInfos, _columnInfos, _columnInfos.Length);
    }

    /// <summary>
    /// 检查列是否有效
    /// </summary>
    /// <returns></returns>
    public bool CheckColumnValid(int column)
    {
        return column >= 0 || column < _datas.GetLength(1);
    }

    /// <summary>
    /// 列是否有效
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool CheckColumnValid(string tag)
    {
        return CheckColumnValid(GetColumnByTag(tag));
    }

    /// <summary>
    /// 是否已满
    /// </summary>
    public bool IsFull()
    {
        return RowCount >= _datas.Rank;
    }

    /// <summary>
    /// 获取列类型
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    public eDataType GetColumnType(int column)
    {
        if (!CheckColumnValid(column))
        {
            return eDataType.Invalid;
        }

        return _columnInfos[column].type;
    }

    /// <summary>
    /// 根据tag获取类型
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public eDataType GetColumnType(string tag)
    {
        int column = GetColumnByTag(tag);
        if (column == -1) return eDataType.Invalid;

        return GetColumnType(column);
    }

    /// <summary>
    /// 根据标签获取列号
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public int GetColumnByTag(string tag)
    {
        for (int i = 0; i < _columnInfos.Length; i++)
        {
            if (_columnInfos[i].tag == tag)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 根据列号获取标签
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public string GetTagByColumn(int column)
    {
        if (!CheckColumnValid(column))
        {
            return null;
        }
        return _columnInfos[column].tag;
    }

    /// <summary>
    /// 数据转为字符串
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public string ToString(int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            return string.Empty;
        }

        return _datas[row, col].ToString();
    }

    /// <summary>
    /// 字符串转为表格数据
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="val"></param>
    public void FromString(int row, int col, string value)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            return;
        }

        // 获取类型
        eDataType type = GetColumnType(col);
        // 获取值
        switch (type)
        {
            case eDataType.Bool:
                {
                    bool res = ConvertUtility.BoolConvert(value);
                    SetBool(res, row, col);
                }
                break;

            case eDataType.Int:
                {
                    int res = ConvertUtility.IntConvert(value);
                    SetInt(res, row, col);
                }
                break;

            case eDataType.Long:
                {
                    long res = ConvertUtility.LongConvert(value);
                    SetLong(res, row, col);
                }
                break;

            case eDataType.ULong:
                {
                    ulong res = ConvertUtility.ULongConvert(value);
                    SetULong(res, row, col);
                }
                break;

            case eDataType.Float:
                {
                    float res = ConvertUtility.FloatConvert(value);
                    SetFloat(res, row, col);
                }
                break;

            case eDataType.Double:
                {
                    double res = ConvertUtility.DoubleConvert(value);
                    SetDouble(res, row, col);
                }
                break;

            case eDataType.String:
                {
                    SetString(value, row, col);
                }
                break;
            case eDataType.Vector2:
                {
                    string[] arr = value.Split(',');
                    if (arr.Length != 2)
                    {
                        GameLog.Error("加载vector2失败，格式错误。{0}", value);
                        break;
                    }
                    Vector2 v = new Vector2(ConvertUtility.FloatConvert(arr[0]), ConvertUtility.FloatConvert(arr[1]));
                    SetVector2(v, row, col);
                }
                break;

            case eDataType.Vector3:
                {
                    string[] arr = value.Split(',');
                    if (arr.Length != 3)
                    {
                        GameLog.Error("加载vector3失败，格式错误。{0}", value);
                        break;
                    }
                    Vector3 v = new Vector3(ConvertUtility.FloatConvert(arr[0]), ConvertUtility.FloatConvert(arr[1]), ConvertUtility.FloatConvert(arr[2]));
                    SetVector3(v, row, col);
                }
                break;
            default:
                {
                    GameLog.Error("Invalid data type.{0}", type);
                    return;
                }
        }
    }

    public void FromString(int row, string tag, string val)
    {
        // 获取行号
        int col = GetColumnByTag(tag);
        if (col == -1)
        {
            return;
        }

        FromString(row, col, val);
    }

    #region 设置值

    public bool SetInt(int value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record int failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.Int)
        {
            GameLog.Error("Set Record int failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    public bool SetULong(ulong value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record ulong failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.ULong)
        {
            GameLog.Error("Set Record ulong failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    public bool SetLong(long value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record long failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.Long)
        {
            GameLog.Error("Set Record long failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    public bool SetFloat(float value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record float failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.Float)
        {
            GameLog.Error("Set Record float failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    public bool SetDouble(double value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record double failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.Double)
        {
            GameLog.Error("Set Record double failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    public bool SetBool(bool value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record bool failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.Bool)
        {
            GameLog.Error("Set Record bool failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    public bool SetString(string value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record string failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.String)
        {
            GameLog.Error("Set Record string failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    public bool SetVector2(Vector2 value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record vector2 failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.Vector2)
        {
            GameLog.Error("Set Record vector2 failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    public bool SetVector3(Vector3 value, int row, int col)
    {
        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Set Record vector3 failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return false;
        }

        if (_columnInfos[col].type != eDataType.Vector3)
        {
            GameLog.Error("Set Record vector3 failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return false;
        }

        _datas[row, col].SetVal(value);
        return true;
    }

    #endregion

    #region 取值
    public int GetInt(int row, int col)
    {
        int val = 0;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record int failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.Int)
        {
            GameLog.Error("Get Record int failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    public ulong GetULong(int row, int col)
    {
        ulong val = 0;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record ulong failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.ULong)
        {
            GameLog.Error("Get Record ulong failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    public long GetLong(int row, int col)
    {
        long val = 0;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record long failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.Long)
        {
            GameLog.Error("Get Record long failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    public float GetFloat(int row, int col)
    {
        float val = 0;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record float failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.Float)
        {
            GameLog.Error("Get Record float failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    public double GetDouble(int row, int col)
    {
        double val = 0;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record double failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.Double)
        {
            GameLog.Error("Get Record double failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    public bool GetBool(int row, int col)
    {
        bool val = false;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record bool failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.Bool)
        {
            GameLog.Error("Get Record bool failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    public string GetString(int row, int col)
    {
        string val = string.Empty;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record string failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.String)
        {
            GameLog.Error("Get Record string failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    public Vector2 GetVector2(int row, int col)
    {
        Vector2 val = Vector2.zero;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record vector2 failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.Vector2)
        {
            GameLog.Error("Get Record vector2 failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    public Vector3 GetVector3(int row, int col)
    {
        Vector3 val = Vector3.zero;

        if (!CheckRowUse(row) || !CheckColumnValid(col))
        {
            GameLog.Error("Get Record vector3 failed. Invalid Row or Col. {0} - {1} - {2}.", Name, row, col);
            return val;
        }

        if (_columnInfos[col].type != eDataType.Vector3)
        {
            GameLog.Error("Get Record vector3 failed. Invalid type. {0} - {1}.", Name, _columnInfos[col].type);
            return val;
        }

        _datas[row, col].GetVal(ref val);
        return val;
    }

    #endregion

    #region 行操作

    /// <summary>
    /// 检查行是否有效
    /// </summary>
    /// <returns></returns>
    public bool CheckRowValid(int row)
    {
        return row >= 0 || row < _datas.GetLength(0);
    }

    /// <summary>
    /// 检查行是否已使用
    /// </summary>
    public bool CheckRowUse(int row)
    {
        if (!CheckRowValid(row))
        {
            return false;
        }

        return _usedRows[row];
    }

    /// <summary>
    /// 设置某行已使用
    /// </summary>
    /// <returns></returns>
    private bool SetRowUse(int row, bool used)
    {
        if (!CheckRowValid(row))
        {
            return false;
        }
        _usedRows[row] = used;
        return true;
    }

    /// <summary>
    /// 添加一个空行
    /// </summary>
    /// <returns></returns>
    public int AddRow()
    {
        int row = -1;
        if (IsFull())
        {
            GameLog.Error($"添加表格失败。表格已满{Name}");
            return -1;
        }

        for (int index = 0; index < MaxRowCount; ++index)
        {
            if (!_usedRows[index])
            {
                row = index;
                break;
            }
        }

        if (row < 0) return -1;
        if (!SetRowUse(row, true)) return -1;

        // 标记为已使用
        RowCount++;
        return row;
    }

    /// <summary>
    /// 添加行数据
    /// </summary>
    /// <param name="DataList">行数据</param>
    /// <returns></returns>
    public int AddRow(DataList DataList)
    {
        int row = -1;
        if (DataList == null)
        {
            GameLog.Error("Add record row failed. Invalid DataList. {0}", Name);
            return -1;
        }

        // 判断表格是否已满
        if (IsFull())
        {
            GameLog.Error("Add record row failed. Record IsFull. {0}", Name);
            return -1;
        }

        // 查找未使用的行
        for (int i = 0; i < MaxRowCount; i++)
        {
            if (!_usedRows[i])
            {
                row = i;
                break;
            }
        }
        // 未找到则返回
        if (row < 0)
        {
            GameLog.Error("Add record row failed. Record Can't find unused row. {0}", Name);
            return -1;
        }

        for (int i = 0; i < DataList.Count; i++)
        {
            // 设置值
            VariableData VariableData = new VariableData();
            switch (_columnInfos[i].type)
            {
                case eDataType.Int:
                    VariableData.SetVal(DataList.ReadByte(i));
                    break;
                case eDataType.Long:
                    VariableData.SetVal(DataList.ReadLong(i));
                    break;
                case eDataType.ULong:
                    VariableData.SetVal(DataList.ReadULong(i));
                    break;
                case eDataType.Float:
                    VariableData.SetVal(DataList.ReadFloat(i));
                    break;
                case eDataType.Double:
                    VariableData.SetVal(DataList.ReadDouble(i));
                    break;
                case eDataType.Bool:
                    VariableData.SetVal(DataList.ReadBool(i));
                    break;
                case eDataType.String:
                    VariableData.SetVal(DataList.ReadString(i));
                    break;
                case eDataType.Vector2:
                    VariableData.SetVal(DataList.ReadVec2(i));
                    break;
                case eDataType.Vector3:
                    VariableData.SetVal(DataList.ReadVec3(i));
                    break;
                default:
                    GameLog.Error("Add record row failed. Column type error! {0} - {1} - {2}", Name, i, _columnInfos[i].type);
                    return -1;
            }

            // 设置值
            _datas[row, i] = VariableData;
        }

        if (!SetRowUse(row, true))
        {
            return -1;
        }
        // 标记为已使用
        RowCount++;
        return row;
    }

    /// <summary>
    /// 移除行数据
    /// </summary>d
    /// <param name="row">行号</param>
    /// <returns></returns>
    public bool RemoveRow(int row)
    {
        // 检查行号是否已使用
        if (!CheckRowUse(row))
        {
            return false;
        }

        for (int i = 0; i < _columnInfos.Length; i++)
        {
            _datas[row, i] = VariableData.None;
        }

        if (!SetRowUse(row, false))
        {
            return false;
        }
        RowCount--;

        return true;
    }

    /// <summary>
    /// 移除多个行数据
    /// </summary>
    /// <param name="rowList">行号列表</param>
    /// <returns></returns>
    public bool RemoveRows(List<int> rowList)
    {
        if (rowList == null)
        {
            return false;
        }

        // 检查行号是否已使用
        bool success = true;
        foreach (var row in rowList)
        {
            if (!RemoveRow(row))
            {
                success = false;
            }
        }
        return success;
    }

    /// <summary>
    /// 获取某行数据
    /// </summary>
    /// <param name="row">行号</param>
    /// <returns></returns>
    public DataList GetRowData(int row)
    {
        if (!CheckRowUse(row))
        {
            return null;
        }

        DataList rowData = DataList.Get();
        for (int i = 0; i < _columnInfos.Length; ++i)
        {
            rowData.Add(_datas[row, i]);
        }
        return rowData;
    }

    /// <summary>
    /// 查找行
    /// </summary>
    /// <param name="column">查找的列号</param>
    /// <param name="VariableData">查找的值</param>
    /// <param name="rowList">查到的结果列表</param>
    /// <returns>查到的总数</returns>
    public int FindRows(int column, VariableData VariableData, out List<int> rowList)
    {
        rowList = null;
        if (!CheckColumnValid(column))
        {
            return 0;
        }

        if (_columnInfos[column].type != VariableData.type)
        {
            return 0;
        }

        for (int i = 0; i < MaxRowCount; i++)
        {
            if (!CheckRowUse(i))
            {
                continue;
            }

            if (_datas[i, column] == VariableData)
            {
                if (rowList == null)
                {
                    rowList = new List<int>();
                }
                rowList.Add(i);
            }
        }
        return rowList.Count;
    }

    /// <summary>
    /// 获取已使用的行
    /// </summary>
    /// <returns></returns>
    public List<int> GetUsedRows()
    {
        if (MaxRowCount <= 0)
        {
            return null;
        }
        List<int> usedRows = new List<int>();
        for (int i = 0; i < _usedRows.GetLength(0); i++)
        {
            if (_usedRows[i])
            {
                usedRows.Add(i);
            }
        }
        return usedRows;
    }

    #endregion
}
