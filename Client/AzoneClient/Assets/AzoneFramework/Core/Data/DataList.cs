using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public enum eDataType
    {
        Invalid = 0,
        Byte,
        SByte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        Bool,
        String,
        Vector2,
        Vector3,
        Object,
    }

    /// <summary>
    /// 可变数据类型
    /// 支持枚举中的所有类型数据
    /// </summary>
    public struct VariableData
    {
        public eDataType type;
        public bool bValue;
        public long lValue;
        public ulong uLValue;
        public double dValue;
        public string strValue;
        public Vector2 vec2;
        public Vector3 vec3;
        public object obj;

        public static VariableData None = new VariableData() { type = eDataType.Invalid };

        #region 构造函数 

        public VariableData(byte value)
        {
            type = eDataType.Byte;
            lValue = 0;
            uLValue = value;
            dValue = 0.0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(sbyte value)
        {
            type = eDataType.SByte;
            lValue = value;
            uLValue = 0;
            dValue = 0.0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(short value)
        {
            type = eDataType.Short;
            lValue = value;
            uLValue = 0;
            dValue = 0.0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(ushort value)
        {
            type = eDataType.UShort;
            lValue = 0;
            uLValue = value;
            dValue = 0.0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(int value)
        {
            type = eDataType.Int;
            lValue = value;
            uLValue = 0;
            dValue = 0.0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(uint value)
        {
            type = eDataType.UInt;
            lValue = 0;
            uLValue = value;
            dValue = 0.0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(long value)
        {
            type = eDataType.Long;
            lValue = value;
            dValue = 0.0;
            uLValue = 0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(ulong value)
        {
            type = eDataType.ULong;
            lValue = 0;
            uLValue = value;
            dValue = 0.0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(float value)
        {
            type = eDataType.Float;
            lValue = 0;
            uLValue = 0;
            dValue = value;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(double value)
        {
            type = eDataType.Double;
            lValue = 0;
            uLValue = 0;
            dValue = value;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(bool value)
        {
            type = eDataType.Bool;
            lValue = 0;
            uLValue = 0;
            dValue = 0;
            bValue = value;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(string value)
        {
            type = eDataType.String;
            lValue = 0;
            uLValue = 0;
            dValue = 0.0;
            bValue = false;
            if (string.IsNullOrEmpty(value))
                strValue = string.Empty;
            else
                strValue = value;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public VariableData(Vector2 value)
        {
            type = eDataType.Vector2;
            lValue = 0;
            dValue = 0;
            uLValue = 0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = value;
            vec3 = Vector3.zero;
        }

        public VariableData(Vector3 value)
        {
            type = eDataType.Vector3;
            lValue = 0;
            dValue = 0;
            uLValue = 0;
            bValue = false;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = value;
        }

        public VariableData(object value)
        {
            type = eDataType.Object;
            lValue = 0;
            uLValue = 0;
            dValue = 0.0;
            bValue = false;
            strValue = string.Empty;
            obj = value;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        #endregion

        #region 设置值   
        public void SetVal(byte value)
        {
            Reset();
            type = eDataType.Byte;
            uLValue = value;
        }

        public void SetVal(sbyte value)
        {
            Reset();
            type = eDataType.SByte;
            lValue = value;
        }

        public void SetVal(short value)
        {
            Reset();
            type = eDataType.Short;
            lValue = value;
        }

        public void SetVal(ushort value)
        {
            Reset();
            type = eDataType.UShort;
            uLValue = value;
        }

        public void SetVal(int value)
        {
            Reset();
            type = eDataType.Int;
            lValue = value;
        }

        public void SetVal(uint value)
        {
            Reset();
            type = eDataType.UInt;
            uLValue = value;
        }

        public void SetVal(long value)
        {
            Reset();
            type = eDataType.Long;
            lValue = value;
        }

        public void SetVal(ulong value)
        {
            Reset();
            type = eDataType.ULong;
            uLValue = value;
        }

        public void SetVal(float value)
        {
            Reset();
            dValue = value;
            type = eDataType.Float;
        }

        public void SetVal(double value)
        {
            Reset();
            dValue = value;
            type = eDataType.Double;
        }

        public void SetVal(string value)
        {
            Reset();
            type = eDataType.String;
            if (string.IsNullOrEmpty(value))
                strValue = string.Empty;
            else
                strValue = value;
        }

        public void SetVal(Vector2 value)
        {
            Reset();
            type = eDataType.Vector2;
            vec2 = value;
        }

        public void SetVal(Vector3 value)
        {
            Reset();
            type = eDataType.Vector3;
            vec3 = value;
        }

        public void SetVal(bool value)
        {
            Reset();
            type = eDataType.Bool;
            bValue = value;
        }

        public void SetVal(object value)
        {
            Reset();
            type = eDataType.Object;
            obj = value;
        }

        #endregion

        #region 获取值
        public void GetVal(ref sbyte val)
        {
            val = (sbyte)uLValue;
        }

        public void GetVal(ref byte val)
        {
            val = (byte)lValue;
        }

        public void GetVal(ref short val)
        {
            val = (short)lValue;
        }

        public void GetVal(ref ushort val)
        {
            val = (ushort)uLValue;
        }

        public void GetVal(ref int val)
        {
            val = (int)lValue;
        }

        public void GetVal(ref uint val)
        {
            val = (uint)uLValue;
        }

        public void GetVal(ref long val)
        {
            val = lValue;
        }

        public void GetVal(ref ulong val)
        {
            val = uLValue;
        }

        public void GetVal(ref string val)
        {
            val = ToString();
        }

        public void GetVal(ref bool val)
        {
            val = bValue;
        }

        public void GetVal(ref float val)
        {
            val = (float)dValue;
        }

        public void GetVal(ref double val)
        {
            val = dValue;
        }

        public void GetVal(ref object val)
        {
            val = obj;
        }

        public void GetVal(ref Vector2 val)
        {
            switch (type)
            {
                case eDataType.Vector2:
                    val = vec2;
                    break;
                case eDataType.Vector3:
                    val = vec3;
                    break;
                default:
                    val = Vector2.zero;
                    break;
            }

            return;
        }

        public void GetVal(ref Vector3 val)
        {
            switch (type)
            {
                case eDataType.Vector2:
                    val = vec2;
                    break;
                case eDataType.Vector3:
                    val = vec3;
                    break;
                default:
                    val = Vector3.zero;
                    break;
            }
            return;
        }

        #endregion

        public void Reset()
        {
            type = eDataType.Invalid;
            lValue = 0;
            uLValue = 0;
            dValue = 0;
            strValue = string.Empty;
            obj = null;
            vec2 = Vector2.zero;
            vec3 = Vector3.zero;
        }

        public override string ToString()
        {
            switch (type)
            {
                case eDataType.SByte:
                case eDataType.Short:
                case eDataType.Int:
                case eDataType.Long:
                    return lValue.ToString();
                case eDataType.Byte:
                case eDataType.UShort:
                case eDataType.UInt:
                case eDataType.ULong:
                    return uLValue.ToString();
                case eDataType.Float:
                case eDataType.Double:
                    return dValue.ToString();
                case eDataType.String:
                    return strValue;
                case eDataType.Vector2:
                    return vec2.ToString();
                case eDataType.Vector3:
                    return vec3.ToString();
                default:
                    return string.Empty;
            }
        }

        public static bool operator ==(VariableData data1, VariableData data2)
        {
            if (data1.type != data2.type)
            {
                return false;
            }

            switch (data1.type)
            {
                case eDataType.Byte:
                case eDataType.Short:
                case eDataType.UShort:
                case eDataType.Int:
                case eDataType.UInt:
                case eDataType.Long:
                case eDataType.ULong:
                    return data1.lValue == data2.lValue;
                case eDataType.Float:
                case eDataType.Double:
                    return data1.dValue == data2.dValue;
                case eDataType.Bool:
                    return data1.bValue == data2.bValue;
                case eDataType.String:
                    return data1.strValue == data2.strValue;
                case eDataType.Vector2:
                    return data1.vec2 == data2.vec2;
                case eDataType.Vector3:
                    return data1.vec3 == data2.vec3;
                case eDataType.Object:
                    return data1.obj.Equals(data2.obj);
                default:
                    return false;
            }
        }

        public static bool operator !=(VariableData data1, VariableData data2)
        {
            return !(data1 == data2);
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// 数据列表
    /// </summary>
    public class DataList
    {
        const int MAX_POOL_COUNT = 128;
        const int MAX_SIZE = 10;

        private VariableData[] _buffer = new VariableData[MAX_SIZE];
        private int _offset;

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 数据池
        /// </summary>
        private static Queue<DataList> _dataPool = new Queue<DataList>();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public static DataList Get()
        {
            DataList list;
            lock (_dataPool)
            {
                if (_dataPool.Count > 0)
                {
                    list = _dataPool.Dequeue();
                }
                else
                {
                    list = new DataList();
                }
            }
            list._offset = 0;
            list.Count = 0;
            return list;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="list"></param>
        public static void Recyle(DataList list)
        {
            if (list == null)
            {
                return;
            }
            list.Dispose();
        }

        /// <summary>
        /// 弃置
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _buffer.Length; i++)
            {
                _buffer[i].Reset();
            }
            lock (_dataPool)
            {
                /* 
                 * 对象池中只允许最大存在MAX个对象，避免造成资源浪费
                */
                if (_dataPool.Count <= MAX_POOL_COUNT)
                {
                    _dataPool.Enqueue(this);
                }
            }
        }

        public bool CheckFull()
        {
            return Count >= MAX_SIZE;
        }

        #region 添加数据

        public DataList AddData(VariableData data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset] = data;
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddSByte(sbyte data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddByte(byte data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddShort(short data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddUShort(ushort data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddInt(int data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].lValue = data;
            _buffer[_offset].type = eDataType.Int;
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddUInt(uint data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddLong(long data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddULong(ulong data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddFloat(float data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddDouble(double data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddBool(bool data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddString(string data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddVector2(Vector2 data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddVector3(Vector3 data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList AddObject(object data)
        {
            if (CheckFull())
            {
                GameLog.Error("数据列表已满，无法继续添加。");
                return this;
            }

            _buffer[_offset].SetVal(data);
            ++_offset;
            ++Count;
            return this;
        }

        public DataList Add(sbyte data)
        {
            return AddSByte(data);
        }

        public DataList Add(byte data)
        {
            return AddByte(data);
        }

        public DataList Add(short data)
        {
            return AddShort(data);
        }

        public DataList Add(ushort data)
        {
            return AddUShort(data);
        }

        public DataList Add(int data)
        {
            return AddInt(data);
        }

        public DataList Add(uint data)
        {
            return AddUInt(data);
        }

        public DataList Add(long data)
        {
            return AddLong(data);
        }

        public DataList Add(ulong data)
        {
            return AddULong(data);
        }

        public DataList Add(float data)
        {
            return AddFloat(data);
        }

        public DataList Add(double data)
        {
            return AddDouble(data);
        }

        public DataList Add(bool data)
        {
            return AddBool(data);
        }

        public DataList Add(string data)
        {
            return AddString(data);
        }

        public DataList Add(Vector2 data)
        {
            return AddVector2(data);
        }

        public DataList Add(Vector3 data)
        {
            return AddVector3(data);
        }

        public DataList Add(VariableData data)
        {
            return AddData(data);
        }

        public DataList Add(object data)
        {
            return AddObject(data);
        }

        /// <summary>
        /// 添加Varlist类型
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public DataList Add(DataList other)
        {
            for (int index = 0; index < other.Count; ++index)
            {
                Add(other._buffer[index]);
            }

            return this;
        }

        /// <summary>
        /// 追加Varlist类型
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public DataList Append(DataList other, int start, int count)
        {
            if (start < 0 || count <= 0)
            {
                return this;
            }

            if (start + count > MAX_SIZE)
            {
                GameLog.Error("数据列表拼接失败！---> 超过长度限制。");
                return this;
            }

            int end = start + count;

            if (end >= other.Count)
            {
                end = other.Count;
            }

            // 已经超过了，从start开始全部添加
            for (int index = start; index < end; ++index)
            {
                Add(other._buffer[index]);
            }

            return this;
        }

        #endregion

        #region 读取数据

        public VariableData ReadVar(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return VariableData.None;
            }

            return _buffer[index];
        }

        public byte ReadByte(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            byte val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public sbyte ReadSByte(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            sbyte val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public short ReadShort(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            short val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public ushort ReadUShort(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            ushort val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public int ReadInt(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            int val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public uint ReadUInt(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            uint val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public long ReadLong(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            long val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public ulong ReadULong(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            ulong val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public float ReadFloat(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            float val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public double ReadDouble(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return 0;
            }
            double val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public string ReadString(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return string.Empty;
            }
            string val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public bool ReadBool(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return false;
            }

            bool val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public object ReadObjectByType(int index, Type type)
        {
            object obj = default;
            _buffer[index].GetVal(ref obj);
            return Convert.ChangeType(obj, type);
        }

        public T ReadObject<T>(int index) where T : class
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return null;
            }
            object obj = default;
            _buffer[index].GetVal(ref obj);
            if (obj != null)
            {
                return obj as T;
            }
            return null;
        }

        public Vector2 ReadVec2(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return Vector2.zero;
            }
            Vector2 val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        public Vector3 ReadVec3(int index)
        {
            if (index < 0 || index >= _buffer.Length)
            {
                return Vector3.zero;
            }
            Vector3 val = default;
            _buffer[index].GetVal(ref val);
            return val;
        }

        #endregion
    }
}
