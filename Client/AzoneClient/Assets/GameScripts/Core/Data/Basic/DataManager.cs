using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ���ݹ�����
    /// </summary>
    public class DataManager : IDataManager
    {
        protected Dictionary<string, Property> _dataList;

        public DataManager()
        {
            _dataList = new Dictionary<string, Property>();
        }

        public virtual void Dispose()
        {
            _dataList.Clear();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        public int Count() => _dataList.Count();

        /// <summary>
        /// �Ƿ����һ������
        /// </summary>
        /// <param name="data_name"></param>
        /// <returns></returns>
        public bool Find(string data_name) => _dataList.ContainsKey(data_name);

        /// <summary>
        /// ���һ������
        /// </summary>
        /// <param name="data_name">������</param>
        /// <param name="data_val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool Add(string data_name, Property data_val)
        {
            // �Ƿ��������
            if (_dataList.ContainsKey(data_name))
            {
                GameLog.Error("��Ϊ {0} �������Ѿ�����", data_name);
                return false;
            }
            _dataList.Add(data_name, data_val);
            return true;
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        /// <param name="data_name"></param>
        /// <param name="data_val"></param>
        /// <returns></returns>
        public bool Set(string data_name, Property data_val)
        {
            if (!_dataList.ContainsKey(data_name))
            {
                GameLog.Warning($"��Ϊ{data_name}������û���ҵ�.");
                return false;
            }

            _dataList[data_name] = data_val;

            return true;
        }

        /// <summary>
        /// �Ƴ�һ������
        /// </summary>
        /// <param name="data_name">������</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool Remove(string data_name)
        {
            // ���������
            if (!_dataList.ContainsKey(data_name))
            {
                return false;
            }
            _dataList.Remove(data_name);
            return true;
        }

        /// <summary>
        /// ��ȡһ������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>��������</returns>
        public Property GetProperty(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return null;
            }
            return _dataList[name];
        }

        /// <summary>
        /// ��ȡָ�����Ե�����
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>��������</returns>
        public eDataType GetType(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return eDataType.Invalid;
            }

            return _dataList[name].Type;
        }

        /// <summary>
        /// �����Ƿ񱣴�
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsSaved(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                return false;
            }

            return _dataList[name].Save;
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="propList">���������б�</param>
        /// <param name="onlySaved">�Ƿ�ֻ����Ҫ�洢������</param>
        /// <returns></returns>
        public int GetPropertyList(ref List<string> propList, bool onlySaved)
        {
            foreach (var kv in _dataList)
            {
                if (onlySaved)
                {
                    if (kv.Value.Save)
                    {
                        // �����save���
                        propList.Add(kv.Key);
                    }
                }
                else
                {
                    // ��������
                    propList.Add(kv.Key);
                }
            }

            return propList.Count;
        }


        #region Get�ӿ�
        /// <summary>
        /// ��ȡint��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public int GetInt(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return 0;
            }
            return _dataList[name].GetInt();
        }

        /// <summary>
        /// ��ȡlong��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public long GetLong(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return 0;
            }
            return _dataList[name].GetLong();
        }

        /// <summary>
        /// ��ȡulong��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public ulong GetULong(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return 0;
            }
            return _dataList[name].GetULong();
        }

        /// <summary>
        /// ��ȡfloat��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public float GetFloat(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return 0.0f;
            }
            return _dataList[name].GetFloat();
        }

        /// <summary>
        /// ��ȡdouble��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public double GetDouble(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return 0.0;
            }
            return _dataList[name].GetDouble();
        }

        /// <summary>
        /// ��ȡbool��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public bool GetBool(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }
            return _dataList[name].GetBool();
        }

        /// <summary>
        /// ��ȡstring��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public string GetString(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return default;
            }
            return _dataList[name].GetString();
        }

        /// <summary>
        /// ��ȡvector2��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public Vector2 GetVector2(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return Vector2.zero;
            }
            return _dataList[name].GetVector2();
        }

        /// <summary>
        /// ��ȡvector3��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public Vector3 GetVector3(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return Vector3.zero;
            }
            return _dataList[name].GetVector3();
        }

        #endregion

        #region Set�ӿ�
        /// <summary>
        /// ����int����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetInt(string name, int val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetInt(val);
            return true;
        }

        /// <summary>
        /// ����long����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetLong(string name, long val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetLong(val);
            return true;
        }

        /// <summary>
        /// ����ulong����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetULong(string name, ulong val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetULong(val);
            return true;
        }

        /// <summary>
        /// ����float����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetFloat(string name, float val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetFloat(val);
            return true;
        }

        /// <summary>
        /// ����doule����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetDouble(string name, double val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetDouble(val);
            return true;
        }

        /// <summary>
        /// ����bool����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetBool(string name, bool val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetBool(val);
            return true;
        }

        /// <summary>
        /// ����string����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetString(string name, string val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetString(val);
            return true;
        }

        /// <summary>
        /// ����vector2����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetVector2(string name, Vector2 val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetVector2(val);
            return true;
        }

        /// <summary>
        /// ����vector3����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool SetVector3(string name, Vector3 val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("��������Ϊ {0} ������", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetVector3(val);
            return true;
        }

        #endregion
    }
}

