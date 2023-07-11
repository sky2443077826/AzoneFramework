using System;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 全局数据管理，游戏内所有的对象创建、修改、移除都是通过此类实现的。
    /// </summary>
    public class ObjectManager : Singleton<ObjectManager>
    {
        // 对象ID的类型字典
        private Dictionary<eObjectType, List<ulong>> _type2uidList;
        // 对象列表
        private Dictionary<ulong, IDataObject> _objects;

        /// <summary>
        /// 属性改变事件
        /// </summary>
        private event Action<DataList> _propChanged;

        /// <summary>
        /// 表格改变事件
        /// </summary>
        private event Action<DataList> _recordChanged;

        /// <summary>
        /// 单例创建
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // 初始化
            _objects = new Dictionary<ulong, IDataObject>();
            _type2uidList = new Dictionary<eObjectType, List<ulong>>();

            // 数据对象池创建
            DataObjectPool.Instance.Create();
            //数据对象工厂
            ObjectFactory.Instance.Create();                    

        }

        /// <summary>
        /// 单例销毁
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            // 释放
            foreach (var iobj in _objects.Values)
            {
                DataObject obj = iobj as DataObject;
                obj.Dispose();
            }

            _objects.Clear();
            _type2uidList.Clear();
            //数据对象工厂
            ObjectFactory.Instance.Dispose();
            // 数据对象池释放
            DataObjectPool.Instance.Dispose();
        }

        /// <summary>
        /// 属性改变回调
        /// </summary>
        /// <param name="args"></param>
        private void OnObjectPropertyChagned(DataList args)
        {
            _propChanged?.Invoke(args);
        }

        /////////////////////////////////////////////////////////////////////////////
        /// 对象集操作
        /////////////////////////////////////////////////////////////////////////////
        #region 对象集
        public bool AddObject(IDataObject dataObject)
        {
            if (dataObject == null) return false;
            // 获取类型
            eObjectType type = (eObjectType)dataObject.GetInt("Type");
            // 获取uid
            ulong uid = dataObject.GetULong("UID");

            if (HasObject(uid))
            {
                GameLog.Error($"重复的UID，添加对象失败。{dataObject.GetInt("ID")}");
                return false;
            }

            // 添加类型映射
            if (!_type2uidList.TryGetValue(type, out List<ulong> uidlist))
            {
                uidlist = new List<ulong>();
                _type2uidList.Add(type, uidlist);
            }
            uidlist.Add(uid);
            // 添加对象
            _objects.Add(uid, dataObject);

            return true;
        }

        /// <summary>
        /// 根据uid获取指定对象类型
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public eObjectType GetType(ulong uid)
        {
            if (!_objects.TryGetValue(uid, out IDataObject iobj) || iobj == null)
            {
                return eObjectType.None;
            }

            return iobj.Type;
        }

        /// <summary>
        /// 获取指定uid的配置ID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int GetConfig(ulong uid)
        {
            if (!_objects.TryGetValue(uid, out IDataObject iobj) || iobj == null) { return 0; }

            return iobj.ConfigID;
        }

        /// <summary>
        /// 根据配置ID创建指定类型的对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IDataObject CreateObjectByConfig(int config)
        {
            if (!ConfigManager.Instance.HasConfig(config))
            {
                return null;
            }

            // 判断类型
            eObjectType type = ConfigManager.Instance.GetType(config);
            if (!Enum.IsDefined(typeof(eObjectType), type))
            {
                return null;
            }

            // 命名空间名
            // string nameSpaceName = GetType().Namespace;
            // 类名
            string className;

            // 解析子类型
            switch (type)
            {
                case eObjectType.ViewPort:
                    {
                        // 获取子类型
                        eViewPort subType = (eViewPort)ConfigManager.Instance.GetConfigInt(config, "ViewType");
                        // 获取类名
                        className = subType.ToString();
                    }
                    break;

                default:
                    className = "DataObject";
                    break;
            }

            // 创建对象
            string fullName = GetType().Namespace + "." + className;
            Type classType = System.Type.GetType(fullName);
            return Activator.CreateInstance(classType) as IDataObject;
        }

        /// <summary>
        /// 创建一个数据对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ulong CreateObject(int config)
        {
            if (!ConfigManager.Instance.HasConfig(config))
            {
                return 0;
            }

            ulong uid = DataUtility.GenerateUID(config);
            if (_objects.ContainsKey(uid))
            {
                return 0;
            }

            // 1.根据config创建对象
            IDataObject dataObject = CreateObjectByConfig(config);
            if (dataObject == null)
            {
                return 0;
            }

            // 2. 初始化对象
            if (!dataObject.Init(config))
            {
                DataObjectPool.Instance.ReleaseObject(dataObject);
                return 0;
            }

            dataObject.SetULong("UID", uid);
            dataObject.UID = uid;

            // 3.添加类型uid映射
            AddObject(dataObject);

            // 4. 添加属性回调函数
            dataObject.SetPropChangedCallback(OnObjectPropertyChagned);

            return uid;
        }

        /// <summary>
        /// 创建一个数据对象
        /// </summary>
        public ulong CreateObject<T>(int config) where T : class, IDataObject, new()
        {
            if (!ConfigManager.Instance.HasConfig(config))
            {
                return 0;
            }

            // 判断类型
            eObjectType type = ConfigManager.Instance.GetType(config);
            if (!Enum.IsDefined(typeof(eObjectType), type))
            {
                return 0;
            }

            /************************************************************************/
            // 顺序不能错                                                             
            /************************************************************************/
            // 1. 生成并且设置uid
            ulong uid = DataUtility.GenerateUID(config);
            if (_objects.ContainsKey(uid))
            {
                return 0;
            }

            // 创建对象
            T dataObject = DataObjectPool.Instance.FetchObject<T>(config);
            if (dataObject == null)
            {
                return 0;
            }

            // 2. 初始化对象
            if (!dataObject.Init(config))
            {
                DataObjectPool.Instance.ReleaseObject(dataObject);
                return 0;
            }

            dataObject.SetULong("UID", uid);
            dataObject.UID = uid;

            // 3.添加类型uid映射
            AddObject(dataObject);

            // 4. 添加属性回调函数
            dataObject.SetPropChangedCallback(OnObjectPropertyChagned);

            return uid;
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool DestoryObject(ulong uid)
        {
            if (!_objects.ContainsKey(uid))
            {
                return false;
            }

            // 尝试获取数据对象
            if (!_objects.TryGetValue(uid, out IDataObject dataObject))
            {
                return false;
            }

            if (dataObject == null)
            {
                _objects.Remove(uid);
                return false;
            }

            //  获取对象类型
            int val = dataObject.GetInt("Type");
            if (val == 0)
            {
                return false;
            }

            eObjectType type = (eObjectType)val;
            if (!Enum.IsDefined(typeof(eObjectType), type))
            {
                return false;
            }

            // 获取所在地图类型，如果不在场景中，有可能是None(0)
            // int mapType = dataObject.GetInt("Map");

            if (_type2uidList.TryGetValue(type, out List<ulong> uidlist))
            {
                uidlist.Remove(uid);
            }

            DataObjectPool.Instance.ReleaseObject(dataObject);
            _objects.Remove(uid);

            return true;
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public bool DestoryObject(IDataObject dataObject)
        {
            if (dataObject == null)
            {
                return false;
            }

            return DestoryObject(dataObject.UID);
        }

        /// <summary>
        /// 是否包含指定uid对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool HasObject(ulong uid)
        {
            if (_objects == null) { return false; }
            return _objects.ContainsKey(uid);
        }

        /// <summary>
        /// 根据uid获取数据对象
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public bool TryGetObject(ulong uid, out IDataObject dataObject)
        {
            dataObject = null;

            // 是否包含指定uid对象
            if (!_objects.TryGetValue(uid, out dataObject))
            {
                return false;
            }

            return dataObject != null;
        }

        /// <summary>
        /// 根据uid获取指定类型的数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <param name="dataobject"></param>
        /// <returns></returns>
        public bool TryGetObject<T>(ulong uid, out T dataobject) where T : IDataObject
        {
            dataobject = default(T);

            if (!_objects.TryGetValue(uid, out IDataObject iobj))
            {
                return false;
            }

            dataobject = (T)iobj;

            return true;
        }

        /// <summary>
        /// 根据指定类型，获取当前类型下面所有的对象UID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="outlist"></param>
        /// <returns></returns>
        public int GetObjects(eObjectType type, out List<ulong> outlist)
        {
            outlist = null;
            // 类型是否有效
            if (!Enum.IsDefined(typeof(eObjectType), type))
            {
                return 0;
            }

            // 获取指定类型所有的dui
            if (!_type2uidList.TryGetValue(type, out List<ulong> uidlist))
            {
                return 0;
            }

            // 赋值
            outlist = new List<ulong>();
            outlist.AddRange(uidlist);
            return outlist.Count;
        }

        #endregion

        #region 子对象
        /// <summary>
        /// 创建子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="config"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool TryCreateChild(ulong parent, int config, int pos, out ulong uid)
        {
            uid = 0;

            if (!_objects.ContainsKey(parent) || !ConfigManager.Instance.HasConfig(config))
            {
                return false;
            }

            // 获取父对象
            IDataObject parentObject = _objects[parent];
            if (parentObject == null)
            {
                return false;
            }

            uid = parentObject.CreateChild(config, pos);

            return uid != 0;
        }

        /// <summary>
        /// 根据位置移除一个子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool RemoveChildByPos(ulong parent, int pos)
        {
            if (pos < 0 || !_objects.ContainsKey(parent))
            {
                return false;
            }

            ulong childUID = GetChildByPos(parent, pos);
            if (!_objects.ContainsKey(childUID))
            {
                return false;
            }

            // 销毁子对象
            IDataObject dataObject = _objects[childUID];
            if (dataObject == null)
            {
                return false;
            }

            // 销毁自已
            DataObjectPool.Instance.ReleaseObject(dataObject);

            // 移除子对象
            _objects.Remove(childUID);

            return true;
        }

        /// <summary>
        /// 根据uid移除子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool RemoveChildByUID(ulong parent, ulong child)
        {
            if (!_objects.ContainsKey(parent) || !_objects.ContainsKey(child))
            {
                return false;
            }

            IDataObject childObj = _objects[child];
            if (childObj == null)
            {
                return false;
            }

            // 是否存在父对象
            IDataObject parentObj = _objects[parent];
            if (parentObj == null)
            {
                return false;
            }

            // 父对象移除子对象
            if (!parentObj.FindChild(childObj))
            {
                GameLog.Error("没有找到子对象，移除失败");
                return false;
            }

            // 销毁自已
            DataObjectPool.Instance.ReleaseObject(childObj);

            // 对象移除
            _objects.Remove(childObj.UID);

            return true;
        }

        /// <summary>
        /// 获取子对象数量
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public int GetChildCount(ulong parent)
        {
            if (!_objects.ContainsKey(parent))
            {
                return 0;
            }

            IDataObject parentObject = _objects[parent];
            if (parentObject == null)
            {
                return 0;
            }

            return parentObject == null ? 0 : parentObject.ChildCount();
        }

        /// <summary>
        /// 获取子对象在父容器中的位置
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int GetPosInParent(ulong uid)
        {
            if (!_objects.ContainsKey(uid) || !_objects.TryGetValue(uid, out IDataObject iobj))
            {
                return 0;
            }

            if (iobj == null)
            {
                return 0;
            }

            DataObject obj = iobj as DataObject;
            if (obj == null)
            {
                return 0;
            }

            return obj.Pos;
        }

        /// <summary>
        /// 获取子对象的父对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ulong GetParent(ulong uid)
        {
            if (!_objects.ContainsKey(uid) || !_objects.TryGetValue(uid, out IDataObject iobj))
            {
                return 0;
            }

            DataObject obj = iobj as DataObject;
            if (obj == null)
            {
                return 0;
            }

            return obj.Parent;
        }

        /// <summary>
        /// 交换两个子对象位置
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool SwapChild(ulong parent, int src, int dest)
        {
            if (!_objects.ContainsKey(parent) || !_objects.TryGetValue(parent, out IDataObject iobj))
            {
                return false;
            }

            DataObject parnetObject = iobj as DataObject;
            if (parnetObject == null)
            {
                return false;
            }

            return parnetObject.Swap(src, dest);
        }

        /// <summary>
        /// 根据位置获取子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public ulong GetChildByPos(ulong parent, int pos)
        {
            if (!_objects.ContainsKey(parent) || !_objects.TryGetValue(parent, out IDataObject iobj))
            {
                return 0;
            }

            DataObject parnetObject = iobj as DataObject;
            if (parnetObject == null)
            {
                return 0;
            }

            return parnetObject.GetChild(pos);
        }

        /// <summary>
        /// 根据config获取第一个满足条件的对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public ulong GetFirstChildByConfig(ulong parent, int config)
        {
            if (ConfigManager.Instance.HasConfig(config) || !_objects.ContainsKey(parent) || !_objects.TryGetValue(parent, out IDataObject iobj))
            {
                return 0;
            }

            if (iobj == null)
            {
                return 0;
            }

            DataObject parentObject = iobj as DataObject;
            if (parentObject == null)
            {
                return 0;
            }

            // 遍历查找
            for (int index = 0; index < parentObject.Capacity; ++index)
            {
                ulong uid = parentObject.GetChild(index);
                if (uid == 0)
                {
                    continue;
                }

                if (!_objects.ContainsKey(uid))
                {
                    continue;
                }

                DataObject childObject = _objects[uid] as DataObject;
                if (childObject == null)
                {
                    continue;
                }

                if (childObject.ConfigID == config)
                {
                    return uid;
                }
            }

            return 0;
        }

        /// <summary>
        /// 根据名称获取指定类型的子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ulong GetFirstChildByName(ulong parent, string name)
        {
            if (string.IsNullOrEmpty(name) || !_objects.ContainsKey(parent) || !_objects.TryGetValue(parent, out IDataObject iobj))
            {
                return 0;
            }

            if (iobj == null)
            {
                return 0;
            }

            DataObject parentObject = iobj as DataObject;
            if (parentObject == null)
            {
                return 0;
            }

            // 遍历查找
            for (int index = 0; index < parentObject.Capacity; ++index)
            {
                ulong uid = parentObject.GetChild(index);
                if (uid == 0)
                {
                    continue;
                }

                if (!_objects.ContainsKey(uid))
                {
                    continue;
                }

                DataObject childObject = _objects[uid] as DataObject;
                if (childObject == null)
                {
                    continue;
                }

                if (childObject.GetString("Name") == name)
                {
                    return uid;
                }
            }

            return 0;
        }

        /// <summary>
        /// 根据config获取所有的子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="config"></param>
        /// <param name="childlist"></param>
        /// <returns></returns>
        public int GetChildrenByConfig(ulong parent, int config, out List<ulong> childlist)
        {
            childlist = new List<ulong>();

            if (ConfigManager.Instance.HasConfig(config) || !_objects.ContainsKey(parent) || !_objects.TryGetValue(parent, out IDataObject iobj))
            {
                return 0;
            }

            DataObject parentObject = iobj == null ? null : iobj as DataObject;
            if (parentObject == null)
            {
                return 0;
            }

            // 遍历查找
            for (int index = 0; index < parentObject.Capacity; ++index)
            {
                ulong uid = parentObject.GetChild(index);
                if (uid == 0)
                {
                    continue;
                }

                if (!_objects.ContainsKey(uid))
                {
                    continue;
                }

                DataObject childObject = _objects[uid] as DataObject;
                if (childObject == null)
                {
                    continue;
                }

                if (childObject.ConfigID == config)
                {
                    childlist.Add(childObject.GetULong("UID"));
                }
            }

            return childlist.Count;
        }

        /// <summary>
        /// 根据名称获取所有的子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="config"></param>
        /// <param name="childlist"></param>
        /// <returns></returns>
        public int GetChildrenByName(ulong parent, string name, out List<ulong> childlist)
        {
            childlist = new List<ulong>();

            if (string.IsNullOrEmpty(name) || !_objects.ContainsKey(parent) || !_objects.TryGetValue(parent, out IDataObject iobj))
            {
                return 0;
            }

            DataObject parentObject = iobj == null ? null : iobj as DataObject;
            if (parentObject == null)
            {
                return 0;
            }

            // 遍历查找
            for (int index = 0; index < parentObject.Capacity; ++index)
            {
                ulong uid = parentObject.GetChild(index);
                if (uid == 0)
                {
                    continue;
                }

                if (!_objects.ContainsKey(uid))
                {
                    continue;
                }

                DataObject childObject = _objects[uid] as DataObject;
                if (childObject == null)
                {
                    continue;
                }

                if (childObject.GetString("Name") == name)
                {
                    childlist.Add(childObject.GetULong("UID"));
                }
            }

            return childlist.Count;
        }

        /// <summary>
        /// 根据类型获取所有满足条件的子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="type"></param>
        /// <param name="childlist"></param>
        /// <returns></returns>
        public int GetChildrenByType(ulong parent, eObjectType type, out List<ulong> childlist)
        {
            childlist = new List<ulong>();

            if (!_objects.ContainsKey(parent) || !_objects.TryGetValue(parent, out IDataObject iobj))
            {
                return 0;
            }

            DataObject parentObject = iobj == null ? null : iobj as DataObject;
            if (parentObject == null)
            {
                return 0;
            }

            // 遍历查找
            for (int index = 0; index < parentObject.Capacity; ++index)
            {
                ulong uid = parentObject.GetChild(index);
                if (uid == 0)
                {
                    continue;
                }

                if (!_objects.ContainsKey(uid))
                {
                    continue;
                }

                DataObject childObject = _objects[uid] as DataObject;
                if (childObject == null)
                {
                    continue;
                }

                if (childObject.GetInt("Type") == (int)type)
                {
                    childlist.Add(childObject.GetULong("UID"));
                }
            }

            return childlist.Count;
        }

        /// <summary>
        /// 获取所有的子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childlist"></param>
        /// <returns></returns>
        public int GetChildren(ulong parent, out List<ulong> childlist)
        {
            childlist = new List<ulong>();

            if (!_objects.ContainsKey(parent) || !_objects.TryGetValue(parent, out IDataObject iobj))
            {
                return 0;
            }

            DataObject parentObject = iobj == null ? null : iobj as DataObject;
            if (parentObject == null)
            {
                return 0;
            }

            // 遍历查找
            for (int index = 0; index < parentObject.Capacity; ++index)
            {
                ulong uid = parentObject.GetChild(index);
                if (uid == 0)
                {
                    continue;
                }

                if (!_objects.ContainsKey(uid))
                {
                    continue;
                }

                DataObject childObject = _objects[uid] as DataObject;
                if (childObject == null)
                {
                    continue;
                }

                childlist.Add(childObject.GetULong("UID"));
            }

            return childlist.Count;
        }

        #endregion

        #region 属性操作

        /// <summary>
        /// 检查属性是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasProperty(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || string.IsNullOrEmpty(name))
            {
                return false;
            }

            return obj.HasProperty(name);
        }

        /// <summary>
        /// 获取指定属性的类型
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public eDataType GetPropertyType(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || string.IsNullOrEmpty(name))
            {
                return eDataType.Invalid;
            }

            return obj.GetPropertyType(name);
        }

        /// <summary>
        /// 设置int类型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetInt(ulong uid, string name, int val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetInt(name) != val) { obj.SetInt(name, val); }
            return true;
        }

        /// <summary>
        /// 设置ulong类型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetULong(ulong uid, string name, ulong val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetULong(name) != val) { obj.SetULong(name, val); }
            return true;
        }

        /// <summary>
        /// 设置long型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetLong(ulong uid, string name, long val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetLong(name) != val) { obj.SetLong(name, val); }
            return true;
        }

        /// <summary>
        /// 设置float类型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetFloat(ulong uid, string name, float val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetFloat(name) != val) { obj.SetFloat(name, val); }
            return true;
        }

        /// <summary>
        /// 设置double类型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetDouble(ulong uid, string name, double val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetDouble(name) != val) { obj.SetDouble(name, val); }
            return true;
        }

        /// <summary>
        /// 设置string类型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetString(ulong uid, string name, string val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetString(name) != val) { obj.SetString(name, val); }
            return true;
        }

        /// <summary>
        /// 设置bool类型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetBool(ulong uid, string name, bool val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetBool(name) != val) { obj.SetBool(name, val); }
            return true;
        }

        /// <summary>
        /// 设置vector2类型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetVector2(ulong uid, string name, Vector2 val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetVector2(name) != val) { obj.SetVector2(name, val); }
            return true;
        }

        /// <summary>
        /// 设置vector3类型属性
        /// </summary>
        /// <param name="uid">指定uid</param>
        /// <param name="name">属性名字</param>
        /// <param name="val">属性值</param>
        /// <returns></returns>
        public bool SetVector3(ulong uid, string name, Vector3 val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetVector3(name) != val) { obj.SetVector3(name, val); }
            return true;
        }

        /// <summary>
        /// 获取指定类型的int类型
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetInt(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetInt(name);
        }

        /// <summary>
        /// 获取ulong属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ulong GetULong(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetULong(name);
        }

        /// <summary>
        /// 获取long属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public long GetLong(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetLong(name);
        }

        /// <summary>
        /// 获取 float 属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public float GetFloat(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetFloat(name);
        }

        /// <summary>
        /// 获取 double 属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public double GetDouble(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetDouble(name);
        }

        /// <summary>
        /// 获取 bool 属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetBool(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetBool(name);
        }

        /// <summary>
        /// 获取 string 属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetString(name);
        }

        /// <summary>
        /// 获取 vector2 属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Vector2 GetVector2(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetVector2(name);
        }

        /// <summary>
        /// 获取 vector3 属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Vector3 GetVector3(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            return obj.GetVector3(name);
        }

        /// <summary>
        /// 根据指定类型获取int类型，并且判定是否获得成功
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryGetInt(ulong uid, string name, out int val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetInt(name);
            return true;
        }

        public bool TryGetULong(ulong uid, string name, out ulong val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetULong(name);
            return true;
        }

        public bool TryGetLong(ulong uid, string name, out long val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetLong(name);
            return true;
        }

        public bool TryGetFloat(ulong uid, string name, out float val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetFloat(name);
            return true;
        }

        public bool TryGetDouble(ulong uid, string name, out double val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetDouble(name);
            return true;
        }

        public bool TryGetString(ulong uid, string name, out string val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetString(name);
            return true;
        }

        public bool TryGetBool(ulong uid, string name, out bool val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetBool(name);
            return true;
        }

        public bool TryGetVector2(ulong uid, string name, out Vector2 val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetVector2(name);
            return true;
        }

        public bool TryGetVector3(ulong uid, string name, out Vector3 val)
        {
            val = default;
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            val = obj.GetVector3(name);
            return true;
        }

        /// <summary>
        /// 监听属性改变事件
        /// </summary>
        public void ListenPropertyChanged(Action<DataList> action)
        {
            // 保证监听对象唯一
            _propChanged -= action;
            _propChanged += action;
        }

        /// <summary>
        /// 移除属性改变事件
        /// </summary>
        public void CancelPropertyChanged(Action<DataList> action)
        {
            _propChanged -= action;
        }

        #endregion

        #region 临时属性
        /// <summary>
        /// 是否存在临时数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasData(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            return obj.HasData(name);
        }

        /// <summary>
        /// 获取指定属性值的类型
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public eDataType GetDataType(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return eDataType.Invalid;
            }

            return obj.GetDataType(name);
        }

        /// <summary>
        /// 添加一个临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool AddData(ulong uid, string name, eDataType type)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否是有效的类型
            if (!Enum.IsDefined(typeof(eDataType), type))
            {
                return false;
            }

            // 是否已经存在了
            if (obj.HasData(name))
            {
                return false;
            }

            return obj.AddData(name, type);
        }

        /// <summary>
        /// 移除一个临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveData(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.RemoveData(name);
        }

        /// <summary>
        /// 设置一个int类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataInt(ulong uid, string name, int val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataInt(name, val);
        }

        /// <summary>
        /// 设置一个ulong类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataULong(ulong uid, string name, ulong val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataULong(name, val);
        }

        /// <summary>
        /// 设置一个long类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataLong(ulong uid, string name, long val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataLong(name, val);
        }

        /// <summary>
        /// 设置一个float类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataFloat(ulong uid, string name, float val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataFloat(name, val);
        }

        /// <summary>
        /// 设置double类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataDouble(ulong uid, string name, double val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataDouble(name, val);
        }

        /// <summary>
        /// 设置string类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataString(ulong uid, string name, string val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataString(name, val);
        }

        /// <summary>
        /// 设置bool类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataBool(ulong uid, string name, bool val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataBool(name, val);
        }

        /// <summary>
        /// 设置vector2类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataVector2(ulong uid, string name, Vector2 val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataVector2(name, val);
        }

        /// <summary>
        /// 设置vector3类型的临时属性
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataVector3(ulong uid, string name, Vector3 val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataVector3(name, val);
        }

        /// <summary>
        /// 获取指定类型的int类型
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetDataInt(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataInt(name);
        }

        public ulong GetDataULong(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataULong(name);
        }

        public long GetDataLong(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataLong(name);
        }

        public float GetDataFloat(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataFloat(name);
        }

        public double GetDataDouble(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataDouble(name);
        }

        public bool GetDataBool(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataBool(name);
        }

        public string GetDataString(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataString(name);
        }

        public Vector2 GetDataVector2(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataVector2(name);
        }

        public Vector3 GetDataVector3(ulong uid, string name)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null || string.IsNullOrEmpty(name))
            {
                return default;
            }

            // 是否存在临时数据
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataVector3(name);
        }

        #endregion

        #region 表格操作

        /// <summary>
        /// 检查表格是否存在
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <returns></returns>
        public bool HasRecord(ulong UID, string recName)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            return obj.HasRecord(recName);
        }

        /// <summary>
        /// 根据表格名获取表格
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <returns></returns>
        public Record GetRecord(ulong UID, string recName)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return null;
            }

            if (!obj.HasRecord(recName))
            {
                return null;
            }

            return obj.GetRecord(recName);
        }

        /// <summary>
        /// 设置表格整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool SetRecordInt(ulong UID, string recName, int value, int row, int col)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int oldValue = record.GetInt(row, col);
            if (!record.SetInt(value, row, col))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, col, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 设置表格无符号长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordULong(ulong UID, string recName, ulong value, int row, int colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            ulong oldValue = record.GetULong(row, colTag);
            if (!record.SetULong(value, row, colTag))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, colTag, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 设置表格长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordLong(ulong UID, string recName, long value, int row, int colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            long oldValue = record.GetLong(row, colTag);
            if (!record.SetLong(value, row, colTag))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, colTag, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 设置表格单精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool SetRecordFloat(ulong UID, string recName, float value, int row, int col)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            float oldValue = record.GetFloat(row, col);
            if (!record.SetFloat(value, row, col))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, col, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 设置表格双精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool SetRecordDouble(ulong UID, string recName, double value, int row, int col)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            double oldValue = record.GetDouble(row, col);
            if (!record.SetDouble(value, row, col))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, col, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 设置表格布尔型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool SetRecordBool(ulong UID, string recName, bool value, int row, int col)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            bool oldValue = record.GetBool(row, col);
            if (!record.SetBool(value, row, col))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, col, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 设置表格string型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool SetRecordString(ulong UID, string recName, string value, int row, int col)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            string oldValue = record.GetString(row, col);
            if (!record.SetString(value, row, col))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, col, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 设置表格二维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool SetRecordVec2(ulong UID, string recName, Vector2 value, int row, int col)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            Vector2 oldValue = record.GetVector2(row, col);
            if (!record.SetVector2(value, row, col))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, col, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 设置表格三维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool SetRecordVec3(ulong UID, string recName, Vector3 value, int row, int col)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            Vector3 oldValue = record.GetVector3(row, col);
            if (!record.SetVector3(value, row, col))
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, col, eRecordOperation.Change, oldValue);
            return true;
        }

        /// <summary>
        /// 获取表格整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public int GetRecordInt(ulong UID, string recName, int row, int col)
        {
            int value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetInt(row, col);
        }

        /// <summary>
        /// 获取表格无符号长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public ulong GetRecordULong(ulong UID, string recName, int row, int col)
        {
            ulong value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetULong(row, col);
        }


        /// <summary>
        /// 获取表格长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public long GetRecordLong(ulong UID, string recName, int row, int col)
        {
            long value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetLong(row, col);
        }

        /// <summary>
        /// 获取表格单精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public float GetRecordFloat(ulong UID, string recName, int row, int col)
        {
            float value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetFloat(row, col);
        }

        /// <summary>
        /// 获取表格双精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public double GetRecordDouble(ulong UID, string recName, int row, int col)
        {
            double value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetDouble(row, col);
        }

        /// <summary>
        /// 获取表格布尔型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool GetRecordBool(ulong UID, string recName, int row, int col)
        {
            bool value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetBool(row, col);
        }

        /// <summary>
        /// 获取表格字符串型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public string GetRecordString(ulong UID, string recName, int row, int col)
        {
            string value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetString(row, col);
        }

        /// <summary>
        /// 获取表格二维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Vector2 GetRecordVec2(ulong UID, string recName, int row, int col)
        {
            Vector2 value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetVector2(row, col);
        }

        /// <summary>
        /// 获取表格三维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Vector3 GetRecordVec3(ulong UID, string recName, int row, int col)
        {
            Vector3 value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            return record.GetVector3(row, col);
        }

        /// <summary>
        /// 获取表格整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordInt(ulong UID, string recName, int row, int col, out int value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetInt(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格无符号长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordULong(ulong UID, string recName, int row, int col, out ulong value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetULong(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordLong(ulong UID, string recName, int row, int col, out long value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetLong(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格单精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordFloat(ulong UID, string recName, int row, int col, out float value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetFloat(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格双精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordDouble(ulong UID, string recName, int row, int col, out double value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetDouble(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格布尔型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordBool(ulong UID, string recName, int row, int col, out bool value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetBool(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格字符串型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordString(ulong UID, string recName, int row, int col, out string value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetString(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格二维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordVec2(ulong UID, string recName, int row, int col, out Vector2 value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetVector2(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格三维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool TryGetRecordVector3(ulong UID, string recName, int row, int col, out Vector3 value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            value = record.GetVector3(row, col);
            return true;
        }

        /// <summary>
        /// 设置表格整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordInt(ulong UID, string recName, int value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordInt(UID, recName, value, row, col);
        }

        /// <summary>
        /// 设置表格无符号长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordULong(ulong UID, string recName, ulong value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordULong(UID, recName, value, row, col);
        }

        /// <summary>
        /// 设置表格长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordLong(ulong UID, string recName, long value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordLong(UID, recName, value, row, col);
        }

        /// <summary>
        /// 设置表格单精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordFloat(ulong UID, string recName, float value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordFloat(UID, recName, value, row, col);
        }

        /// <summary>
        /// 设置表格双精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordDouble(ulong UID, string recName, double value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordDouble(UID, recName, value, row, col);
        }

        /// <summary>
        /// 设置表格布尔型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordBool(ulong UID, string recName, bool value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordBool(UID, recName, value, row, col);
        }

        /// <summary>
        /// 设置表格string型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordString(ulong UID, string recName, string value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordString(UID, recName, value, row, col);
        }

        /// <summary>
        /// 设置表格二维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordVec2(ulong UID, string recName, Vector2 value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordVec2(UID, recName, value, row, col);
        }

        /// <summary>
        /// 设置表格三维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool SetRecordVec3(ulong UID, string recName, Vector3 value, int row, string colTag)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("设置表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            return SetRecordVec3(UID, recName, value, row, col);
        }

        /// <summary>
        /// 获取表格整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public int GetRecordInt(ulong UID, string recName, int row, string colTag)
        {
            int value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetInt(row, col);
        }

        /// <summary>
        /// 获取表格无符号长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public ulong GetRecordULong(ulong UID, string recName, int row, string colTag)
        {
            ulong value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetULong(row, col);
        }


        /// <summary>
        /// 获取表格长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public long GetRecordLong(ulong UID, string recName, int row, string colTag)
        {
            long value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetLong(row, col);
        }

        /// <summary>
        /// 获取表格单精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public float GetRecordFloat(ulong UID, string recName, int row, string colTag)
        {
            float value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetFloat(row, col);
        }

        /// <summary>
        /// 获取表格双精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public double GetRecordDouble(ulong UID, string recName, int row, string colTag)
        {
            double value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetDouble(row, col);
        }

        /// <summary>
        /// 获取表格布尔型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool GetRecordBool(ulong UID, string recName, int row, string colTag)
        {
            bool value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetBool(row, col);
        }

        /// <summary>
        /// 获取表格字符串型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public string GetRecordString(ulong UID, string recName, int row, string colTag)
        {
            string value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetString(row, col);
        }

        /// <summary>
        /// 获取表格二维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public Vector2 GetRecordVec2(ulong UID, string recName, int row, string colTag)
        {
            Vector2 value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetVector2(row, col);
        }

        /// <summary>
        /// 获取表格三维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public Vector3 GetRecordVec3(ulong UID, string recName, int row, string colTag)
        {
            Vector3 value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return value;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return value;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return value;
            }

            return record.GetVector3(row, col);
        }

        /// <summary>
        /// 获取表格整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordInt(ulong UID, string recName, int row, string colTag, out int value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetInt(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格无符号长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordULong(ulong UID, string recName, int row, string colTag, out ulong value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetULong(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格长整型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordLong(ulong UID, string recName, int row, string colTag, out long value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetLong(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格单精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordFloat(ulong UID, string recName, int row, string colTag, out float value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetFloat(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格双精度浮点型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordDouble(ulong UID, string recName, int row, string colTag, out double value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetDouble(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格布尔型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordBool(ulong UID, string recName, int row, string colTag, out bool value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetBool(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格字符串型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordString(ulong UID, string recName, int row, string colTag, out string value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetString(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格二维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordVec2(ulong UID, string recName, int row, string colTag, out Vector2 value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetVector2(row, col);
            return true;
        }

        /// <summary>
        /// 获取表格三维向量型数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="colTag"></param>
        /// <returns></returns>
        public bool TryGetRecordVector3(ulong UID, string recName, int row, string colTag, out Vector3 value)
        {
            value = default;
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int col = record.GetColumnByTag(colTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return false;
            }

            value = record.GetVector3(row, col);
            return true;
        }


        /// <summary>
        /// 表格添加行数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public bool AddRecordRow(ulong UID, string recName, DataList rowData)
        {
            if (rowData == null)
            {
                return false;
            }

            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            int row = record.AddRow(rowData);

            if (row == -1)
            {
                return false;
            }

            OnRecordChanged(UID, recName, row, -1, eRecordOperation.Add, -1);
            return true;
        }

        /// <summary>
        /// 表格添加多行数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public bool AddRecordRows(ulong UID, string recName, List<DataList> rowDataList)
        {
            if (rowDataList == null)
            {
                return false;
            }

            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            foreach (var rowData in rowDataList)
            {
                int row = record.AddRow(rowData);
                if (row == -1)
                {
                    continue;
                }
                OnRecordChanged(UID, recName, row, -1, eRecordOperation.Add, -1);
            }

            return true;
        }

        /// <summary>
        /// 表格移除行数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public bool RemoveRecordRow(ulong UID, string recName, int row)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            if (!record.RemoveRow(row))
            {
                return false;
            }
            OnRecordChanged(UID, recName, row, -1, eRecordOperation.Del, -1);
            return true;
        }

        /// <summary>
        /// 表格移除多行数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public bool RemoveRecordRows(ulong UID, string recName, List<int> rowList)
        {
            if (rowList == null)
            {
                return false;
            }

            if (!_objects.TryGetValue(UID, out IDataObject obj) || string.IsNullOrEmpty(recName))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            if (!record.RemoveRows(rowList))
            {
                return false;
            }

            foreach (var row in rowList)
            {
                OnRecordChanged(UID, recName, row, -1, eRecordOperation.Del, -1);
            }
            return true;
        }

        /// <summary>
        /// 获取表格整行数据
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="row"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public bool GetRecordRowData(ulong UID, string recName, int row, out DataList rowData)
        {
            rowData = null;
            if (!_objects.TryGetValue(UID, out IDataObject obj))
            {
                return false;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return false;
            }

            rowData = record.GetRowData(row);
            if (rowData == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查找表格符合条件的行
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="column"></param>
        /// <param name="dataVariant"></param>
        /// <param name="rowList"></param>
        /// <returns></returns>
        public int FindRows(ulong UID, string recName, int column, VariableData dataVariant, out List<int> rowList)
        {
            rowList = null;
            int count = 0;
            if (!_objects.TryGetValue(UID, out IDataObject obj))
            {
                return count;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return count;
            }

            return record.FindRows(column, dataVariant, out rowList);
        }

        /// <summary>
        /// 查找表格符合条件的行
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="recName"></param>
        /// <param name="columnTag"></param>
        /// <param name="dataVariant"></param>
        /// <param name="rowList"></param>
        /// <returns></returns>
        public int FindRows(ulong UID, string recName, string columnTag, VariableData dataVariant, out List<int> rowList)
        {
            rowList = null;
            int count = 0;
            if (!_objects.TryGetValue(UID, out IDataObject obj))
            {
                return count;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return count;
            }

            int col = record.GetColumnByTag(columnTag);
            if (col < 0)
            {
                GameLog.Error("读取表格数据失败。---> 表格{recName}不存在行标签{colTag}!");
                return count;
            }

            return record.FindRows(col, dataVariant, out rowList);
        }

        /// <summary>
        /// 获取已使用的行
        /// </summary>
        /// <param name="recName">表格名称</param>
        /// <returns></returns>
        public List<int> GetRecordUsedRows(ulong UID, string recName)
        {
            if (!_objects.TryGetValue(UID, out IDataObject obj))
            {
                return null;
            }

            Record record = obj.GetRecord(recName);
            if (record == null)
            {
                return null;
            }

            return record.GetUsedRows();
        }

        /// <summary>
        /// 监听表格改变事件
        /// </summary>
        public void ListenRecordChanged(Action<DataList> func)
        {
            // 保证监听对象唯一
            _recordChanged -= func;
            _recordChanged += func;
        }

        /// <summary>
        /// 移除表格改变事件
        /// </summary>
        public void CancelRecordChanged(Action<DataList> func)
        {
            _recordChanged -= func;
        }

        /// <summary>
        /// 表格改变回调
        /// </summary>
        private void OnRecordChanged<T>(ulong UID, string recName, int row, int column, eRecordOperation op, T oldValue)
        {
            DataList args = DataList.Get();

            int objectType = GetInt(UID, "Type");
            if (objectType == 0)
            {
                int configID = GetInt(UID, "ID");
                GameLog.Error($"对象表格回调触发失败。---> 对象{configID}的ObjectType为{objectType}！");
                return;
            }

            args.AddULong(UID);
            args.AddString(recName);
            args.AddInt(row);
            args.AddInt(column);
            args.AddInt((int)op);
            args.Add(oldValue);

            // 直接事件回调
            _recordChanged?.Invoke(args);
            DataList.Recyle(args);
        }

        #endregion
    }
}
