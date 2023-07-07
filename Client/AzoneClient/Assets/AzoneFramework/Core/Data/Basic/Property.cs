using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ��Ϸ������������
    /// </summary>
    public class Property
    {
        // ������
        private readonly string _name;
        // ����ֵ
        private VariableData _data;
        // ����ʱ��
        private long _expire;
        // �Ƿ񱣴�
        private bool _save;
        // �Ƿ�ͬ��
        private bool _sync;
        // �Ƿ���
        private bool _shared;

        public Property(string name, eDataType type)
        {
            _data = new VariableData();
            _data.type = type;
            _name = name;
            _save = false;
            _sync = false;
            _shared = false;
        }

        public Property(Property property)
        {
            _name = property._name;
            _data = property._data;
            _expire = property._expire;
            _save = property._save;
            _sync = property._sync;
            _shared = property._shared;
        }

        /// <summary>
        /// ��ȡ������
        /// </summary>
        public string Name
        {
            get => _name;
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public VariableData Data
        {
            get => _data;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public eDataType Type
        {
            get => _data.type;
        }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public long Expire
        {
            get => _expire;
            set => _expire = value;
        }

        /// <summary>
        /// �Ƿ񱣴�
        /// </summary>
        public bool Save
        {
            get => _save;
            set => _save = value;
        }

        /// <summary>
        /// �Ƿ�ͬ��
        /// </summary>
        public bool Sync
        {
            get => _sync;
            set => _sync = value;
        }

        /// <summary>
        /// �Ƿ���
        /// </summary>
        public bool Shared
        {
            get => _shared;
            set => _shared = value;
        }

        /// <summary>
        /// �Ƿ��ǿ�����
        /// </summary>
        /// <returns></returns>
        public bool IsNull() => _data.type == eDataType.Invalid;

        /// <summary>
        /// ת���ַ���
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _data.ToString();

        #region ����ֵ

        public void SetInt(int value)
        {
            if (_data.type != eDataType.Int)
            {
                GameLog.Error("Invalid data type. Target is int. Name:{0}", _name);
                return;
            }

            _data.SetVal(value);
        }

        public void SetULong(ulong value)
        {
            if (_data.type != eDataType.ULong)
            {
                GameLog.Error("Invalid data type. Target is ulong. Name:{0}", _name);
                return;
            }
            _data.SetVal(value);
        }

        public void SetLong(long value)
        {
            if (_data.type != eDataType.Long)
            {
                GameLog.Error("Invalid data type. Target is long. Name:{0}", _name);
                return;
            }
            _data.SetVal(value);
        }

        public void SetFloat(float value)
        {
            if (_data.type != eDataType.Float)
            {
                GameLog.Error("Invalid data type. Target is float. Name:{0}", _name);
                return;
            }
            _data.SetVal(value);
        }

        public void SetDouble(double value)
        {
            if (_data.type != eDataType.Double)
            {
                GameLog.Error("Invalid data type. Target is double. Name:{0}", _name);
                return;
            }
            _data.SetVal(value);
        }

        public void SetBool(bool value)
        {
            if (_data.type != eDataType.Bool)
            {
                GameLog.Error("Invalid data type. Target is boolean. Name:{0}", _name);
                return;
            }
            _data.SetVal(value);
        }

        public void SetString(string value)
        {
            if (_data.type != eDataType.String)
            {
                GameLog.Error("Invalid data type. Target is string. Name:{0}", _name);
                return;
            }
            _data.SetVal(value);
        }

        public void SetVector2(Vector2 value)
        {
            if (_data.type != eDataType.Vector2)
            {
                GameLog.Error("Invalid data type. Target is vector2. Name:{0}", _name);
                return;
            }
            _data.SetVal(value);
        }

        public void SetVector3(Vector3 value)
        {
            if (_data.type != eDataType.Vector3)
            {
                GameLog.Error("Invalid data type. Target is vector3. Name:{0}", _name);
                return;
            }
            _data.SetVal(value);
        }


        #endregion

        #region ȡֵ

        public int GetInt()
        {
            if (_data.type != eDataType.Int)
            {
                GameLog.Error("Get int failed. Invalid type. {0} - {1}.", _name, _data.type);
                return 0;
            }

            int val = 0;
            _data.GetVal(ref val);
            return val;
        }

        public ulong GetULong()
        {
            if (_data.type != eDataType.ULong)
            {
                GameLog.Error("Get ulong failed. Invalid type. {0} - {1}.", _name, _data.type);
                return 0;
            }
            ulong val = 0;
            _data.GetVal(ref val);
            return val;
        }

        public long GetLong()
        {
            if (_data.type != eDataType.Long)
            {
                GameLog.Error("Get long failed. Invalid type. {0} - {1}.", _name, _data.type);
                return 0;
            }

            long val = 0;
            _data.GetVal(ref val);
            return val;
        }

        public float GetFloat()
        {
            float val = 0.0f;

            if (_data.type != eDataType.Float)
            {
                GameLog.Error("Get float failed. Invalid type. {0} - {1}.", _name, _data.type);
                return val;
            }

            _data.GetVal(ref val);
            return val;
        }

        public double GetDouble()
        {
            double val = 0.0;
            if (_data.type != eDataType.Double)
            {
                GameLog.Error("Get doubl failed. Invalid type. {0} - {1}.", _name, _data.type);
                return val;
            }
            _data.GetVal(ref val);
            return val;
        }

        public bool GetBool()
        {
            bool val = false;
            if (_data.type != eDataType.Bool)
            {
                GameLog.Error("Get bool failed. Invalid type. {0} - {1}.", _name, _data.type);
                return val;
            }
            _data.GetVal(ref val);
            return val;
        }

        public string GetString()
        {
            string val = string.Empty;
            if (_data.type != eDataType.String)
            {
                GameLog.Error("Get string failed. Invalid type. {0} - {1}.", _name, _data.type);
                return val;
            }
            _data.GetVal(ref val);
            return val;
        }

        public Vector2 GetVector2()
        {
            Vector2 val = Vector2.zero;
            if (_data.type != eDataType.Vector2)
            {
                GameLog.Error("Get vector2 failed. Invalid type. {0} - {1}.", _name, _data.type);
                return val;
            }
            _data.GetVal(ref val);
            return val;
        }

        public Vector3 GetVector3()
        {
            Vector3 val = Vector3.zero;
            if (_data.type != eDataType.Vector3)
            {
                GameLog.Error("Get vector3 failed. Invalid type. {0} - {1}.", _name, _data.type);
                return val;
            }
            _data.GetVal(ref val);
            return val;
        }

        internal void SetBool(object p)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
