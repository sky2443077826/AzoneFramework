
using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 游戏对象数据类
    /// </summary>
    public class DataObject : IDataObject
    {
        // 配置ID
        public int ConfigID { get; set; }
        // 唯一ID
        public ulong UID { get; set; }
        // 子对象索引
        public int Pos { get; set; }
        // 父对象
        public ulong Parent { get; set; }
        // 容量
        public int Capacity { get; set; }
        // 对象类型
        public eObjectType Type { get; set; }

        // 属性改变回调
        public ObjectPropChangedCallback PropChagnedCallback { get; set; }

        // 游戏对象属性管理器
        protected PropertyManager _propMgr;
        // 游戏临时数据管理器
        protected DataManager _dataMgr;
        // 游戏对象表格管理器
        protected RecordManager _recMgr;
        // 游戏子对象
        protected ulong[] _childObjects;

        /// <summary>
        /// 初始化数据
        /// </summary>
        public virtual bool Init(int configID)
        {
            if (!ConfigManager.Instance.HasConfig(configID))
            {
                GameLog.Error("没有发现配置ID，初始化数据失败。{0}", configID);
                return false;
            }

            // 类型
            eObjectType type = ConfigManager.Instance.GetType(configID);
            if (type == eObjectType.None || !Enum.IsDefined(typeof(eObjectType), type))
            {
                GameLog.Error($"无效的配置: {configID} -> {type}");
                return false;
            }

            this.ConfigID = configID;

            _propMgr = new PropertyManager();
            _dataMgr = new DataManager();
            _recMgr = new RecordManager();

            // 初始化属性管理器
            if (!ConfigManager.Instance.CloneTo(this.ConfigID, ref _propMgr, ref _recMgr))
            {
                GameLog.Error("拷贝属性和表格失败，初始化对象失败。{0}", this.ConfigID);
                return false;
            }

            // 设置类型
            _propMgr.SetInt("Type", (int)type);
            this.Type = type;

            // 获取容器容量
            this.Capacity = _propMgr.GetInt("Capacity");

            // 创建游戏子对象
            _childObjects = new ulong[Capacity];

            // 父对象中的索引
            this.Pos = -1;

            // 父对象
            this.Parent = 0;

            return true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Dispose()
        {
            // 1.首先销毁子对象
            foreach (ulong obj in _childObjects)
            {
                if (ObjectManager.Instance.HasObject(obj))
                {
                    ObjectManager.Instance.DestoryObject(obj);
                }
            }

            this.ConfigID = 0;
            this.Pos = -1;
            this.Parent = 0;
            this.UID = 0;

            _propMgr?.Dispose();
            _propMgr = null;

            _dataMgr?.Dispose();
            _dataMgr = null;

            _recMgr?.Dispose();
            _recMgr = null;
        }

        /// <summary>
        /// 是否是角色
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRole()
        {
            return false;
        }

        /// <summary>
        /// 设置属性回调
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public void SetPropChangedCallback(ObjectPropChangedCallback func)
        {
            PropChagnedCallback = func;
        }

        /// <summary>
        /// 属性改变会带哦
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="val"></param>
        private void OnPropertyChanged<T>(string name, T val)
        {
            if (PropChagnedCallback == null) { return; }

            DataList args = DataList.Get();
            // 对象类型
            args.AddInt((int)Type);
            // 对象UID
            args.AddULong(UID);
            // 属性名
            args.AddString(name);
            // 旧值
            args.Add(val);

            // 回调
            PropChagnedCallback.Invoke(args);
            args.Dispose();
        }

        #region 游戏子对象    

        /// <summary>
        /// 子对象数量
        /// </summary>
        public int ChildCount()
        {
            int count = 0;
            foreach (ulong child in _childObjects)
            {
                if (child != 0)
                {
                    ++count;
                }
            }

            return count;
        }

        /// <summary>
        /// 检查子对象是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool FindChild(int pos)
        {
            if (pos < 0 || pos >= Capacity)
            {
                return false;
            }

            return _childObjects[pos] != 0;
        }

        /// <summary>
        /// 检查对象是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool FindChild(ulong uid)
        {
            if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject obj))
            {
                return false;
            }

            return FindChild(obj);
        }

        /// <summary>
        /// 检查子对象是否存在
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool FindChild(IDataObject iobj)
        {
            DataObject obj = iobj as DataObject;// (DataObject)iobj;
            if (obj == null)
            {
                return false;
            }

            return FindChild(obj.Pos);
        }

        /// <summary>
        /// 添加一个子对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pos"></param>
        public bool AddChild(IDataObject obj, int pos = -1)
        {
            if (obj == null)
            {
                return false;
            }

            if (pos < 0)
            {
                for (int index = 0; index < _childObjects.Length; ++index)
                {
                    if (_childObjects[index] == 0)
                    {
                        pos = index;
                        break;
                    }
                }
                if (pos == -1)
                {
                    return false;
                }
            }

            _childObjects[pos] = obj.UID;
            return true;
        }

        /// <summary>
        /// 创建子对象
        /// </summary>
        /// <returns></returns>
        public ulong CreateChild(int config, int pos)
        {
            if (ChildCount() > Capacity || !ConfigManager.Instance.HasConfig(config))
            {
                return 0;
            }

            // 查找一个合适的位置
            if (pos < 0)
            {
                for (int index = 0; index < _childObjects.Length; ++index)
                {
                    if (_childObjects[index] == 0)
                    {
                        pos = index;
                        break;
                    }
                }
                if (pos == -1)
                {
                    return 0;
                }
            }

            if (_childObjects[pos] != 0)
            {
                GameLog.Error($"创建子对象失败，位置不合法{pos} -> {config}");
                return 0;
            }

            // 生成数据对象
            ulong uid = ObjectManager.Instance.CreateObject(config);
            if (0 == uid)
            {
                GameLog.Error($"创建dataobject失败。{config}");
                return 0;
            }

            // 获取数据对象
            if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject childObj))
            {
                ObjectManager.Instance.DestoryObject(uid);
                return 0;
            }

            // 设置子对象属性
            childObj.Pos = pos;
            childObj.Parent = this.UID;

            // 添加到父对象
            _childObjects[pos] = uid;

            return uid;
        }

        /// <summary>
        /// 创建子对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public ulong CreateChild<T>(int config, int pos) where T : class, IDataObject, new()
        {
            if (ChildCount() > Capacity || !ConfigManager.Instance.HasConfig(config))
            {
                return 0;
            }

            // 查找一个合适的位置
            if (pos < 0)
            {
                for (int index = 0; index < _childObjects.Length; ++index)
                {
                    if (_childObjects[index] == 0)
                    {
                        pos = index;
                        break;
                    }
                }
                if (pos == -1)
                {
                    return 0;
                }
            }

            if (_childObjects[pos] != 0)
            {
                GameLog.Error($"创建子对象失败，位置不合法{pos} -> {config}");
                return 0;
            }

            // 生成数据对象
            ulong uid = ObjectManager.Instance.CreateObject<T>(config);
            if (0 == uid)
            {
                GameLog.Error($"创建dataobject失败。{config}");
                return 0;
            }

            // 获取数据对象
            if (!ObjectManager.Instance.TryGetObject(uid, out T childObj))
            {
                ObjectManager.Instance.DestoryObject(uid);
                return 0;
            }

            // 设置子对象属性
            childObj.Pos = pos;
            childObj.Parent = this.UID;

            // 添加到父对象
            _childObjects[pos] = uid;

            return uid;
        }

        /// <summary>
        /// 移除指定uid对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool RemoveChild(int pos)
        {
            if (pos < 0 || pos >= _childObjects.Length)
            {
                return false;
            }

            ulong childUID = _childObjects[pos];
            if (childUID == 0)
            {
                return false;
            }

            // 获取子对象
            if (!ObjectManager.Instance.TryGetObject(childUID, out IDataObject child))
            {
                return false;
            }

            if (child == null)
            {
                return false;
            }

            child.Dispose();
            _childObjects[pos] = 0;
            return true;
        }

        /// <summary>
        /// 移除子对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool RemoveChild(IDataObject iobj)
        {
            DataObject obj = iobj as DataObject;
            if (obj == null)
            {
                return false;
            }

            return RemoveChild(obj.Pos);
        }

        /// <summary>
        /// 交换两个子对象位置
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Swap(int src, int dest)
        {
            if (src < 0 || src >= _childObjects.Length)
            {
                return false;
            }

            if (dest < 0 || dest >= _childObjects.Length)
            {
                return false;
            }

            if (_childObjects[src] == 0 && _childObjects[dest] == 0)
            {
                return false;
            }

            ulong srcUID = _childObjects[src];
            ulong destUID = _childObjects[dest];

            // 父对象是否相等
            if (!ObjectManager.Instance.TryGetObject(srcUID, out IDataObject srcObj)
                || !ObjectManager.Instance.TryGetObject(destUID, out IDataObject destObj))
            {
                return false;
            }

            if (srcObj == null || destObj == null)
            {
                return false;
            }

            if (srcObj.Parent != destObj.Parent)
            {
                return false;
            }

            // 交换对象
            _childObjects[src] = destUID;
            _childObjects[dest] = srcUID;

            srcObj.Pos = dest;
            destObj.Pos = src;

            return true;
        }

        /// <summary>
        /// 根据位置获取对象
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public ulong GetChild(int pos)
        {
            if (pos < 0 || pos >= _childObjects.Length)
            {
                return 0;
            }

            // 获取uid
            ulong uid = _childObjects[pos];
            if (uid == 0)
            {
                return 0;
            }

            if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject obj))
            {
                return 0;
            }

            if (obj == null)
            {
                return 0;
            }

            return uid;
        }

        /// <summary>
        /// 根据confiid获取第一个子对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ulong GetFirstChild(int config)
        {
            if (config == 0)
            {
                return 0;
            }

            foreach (ulong uid in _childObjects)
            {
                if (uid == 0)
                {
                    continue;
                }

                // 获取对象
                if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject child))
                {
                    continue;
                }

                if (child == null)
                {
                    continue;
                }

                if (child.GetInt("ID") == config)
                {
                    return uid;
                }
            }

            return 0;
        }

        /// <summary>
        /// 根据类型获取第一个子对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ulong GetFirstChild(eObjectType type)
        {
            if (type == eObjectType.None)
            {
                return 0;
            }

            foreach (ulong uid in _childObjects)
            {
                if (uid == 0)
                {
                    continue;
                }

                // 获取对象
                if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject child))
                {
                    continue;
                }

                if (child == null)
                {
                    continue;
                }

                if ((eObjectType)child.GetInt("Type") == type)
                {
                    return uid;
                }
            }

            return 0;
        }

        /// <summary>
        /// 根据configID获取所有子对象
        /// </summary>
        /// <param name="config"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public int GetChildren(int config, out List<ulong> children)
        {
            children = null;
            if (config == 0)
            {
                return 0;
            }

            foreach (ulong uid in _childObjects)
            {
                if (uid == 0)
                {
                    continue;
                }

                if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject child))
                {
                    continue;
                }

                if (child.GetInt("ID") == config)
                {
                    if (children == null)
                    {
                        children = new List<ulong>();
                    }
                    children.Add(uid);
                }
            }

            return children == null ? 0 : children.Count;
        }

        /// <summary>
        /// 根据类型获取所有子对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public int GetChildren(eObjectType type, out List<ulong> children)
        {
            children = null;
            if (type == eObjectType.None)
            {
                return 0;
            }

            foreach (ulong uid in _childObjects)
            {
                if (uid == 0)
                {
                    continue;
                }

                if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject child))
                {
                    continue;
                }


                if ((eObjectType)child.GetInt("Type") == type)
                {
                    if (children == null)
                    {
                        children = new List<ulong>();
                    }
                    children.Add(uid);
                }
            }

            return children == null ? 0 : children.Count;
        }

        #endregion

        #region 属性管理器

        /// <summary>
        /// 获取需要存储的属性的名字
        /// </summary>
        /// <param name="propList"></param>
        /// <returns></returns>
        public int GetPropertyList(ref List<string> propList, bool onlySaved)
        {
            return _propMgr.GetPropertyList(ref propList, onlySaved);
        }

        /// <summary>
        /// 获取属性类型
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        public eDataType GetPropertyType(string name)
        {
            if (string.IsNullOrEmpty(name) || !_propMgr.Find(name))
            {
                return eDataType.Invalid;
            }

            return _propMgr.GetType(name);
        }

        /// <summary>
        /// 是否存在属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasProperty(string name)
        {
            return _propMgr.Find(name);
        }

        #region  Set操作集

        /// <summary>
        /// 设置int类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetInt(string name, int val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("设置属性失败，没有{0}。", name);
                return;
            }

            int oldVal = _propMgr.GetInt(name);
            if (oldVal == val) { return; }

            _propMgr.SetInt(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// 设置long类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetLong(string name, long val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("设置属性失败，没有{0}。", name);
                return;
            }

            long oldVal = _propMgr.GetLong(name);
            if (oldVal == val) { return; }

            _propMgr.SetLong(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// 设置ulong类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetULong(string name, ulong val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error($"设置属性失败，{ConfigID}没有{name}。");
                return;
            }

            ulong oldVal = _propMgr.GetULong(name);
            if (oldVal == val) { return; }

            _propMgr.SetULong(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// 设置float类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetFloat(string name, float val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("设置属性失败，没有{0}。", name);
                return;
            }

            float oldVal = _propMgr.GetFloat(name);
            if (oldVal == val) { return; }

            _propMgr.SetFloat(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// 设置double类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetDouble(string name, double val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("设置属性失败，没有{0}。", name);
                return;
            }

            double oldVal = _propMgr.GetDouble(name);
            if (oldVal == val) { return; }

            _propMgr.SetDouble(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// 设置string类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetString(string name, string val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("设置属性失败，没有{0}。", name);
                return;
            }

            string oldVal = _propMgr.GetString(name);
            if (oldVal == val) { return; }
            _propMgr.SetString(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// 设置bool类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetBool(string name, bool val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("设置属性失败，没有{0}。", name);
                return;
            }
            bool oldVal = _propMgr.GetBool(name);
            if (oldVal == val) { return; }
            _propMgr.SetBool(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// 设置vector2属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetVector2(string name, Vector2 val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("设置属性失败，没有{0}。", name);
                return;
            }

            Vector2 oldVal = _propMgr.GetVector2(name);
            if (oldVal == val) { return; }
            _propMgr.SetVector2(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// 设置vector3类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        public void SetVector3(string name, Vector3 val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("设置属性失败，没有{0}。", name);
                return;
            }
            Vector3 oldVal = _propMgr.GetVector3(name);
            if (oldVal == val) { return; }
            _propMgr.SetVector3(name, val);
            OnPropertyChanged(name, oldVal);
        }
        #endregion

        #region Get操作集
        /// <summary>
        /// 获取int属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public int GetInt(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return 0;
            }

            return _propMgr.GetInt(name);
        }

        /// <summary>
        /// 获取long属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public long GetLong(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return 0;
            }

            return _propMgr.GetLong(name);
        }

        /// <summary>
        /// 获取ulong属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public ulong GetULong(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return 0;
            }

            return _propMgr.GetULong(name);
        }

        /// <summary>
        /// 获取float属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public float GetFloat(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return 0.0f;
            }

            return _propMgr.GetFloat(name);
        }

        /// <summary>
        /// 获取double属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public double GetDouble(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return 0.0;
            }

            return _propMgr.GetDouble(name);
        }

        /// <summary>
        /// 获取string属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public string GetString(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return default;
            }

            return _propMgr.GetString(name);
        }

        /// <summary>
        /// 获取bool属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public bool GetBool(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return default;
            }
            return _propMgr.GetBool(name);
        }

        /// <summary>
        /// 获取vector属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public Vector2 GetVector2(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return Vector2.zero;
            }

            return _propMgr.GetVector2(name);
        }

        /// <summary>
        /// 获取vector3属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public Vector3 GetVector3(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("获取属性失败，没有{0}。", name);
                return Vector3.zero;
            }

            return _propMgr.GetVector3(name);
        }
        #endregion
        #endregion

        #region 临时属性管理器

        /// <summary>
        /// 是否存在临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasData(string name)
        {
            return _dataMgr.Find(name);
        }

        /// <summary>
        /// 获取临时数据类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public eDataType GetDataType(string name)
        {
            return _dataMgr.GetType(name);
        }

        /// <summary>
        /// 添加一个临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool AddData(string name, eDataType type)
        {
            if (_dataMgr.Find(name))
            {
                GameLog.Error("临时属性已经存在，添加失败。{0}", name);
                return false;
            }

            if (!_dataMgr.Add(name, new Property(name, type)))
            {
                GameLog.Error("添加临时属性失败。{0} -> {1}", name, type);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 移除临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveData(string name)
        {
            if (!_dataMgr.Find(name))
            {
                return false;
            }

            return _dataMgr.Remove(name);
        }

        /// <summary>
        /// 设置一个int类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataInt(string name, int val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetInt(name, val);
        }

        public bool SetDataULong(string name, ulong val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetULong(name, val);
        }

        public bool SetDataLong(string name, long val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetLong(name, val);
        }

        public bool SetDataFloat(string name, float val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetFloat(name, val);
        }

        public bool SetDataDouble(string name, double val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetDouble(name, val);
        }

        public bool SetDataBool(string name, bool val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetBool(name, val);
        }

        public bool SetDataString(string name, string val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetString(name, val);
        }

        public bool SetDataVector2(string name, Vector2 val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetVector2(name, val);
        }

        public bool SetDataVector3(string name, Vector3 val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("设置临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.SetVector3(name, val);
        }

        /// <summary>
        /// 获取一个临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetDataInt(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return 0;
            }

            return _dataMgr.GetInt(name);
        }

        public ulong GetDataULong(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return 0;
            }

            return _dataMgr.GetULong(name);
        }

        public long GetDataLong(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return 0;
            }

            return _dataMgr.GetLong(name);
        }

        public float GetDataFloat(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return 0.0f;
            }

            return _dataMgr.GetFloat(name);
        }

        public double GetDataDouble(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return 0.0;
            }

            return _dataMgr.GetDouble(name);
        }

        public bool GetDataBool(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return false;
            }

            return _dataMgr.GetBool(name);
        }

        public string GetDataString(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return default;
            }

            return _dataMgr.GetString(name);
        }

        public Vector2 GetDataVector2(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return Vector2.zero;
            }

            return _dataMgr.GetVector2(name);
        }


        public Vector3 GetDataVector3(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("获取临时属性失败，没有找到属性{0}", name);
                return Vector3.zero;
            }

            return _dataMgr.GetVector3(name);
        }

        #endregion

        #region 序列化

        /// <summary>
        /// 序列化到xml
        /// </summary>
        /// <returns></returns>
        public virtual bool SerializeToXml(XmlElement root)
        {
            string clsName = _propMgr.GetString("Class");
            if (string.IsNullOrEmpty(clsName))
            {
                GameLog.Error("序列化对象失败，没有找到class。");
                return false;
            }

            if (root == null)
            {
                GameLog.Error($"序列化对象失败，xml根节点为null。Class:{clsName}, ConfigID:{ConfigID}");
                return false;
            }

            // 创建类型节点
            XmlElement node = root.OwnerDocument.CreateElement(clsName);
            if (node == null)
            {
                GameLog.Error($"列化对象失败，创建数据节点失败。Class:{clsName}, ConfigID: {ConfigID}");
                return false;
            }

            /**
             *      序列化属性
             **/
            SerializeProperty(node);

            /**
             *      序列化表格
             **/
            SerializeRecord(node);

            /**
             *      序列化子对象
             **/
            SerializeChildren(node);

            /**
             *      将节点添加到root上
             **/
            root.AppendChild(node);

            return true;
        }

        /// <summary>
        /// 从外部解析
        /// </summary>
        /// <returns></returns>
        public virtual bool ParseFrom(string data)
        {
            if (string.IsNullOrEmpty(data)) { return false; }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);

            XmlElement node = doc.DocumentElement;

            if (node == null)
            {
                return false;
            }

            if (!ConfigManager.Instance.HasClass(node.Name))
            {
                GameLog.Error($"没有找到类型{node.Name}");
                return false;
            }

            /**
             *      解析所有属性
             */
            if (!ParsePropertyFromXML(node))
            {
                GameLog.Error($"解析属性失败{node.Name}");
            }

            /**
             *      解析所有表格
             */

            if (!ParseRecordFromXML(node))
            {
                GameLog.Error($"解析表格失败{node.Name}");
            }

            /**
             *      解析子对象
             */
            if (!ParseChildrenFromXML(node))
            {
                GameLog.Error($"解析子对象失败{node.Name}");
            }
            return true;
        }

        /// <summary>
        /// 解析属性
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool ParsePropertyFromXML(XmlNode node)
        {
            if (node == null)
            {
                return false;
            }
            ConfigManager.Instance.GetClassPropMgr(node.Name, out PropertyManager propMgr);
            if (propMgr == null)
            {
                GameLog.Error($"{node.Name}没有找到属性管理器， 解析存储失败！");
                return false;
            }

            // 2.属性节点是否存在
            XmlNode propNode = node.SelectSingleNode("Property");
            if (propNode == null)
            {
                GameLog.Error($"对象存档异常，没有Property节点。");
                return false;
            }

            // 3.解析所有属性
            foreach (XmlNode childNode in propNode.ChildNodes)
            {
                if (childNode == null)
                {
                    continue;
                }

                // 属性名
                string propName = childNode.Name;
                // 属性值
                string propValue = childNode.InnerText;

                if (string.IsNullOrEmpty(propName))
                {
                    continue;
                }

                // 属性类型
                eDataType propType = propMgr.GetType(propName);
                // 转化属性
                ConfigManager.ConverToProperty(propType, propName, propValue, out Property prop);
                // 设置存储属性
                prop.Save = propMgr.IsSaved(propName);

                // 添加到属性管理器
                _propMgr.Set(propName, prop);
            }

            return true;
        }

        /// <summary>
        /// 解析属性
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool ParseRecordFromXML(XmlNode node)
        {
            if (node == null) return false;

            XmlNode recNode = node.SelectSingleNode("Record");
            if (recNode == null)
            {
                // GameLog.Error($"对象存档异常，没有Record节点。");
                return true;
            }

            // 解析所有表格
            foreach (XmlNode childNode in recNode.ChildNodes)
            {
                if (childNode == null)
                {
                    continue;
                }

                // 表格名
                string recName = childNode.Name;
                if (!_recMgr.HasRecord(recName))
                {
                    // 没有找到表格
                    continue;
                }

                Record rec = _recMgr.GetRecord(recName);
                if (rec == null)
                {
                    continue;
                }

                // 行数据
                foreach (XmlNode rowNode in childNode.ChildNodes)
                {
                    // 添加一行
                    int row = rec.AddRow();
                    if (row < 0) { continue; }
                    // 获取所有的属性值
                    foreach (XmlAttribute columnAttr in rowNode.Attributes)
                    {
                        // 获取属性名
                        string tag = columnAttr.Name;
                        // 获取属性值
                        string val = columnAttr.Value;
                        if (string.IsNullOrEmpty(tag))
                        {
                            GameLog.Error("表格数据错误，tag为空");
                            continue;
                        }

                        // 从字符串中序列到表格数据
                        rec.FromString(row, tag, val);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 解析子节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool ParseChildrenFromXML(XmlNode node)
        {
            if (node == null) return false;
            // string xpath = "./*[not (name() = 'Record') and not (name() = 'Property')]";
            XmlNodeList childNodeList = node.SelectNodes(GameConstant.kParseChildXPath);
            foreach (XmlNode childNode in childNodeList)
            {
                // 获取对象名
                string childClassName = childNode.Name;
                if (string.IsNullOrEmpty(childClassName))
                {
                    continue;
                }

                // 创建子对象
                int childConfigID = Convert.ToInt32(childNode.SelectSingleNode("Property/ID").InnerText);

                ulong childUID = CreateChild(childConfigID, -1);
                // 获取子对象
                if (!ObjectManager.Instance.TryGetObject(childUID, out IDataObject childObject))
                {
                    continue;
                }
                if (childObject == null)
                {
                    GameLog.Error("创建子对象失败。");
                    continue;
                }
                childObject.ParseFrom(childNode.OuterXml);
            }

            return true;
        }

        /// <summary>
        /// 序列化对象属性
        /// </summary>
        /// <param name="rootNode"></param>
        public bool SerializeProperty(XmlElement rootNode)
        {
            List<string> storePropList = new List<string>();
            if (_propMgr.GetPropertyList(ref storePropList, true) == 0)
            {
                // GameLog.Error("列化对象失败，角色没有需要存储的属性，保存属性失败。");
                return false;
            }

            // 创建属性节点
            XmlElement propertyNode = rootNode.OwnerDocument.CreateElement("Property");
            foreach (string propName in storePropList)
            {
                SerializeProperty(propertyNode, propName);
            }
            rootNode.AppendChild(propertyNode);
            return true;
        }

        /// <summary>
        /// 序列化指定属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rootNode"></param>
        public void SerializeProperty(XmlElement rootNode, string propName)
        {
            if (rootNode == null)
            {
                return;
            }

            // 获取属性类型
            eDataType propType = _propMgr.GetType(propName);

            // 创建属性节点
            XmlElement childNode = rootNode.OwnerDocument.CreateElement(propName);
            switch (propType)
            {
                case eDataType.Int:
                    {
                        childNode.InnerText = _propMgr.GetInt(propName).ToString();
                    }
                    break;

                case eDataType.Long:
                    {
                        childNode.InnerText = _propMgr.GetLong(propName).ToString();
                    }
                    break;

                case eDataType.ULong:
                    {
                        childNode.InnerText = _propMgr.GetULong(propName).ToString();
                    }
                    break;

                case eDataType.Float:
                    {
                        childNode.InnerText = _propMgr.GetFloat(propName).ToString();
                    }
                    break;

                case eDataType.Double:
                    {
                        childNode.InnerText = _propMgr.GetDouble(propName).ToString();
                    }
                    break;

                case eDataType.String:
                    {
                        childNode.InnerText = _propMgr.GetString(propName);
                    }
                    break;

                case eDataType.Bool:
                    {
                        childNode.InnerText = _propMgr.GetBool(propName).ToString().ToLowerInvariant();
                    }
                    break;
                case eDataType.Vector2:
                    {
                        childNode.InnerText = _propMgr.GetVector2(propName).x.ToString() + "," + _propMgr.GetVector2(propName).y.ToString();
                    }
                    break;
                case eDataType.Vector3:
                    {
                        // 创建两个子节点  x，y，然后添加
                        childNode.InnerText = _propMgr.GetVector3(propName).x.ToString()
                            + "," + _propMgr.GetVector3(propName).y.ToString()
                            + "," + _propMgr.GetVector3(propName).z.ToString();
                    }
                    break;
                default:
                    {
                        GameLog.Error("保存数据失败，未识别的类型{0} -> {1}", propName, propType);
                        break;
                    }
            }

            rootNode.AppendChild(childNode);
        }

        /// <summary>
        /// 序列化表格
        /// </summary>
        public bool SerializeRecord(XmlElement rootNode)
        {
            // 创建表格节点
            XmlElement recordNode = rootNode.OwnerDocument.CreateElement("Record");
            // 序列化表格逻辑
            if (_recMgr.GetRecordList(out List<string> records, true) != 0)
            {
                // 有需要保存的表格
                foreach (string recName in records)
                {
                    if (string.IsNullOrEmpty(recName))
                    {
                        continue;
                    }

                    Record rec = _recMgr.GetRecord(recName);
                    if (rec == null)
                    {
                        continue;
                    }

                    // 是否是保存的表格
                    if (!rec.Save)
                    {
                        continue;
                    }

                    // 获取表格
                    SerializeRecord(recordNode, rec);
                }
            }

            rootNode.AppendChild(recordNode);

            return true;
        }

        /// <summary>
        /// 序列化表格到xml
        /// </summary>
        /// <param name="recordNode"></param>
        /// <param name="rec"></param>
        public void SerializeRecord(XmlElement recordNode, Record rec)
        {
            if (recordNode == null || rec == null)
            {
                return;
            }

            // 获取表格名
            string recName = rec.Name;
            if (string.IsNullOrEmpty(recName))
            {
                return;
            }

            // 创建节点
            XmlElement node = recordNode.OwnerDocument.CreateElement(recName);
            // 最大行数
            int maxRows = rec.MaxRowCount;

            for (int row = 0; row < maxRows; ++row)
            {
                if (!rec.CheckRowUse(row))
                {
                    continue;
                }

                // 创建行节点
                XmlElement rowNode = node.OwnerDocument.CreateElement("Row");

                for (int column = 0; column < rec.ColumnCount; ++column)
                {
                    // 获取tag名字
                    string tag = rec.GetTagByColumn(column);
                    if (string.IsNullOrEmpty(tag))
                    {
                        continue;
                    }

                    // 字符串值
                    string val = rec.ToString(row, column);

                    // 设置属性
                    rowNode.SetAttribute(tag, val);
                }

                // 添加行节点
                node.AppendChild(rowNode);
            }

            // 添加节点
            recordNode.AppendChild(node);
        }

        /// <summary>
        /// 序列化子对象
        /// </summary>
        /// <param name="rootNode"></param>
        public virtual bool SerializeChildren(XmlElement rootNode)
        {
            if (rootNode == null)
            {
                return false;
            }

            // 获取子节点，然后继续序列化
            foreach (ulong uid in _childObjects)
            {
                if (uid == 0)
                {
                    continue;
                }

                if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject obj))
                {
                    continue;
                }

                if (obj == null)
                {
                    continue;
                }

                obj.SerializeToXml(rootNode);
            }

            return true;
        }

        #endregion

        #region 表格操作

        /// <summary>
        /// 是否存在表格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasRecord(string name)
        {
            return _recMgr.HasRecord(name);
        }

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Record GetRecord(string name)
        {
            return _recMgr.GetRecord(name);
        }

        /// <summary>
        /// 获取需要存储的列表的名字
        /// </summary>
        /// <param name="recList"></param>
        /// <returns></returns>
        public int GetRecordList(out List<string> recList, bool onlySaved)
        {
            return _recMgr.GetRecordList(out recList, onlySaved);
        }

        #endregion
    }
}