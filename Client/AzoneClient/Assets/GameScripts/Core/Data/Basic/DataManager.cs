using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 数据管理器
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
        /// 数据数量
        /// </summary>
        /// <returns></returns>
        public int Count() => _dataList.Count();

        /// <summary>
        /// 是否存在一个数据
        /// </summary>
        /// <param name="data_name"></param>
        /// <returns></returns>
        public bool Find(string data_name) => _dataList.ContainsKey(data_name);

        /// <summary>
        /// 添加一个数据
        /// </summary>
        /// <param name="data_name">数据名</param>
        /// <param name="data_val">数据值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool Add(string data_name, Property data_val)
        {
            // 是否存在数据
            if (_dataList.ContainsKey(data_name))
            {
                GameLog.Error("名为 {0} 的属性已经存在", data_name);
                return false;
            }
            _dataList.Add(data_name, data_val);
            return true;
        }

        /// <summary>
        /// 设置一个属性
        /// </summary>
        /// <param name="data_name"></param>
        /// <param name="data_val"></param>
        /// <returns></returns>
        public bool Set(string data_name, Property data_val)
        {
            if (!_dataList.ContainsKey(data_name))
            {
                GameLog.Warning($"名为{data_name}的属性没有找到.");
                return false;
            }

            _dataList[data_name] = data_val;

            return true;
        }

        /// <summary>
        /// 移除一个数据
        /// </summary>
        /// <param name="data_name">数据名</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool Remove(string data_name)
        {
            // 如果不存在
            if (!_dataList.ContainsKey(data_name))
            {
                return false;
            }
            _dataList.Remove(data_name);
            return true;
        }

        /// <summary>
        /// 获取一个数据
        /// </summary>
        /// <param name="name">数据名</param>
        /// <returns>数据类型</returns>
        public Property GetProperty(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return null;
            }
            return _dataList[name];
        }

        /// <summary>
        /// 获取指定属性的类型
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性类型</returns>
        public eDataType GetType(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return eDataType.Invalid;
            }

            return _dataList[name].Type;
        }

        /// <summary>
        /// 属性是否保存
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
        /// 获取属性
        /// </summary>
        /// <param name="propList">属性名称列表</param>
        /// <param name="onlySaved">是否只有需要存储的属性</param>
        /// <returns></returns>
        public int GetPropertyList(ref List<string> propList, bool onlySaved)
        {
            foreach (var kv in _dataList)
            {
                if (onlySaved)
                {
                    if (kv.Value.Save)
                    {
                        // 如果是save标记
                        propList.Add(kv.Key);
                    }
                }
                else
                {
                    // 所有属性
                    propList.Add(kv.Key);
                }
            }

            return propList.Count;
        }


        #region Get接口
        /// <summary>
        /// 获取int类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public int GetInt(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return 0;
            }
            return _dataList[name].GetInt();
        }

        /// <summary>
        /// 获取long类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public long GetLong(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return 0;
            }
            return _dataList[name].GetLong();
        }

        /// <summary>
        /// 获取ulong类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public ulong GetULong(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return 0;
            }
            return _dataList[name].GetULong();
        }

        /// <summary>
        /// 获取float类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public float GetFloat(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return 0.0f;
            }
            return _dataList[name].GetFloat();
        }

        /// <summary>
        /// 获取double类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public double GetDouble(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return 0.0;
            }
            return _dataList[name].GetDouble();
        }

        /// <summary>
        /// 获取bool类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public bool GetBool(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }
            return _dataList[name].GetBool();
        }

        /// <summary>
        /// 获取string类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public string GetString(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return default;
            }
            return _dataList[name].GetString();
        }

        /// <summary>
        /// 获取vector2类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public Vector2 GetVector2(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return Vector2.zero;
            }
            return _dataList[name].GetVector2();
        }

        /// <summary>
        /// 获取vector3类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public Vector3 GetVector3(string name)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return Vector3.zero;
            }
            return _dataList[name].GetVector3();
        }

        #endregion

        #region Set接口
        /// <summary>
        /// 设置int属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetInt(string name, int val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetInt(val);
            return true;
        }

        /// <summary>
        /// 设置long属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetLong(string name, long val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetLong(val);
            return true;
        }

        /// <summary>
        /// 设置ulong属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetULong(string name, ulong val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetULong(val);
            return true;
        }

        /// <summary>
        /// 设置float属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetFloat(string name, float val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetFloat(val);
            return true;
        }

        /// <summary>
        /// 设置doule属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetDouble(string name, double val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetDouble(val);
            return true;
        }

        /// <summary>
        /// 设置bool属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetBool(string name, bool val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetBool(val);
            return true;
        }

        /// <summary>
        /// 设置string属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetString(string name, string val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetString(val);
            return true;
        }

        /// <summary>
        /// 设置vector2属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetVector2(string name, Vector2 val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetVector2(val);
            return true;
        }

        /// <summary>
        /// 设置vector3属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetVector3(string name, Vector3 val)
        {
            if (!_dataList.ContainsKey(name))
            {
                GameLog.Error("不存在名为 {0} 的属性", name);
                return false;
            }

            Property prop = _dataList[name];
            prop.SetVector3(val);
            return true;
        }

        #endregion
    }
}

