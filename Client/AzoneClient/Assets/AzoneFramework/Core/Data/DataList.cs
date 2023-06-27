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
    /// 自定义数据类型
    /// </summary>
    public struct CustomData
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

        public static CustomData None = new CustomData() { type = eDataType.Invalid };

        #region 构造函数 

        public CustomData(byte value)
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

        public CustomData(sbyte value)
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

        public CustomData(short value)
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

        public CustomData(ushort value)
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

        public CustomData(int value)
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

        public CustomData(uint value)
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

        public CustomData(long value)
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

        public CustomData(ulong value)
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

        public CustomData(float value)
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

        public CustomData(double value)
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

        public CustomData(bool value)
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

        public CustomData(string value)
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

        public CustomData(Vector2 value)
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

        public CustomData(Vector3 value)
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

        public CustomData(object value)
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

        public static bool operator ==(CustomData data1, CustomData data2)
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

        public static bool operator !=(CustomData data1, CustomData data2)
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

        private CustomData[] _buffer = new CustomData[MAX_SIZE];
        private int offset;

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 数据池
        /// </summary>
        private static Queue<DataList> _customDataPool = new Queue<DataList>();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public static DataList Get()
        {
            DataList list;
            lock (_customDataPool)
            {
                if (_customDataPool.Count > 0)
                {
                    list = _customDataPool.Dequeue();
                }
                else
                {
                    list = new DataList();
                }
            }
            list.offset = 0;
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
            lock (_customDataPool)
            {
                /* 
                 * 对象池中只允许最大存在MAX个对象，避免造成资源浪费
                */
                if (_customDataPool.Count <= MAX_POOL_COUNT)
                {
                    _customDataPool.Enqueue(this);
                }
            }
        }
    }
}
