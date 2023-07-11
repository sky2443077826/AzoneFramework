using System;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ȫ�����ݹ�����Ϸ�����еĶ��󴴽����޸ġ��Ƴ�����ͨ������ʵ�ֵġ�
    /// </summary>
    public class ObjectManager : Singleton<ObjectManager>
    {
        // ����ID�������ֵ�
        private Dictionary<eObjectType, List<ulong>> _type2uidList;
        // �����б�
        private Dictionary<ulong, IDataObject> _objects;

        /// <summary>
        /// ���Ըı��¼�
        /// </summary>
        private event Action<DataList> _propChanged;

        /// <summary>
        /// ���ı��¼�
        /// </summary>
        private event Action<DataList> _recordChanged;

        /// <summary>
        /// ��������
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // ��ʼ��
            _objects = new Dictionary<ulong, IDataObject>();
            _type2uidList = new Dictionary<eObjectType, List<ulong>>();

            // ���ݶ���ش���
            DataObjectPool.Instance.Create();
            //���ݶ��󹤳�
            ObjectFactory.Instance.Create();                    

        }

        /// <summary>
        /// ��������
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            // �ͷ�
            foreach (var iobj in _objects.Values)
            {
                DataObject obj = iobj as DataObject;
                obj.Dispose();
            }

            _objects.Clear();
            _type2uidList.Clear();
            //���ݶ��󹤳�
            ObjectFactory.Instance.Dispose();
            // ���ݶ�����ͷ�
            DataObjectPool.Instance.Dispose();
        }

        /// <summary>
        /// ���Ըı�ص�
        /// </summary>
        /// <param name="args"></param>
        private void OnObjectPropertyChagned(DataList args)
        {
            _propChanged?.Invoke(args);
        }

        /////////////////////////////////////////////////////////////////////////////
        /// ���󼯲���
        /////////////////////////////////////////////////////////////////////////////
        #region ����
        public bool AddObject(IDataObject dataObject)
        {
            if (dataObject == null) return false;
            // ��ȡ����
            eObjectType type = (eObjectType)dataObject.GetInt("Type");
            // ��ȡuid
            ulong uid = dataObject.GetULong("UID");

            if (HasObject(uid))
            {
                GameLog.Error($"�ظ���UID����Ӷ���ʧ�ܡ�{dataObject.GetInt("ID")}");
                return false;
            }

            // �������ӳ��
            if (!_type2uidList.TryGetValue(type, out List<ulong> uidlist))
            {
                uidlist = new List<ulong>();
                _type2uidList.Add(type, uidlist);
            }
            uidlist.Add(uid);
            // ��Ӷ���
            _objects.Add(uid, dataObject);

            return true;
        }

        /// <summary>
        /// ����uid��ȡָ����������
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
        /// ��ȡָ��uid������ID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int GetConfig(ulong uid)
        {
            if (!_objects.TryGetValue(uid, out IDataObject iobj) || iobj == null) { return 0; }

            return iobj.ConfigID;
        }

        /// <summary>
        /// ��������ID����ָ�����͵Ķ���
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IDataObject CreateObjectByConfig(int config)
        {
            if (!ConfigManager.Instance.HasConfig(config))
            {
                return null;
            }

            // �ж�����
            eObjectType type = ConfigManager.Instance.GetType(config);
            if (!Enum.IsDefined(typeof(eObjectType), type))
            {
                return null;
            }

            // �����ռ���
            // string nameSpaceName = GetType().Namespace;
            // ����
            string className;

            // ����������
            switch (type)
            {
                case eObjectType.ViewPort:
                    {
                        // ��ȡ������
                        eViewPort subType = (eViewPort)ConfigManager.Instance.GetConfigInt(config, "ViewType");
                        // ��ȡ����
                        className = subType.ToString();
                    }
                    break;

                default:
                    className = "DataObject";
                    break;
            }

            // ��������
            string fullName = GetType().Namespace + "." + className;
            Type classType = System.Type.GetType(fullName);
            return Activator.CreateInstance(classType) as IDataObject;
        }

        /// <summary>
        /// ����һ�����ݶ���
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

            // 1.����config��������
            IDataObject dataObject = CreateObjectByConfig(config);
            if (dataObject == null)
            {
                return 0;
            }

            // 2. ��ʼ������
            if (!dataObject.Init(config))
            {
                DataObjectPool.Instance.ReleaseObject(dataObject);
                return 0;
            }

            dataObject.SetULong("UID", uid);
            dataObject.UID = uid;

            // 3.�������uidӳ��
            AddObject(dataObject);

            // 4. ������Իص�����
            dataObject.SetPropChangedCallback(OnObjectPropertyChagned);

            return uid;
        }

        /// <summary>
        /// ����һ�����ݶ���
        /// </summary>
        public ulong CreateObject<T>(int config) where T : class, IDataObject, new()
        {
            if (!ConfigManager.Instance.HasConfig(config))
            {
                return 0;
            }

            // �ж�����
            eObjectType type = ConfigManager.Instance.GetType(config);
            if (!Enum.IsDefined(typeof(eObjectType), type))
            {
                return 0;
            }

            /************************************************************************/
            // ˳���ܴ�                                                             
            /************************************************************************/
            // 1. ���ɲ�������uid
            ulong uid = DataUtility.GenerateUID(config);
            if (_objects.ContainsKey(uid))
            {
                return 0;
            }

            // ��������
            T dataObject = DataObjectPool.Instance.FetchObject<T>(config);
            if (dataObject == null)
            {
                return 0;
            }

            // 2. ��ʼ������
            if (!dataObject.Init(config))
            {
                DataObjectPool.Instance.ReleaseObject(dataObject);
                return 0;
            }

            dataObject.SetULong("UID", uid);
            dataObject.UID = uid;

            // 3.�������uidӳ��
            AddObject(dataObject);

            // 4. ������Իص�����
            dataObject.SetPropChangedCallback(OnObjectPropertyChagned);

            return uid;
        }

        /// <summary>
        /// ���ٶ���
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool DestoryObject(ulong uid)
        {
            if (!_objects.ContainsKey(uid))
            {
                return false;
            }

            // ���Ի�ȡ���ݶ���
            if (!_objects.TryGetValue(uid, out IDataObject dataObject))
            {
                return false;
            }

            if (dataObject == null)
            {
                _objects.Remove(uid);
                return false;
            }

            //  ��ȡ��������
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

            // ��ȡ���ڵ�ͼ���ͣ�������ڳ����У��п�����None(0)
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
        /// ���ٶ���
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
        /// �Ƿ����ָ��uid����
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool HasObject(ulong uid)
        {
            if (_objects == null) { return false; }
            return _objects.ContainsKey(uid);
        }

        /// <summary>
        /// ����uid��ȡ���ݶ���
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public bool TryGetObject(ulong uid, out IDataObject dataObject)
        {
            dataObject = null;

            // �Ƿ����ָ��uid����
            if (!_objects.TryGetValue(uid, out dataObject))
            {
                return false;
            }

            return dataObject != null;
        }

        /// <summary>
        /// ����uid��ȡָ�����͵����ݶ���
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
        /// ����ָ�����ͣ���ȡ��ǰ�����������еĶ���UID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="outlist"></param>
        /// <returns></returns>
        public int GetObjects(eObjectType type, out List<ulong> outlist)
        {
            outlist = null;
            // �����Ƿ���Ч
            if (!Enum.IsDefined(typeof(eObjectType), type))
            {
                return 0;
            }

            // ��ȡָ���������е�dui
            if (!_type2uidList.TryGetValue(type, out List<ulong> uidlist))
            {
                return 0;
            }

            // ��ֵ
            outlist = new List<ulong>();
            outlist.AddRange(uidlist);
            return outlist.Count;
        }

        #endregion

        #region �Ӷ���
        /// <summary>
        /// �����Ӷ���
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

            // ��ȡ������
            IDataObject parentObject = _objects[parent];
            if (parentObject == null)
            {
                return false;
            }

            uid = parentObject.CreateChild(config, pos);

            return uid != 0;
        }

        /// <summary>
        /// ����λ���Ƴ�һ���Ӷ���
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

            // �����Ӷ���
            IDataObject dataObject = _objects[childUID];
            if (dataObject == null)
            {
                return false;
            }

            // ��������
            DataObjectPool.Instance.ReleaseObject(dataObject);

            // �Ƴ��Ӷ���
            _objects.Remove(childUID);

            return true;
        }

        /// <summary>
        /// ����uid�Ƴ��Ӷ���
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

            // �Ƿ���ڸ�����
            IDataObject parentObj = _objects[parent];
            if (parentObj == null)
            {
                return false;
            }

            // �������Ƴ��Ӷ���
            if (!parentObj.FindChild(childObj))
            {
                GameLog.Error("û���ҵ��Ӷ����Ƴ�ʧ��");
                return false;
            }

            // ��������
            DataObjectPool.Instance.ReleaseObject(childObj);

            // �����Ƴ�
            _objects.Remove(childObj.UID);

            return true;
        }

        /// <summary>
        /// ��ȡ�Ӷ�������
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
        /// ��ȡ�Ӷ����ڸ������е�λ��
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
        /// ��ȡ�Ӷ���ĸ�����
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
        /// ���������Ӷ���λ��
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
        /// ����λ�û�ȡ�Ӷ���
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
        /// ����config��ȡ��һ�����������Ķ���
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

            // ��������
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
        /// �������ƻ�ȡָ�����͵��Ӷ���
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

            // ��������
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
        /// ����config��ȡ���е��Ӷ���
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

            // ��������
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
        /// �������ƻ�ȡ���е��Ӷ���
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

            // ��������
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
        /// �������ͻ�ȡ���������������Ӷ���
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

            // ��������
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
        /// ��ȡ���е��Ӷ���
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

            // ��������
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

        #region ���Բ���

        /// <summary>
        /// ��������Ƿ����
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
        /// ��ȡָ�����Ե�����
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
        /// ����int��������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetInt(ulong uid, string name, int val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetInt(name) != val) { obj.SetInt(name, val); }
            return true;
        }

        /// <summary>
        /// ����ulong��������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetULong(ulong uid, string name, ulong val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetULong(name) != val) { obj.SetULong(name, val); }
            return true;
        }

        /// <summary>
        /// ����long������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetLong(ulong uid, string name, long val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetLong(name) != val) { obj.SetLong(name, val); }
            return true;
        }

        /// <summary>
        /// ����float��������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetFloat(ulong uid, string name, float val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetFloat(name) != val) { obj.SetFloat(name, val); }
            return true;
        }

        /// <summary>
        /// ����double��������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetDouble(ulong uid, string name, double val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetDouble(name) != val) { obj.SetDouble(name, val); }
            return true;
        }

        /// <summary>
        /// ����string��������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetString(ulong uid, string name, string val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetString(name) != val) { obj.SetString(name, val); }
            return true;
        }

        /// <summary>
        /// ����bool��������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetBool(ulong uid, string name, bool val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetBool(name) != val) { obj.SetBool(name, val); }
            return true;
        }

        /// <summary>
        /// ����vector2��������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetVector2(ulong uid, string name, Vector2 val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetVector2(name) != val) { obj.SetVector2(name, val); }
            return true;
        }

        /// <summary>
        /// ����vector3��������
        /// </summary>
        /// <param name="uid">ָ��uid</param>
        /// <param name="name">��������</param>
        /// <param name="val">����ֵ</param>
        /// <returns></returns>
        public bool SetVector3(ulong uid, string name, Vector3 val)
        {
            if (!_objects.TryGetValue(uid, out IDataObject obj) || obj == null) { return false; }
            if (obj.GetVector3(name) != val) { obj.SetVector3(name, val); }
            return true;
        }

        /// <summary>
        /// ��ȡָ�����͵�int����
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
        /// ��ȡulong����
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
        /// ��ȡlong����
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
        /// ��ȡ float ����
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
        /// ��ȡ double ����
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
        /// ��ȡ bool ����
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
        /// ��ȡ string ����
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
        /// ��ȡ vector2 ����
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
        /// ��ȡ vector3 ����
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
        /// ����ָ�����ͻ�ȡint���ͣ������ж��Ƿ��óɹ�
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
        /// �������Ըı��¼�
        /// </summary>
        public void ListenPropertyChanged(Action<DataList> action)
        {
            // ��֤��������Ψһ
            _propChanged -= action;
            _propChanged += action;
        }

        /// <summary>
        /// �Ƴ����Ըı��¼�
        /// </summary>
        public void CancelPropertyChanged(Action<DataList> action)
        {
            _propChanged -= action;
        }

        #endregion

        #region ��ʱ����
        /// <summary>
        /// �Ƿ������ʱ����
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
        /// ��ȡָ������ֵ������
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
        /// ���һ����ʱ����
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

            // �Ƿ�����Ч������
            if (!Enum.IsDefined(typeof(eDataType), type))
            {
                return false;
            }

            // �Ƿ��Ѿ�������
            if (obj.HasData(name))
            {
                return false;
            }

            return obj.AddData(name, type);
        }

        /// <summary>
        /// �Ƴ�һ����ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.RemoveData(name);
        }

        /// <summary>
        /// ����һ��int���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataInt(name, val);
        }

        /// <summary>
        /// ����һ��ulong���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataULong(name, val);
        }

        /// <summary>
        /// ����һ��long���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataLong(name, val);
        }

        /// <summary>
        /// ����һ��float���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataFloat(name, val);
        }

        /// <summary>
        /// ����double���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataDouble(name, val);
        }

        /// <summary>
        /// ����string���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataString(name, val);
        }

        /// <summary>
        /// ����bool���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataBool(name, val);
        }

        /// <summary>
        /// ����vector2���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataVector2(name, val);
        }

        /// <summary>
        /// ����vector3���͵���ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return false;
            }

            return obj.SetDataVector3(name, val);
        }

        /// <summary>
        /// ��ȡָ�����͵�int����
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

            // �Ƿ������ʱ����
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

            // �Ƿ������ʱ����
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

            // �Ƿ������ʱ����
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

            // �Ƿ������ʱ����
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

            // �Ƿ������ʱ����
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

            // �Ƿ������ʱ����
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

            // �Ƿ������ʱ����
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

            // �Ƿ������ʱ����
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

            // �Ƿ������ʱ����
            if (!obj.HasData(name))
            {
                return default;
            }

            return obj.GetDataVector3(name);
        }

        #endregion

        #region ������

        /// <summary>
        /// ������Ƿ����
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
        /// ���ݱ������ȡ���
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
        /// ���ñ����������
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
        /// ���ñ���޷��ų���������
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
        /// ���ñ����������
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
        /// ���ñ�񵥾��ȸ���������
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
        /// ���ñ��˫���ȸ���������
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
        /// ���ñ�񲼶�������
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
        /// ���ñ��string������
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
        /// ���ñ���ά����������
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
        /// ���ñ����ά����������
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
        /// ��ȡ�����������
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
        /// ��ȡ����޷��ų���������
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
        /// ��ȡ�����������
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
        /// ��ȡ��񵥾��ȸ���������
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
        /// ��ȡ���˫���ȸ���������
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
        /// ��ȡ��񲼶�������
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
        /// ��ȡ����ַ���������
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
        /// ��ȡ����ά����������
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
        /// ��ȡ�����ά����������
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
        /// ��ȡ�����������
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
        /// ��ȡ����޷��ų���������
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
        /// ��ȡ�����������
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
        /// ��ȡ��񵥾��ȸ���������
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
        /// ��ȡ���˫���ȸ���������
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
        /// ��ȡ��񲼶�������
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
        /// ��ȡ����ַ���������
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
        /// ��ȡ����ά����������
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
        /// ��ȡ�����ά����������
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
        /// ���ñ����������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordInt(UID, recName, value, row, col);
        }

        /// <summary>
        /// ���ñ���޷��ų���������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordULong(UID, recName, value, row, col);
        }

        /// <summary>
        /// ���ñ����������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordLong(UID, recName, value, row, col);
        }

        /// <summary>
        /// ���ñ�񵥾��ȸ���������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordFloat(UID, recName, value, row, col);
        }

        /// <summary>
        /// ���ñ��˫���ȸ���������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordDouble(UID, recName, value, row, col);
        }

        /// <summary>
        /// ���ñ�񲼶�������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordBool(UID, recName, value, row, col);
        }

        /// <summary>
        /// ���ñ��string������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordString(UID, recName, value, row, col);
        }

        /// <summary>
        /// ���ñ���ά����������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordVec2(UID, recName, value, row, col);
        }

        /// <summary>
        /// ���ñ����ά����������
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
                GameLog.Error("���ñ������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            return SetRecordVec3(UID, recName, value, row, col);
        }

        /// <summary>
        /// ��ȡ�����������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetInt(row, col);
        }

        /// <summary>
        /// ��ȡ����޷��ų���������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetULong(row, col);
        }


        /// <summary>
        /// ��ȡ�����������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetLong(row, col);
        }

        /// <summary>
        /// ��ȡ��񵥾��ȸ���������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetFloat(row, col);
        }

        /// <summary>
        /// ��ȡ���˫���ȸ���������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetDouble(row, col);
        }

        /// <summary>
        /// ��ȡ��񲼶�������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetBool(row, col);
        }

        /// <summary>
        /// ��ȡ����ַ���������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetString(row, col);
        }

        /// <summary>
        /// ��ȡ����ά����������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetVector2(row, col);
        }

        /// <summary>
        /// ��ȡ�����ά����������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return value;
            }

            return record.GetVector3(row, col);
        }

        /// <summary>
        /// ��ȡ�����������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetInt(row, col);
            return true;
        }

        /// <summary>
        /// ��ȡ����޷��ų���������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetULong(row, col);
            return true;
        }

        /// <summary>
        /// ��ȡ�����������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetLong(row, col);
            return true;
        }

        /// <summary>
        /// ��ȡ��񵥾��ȸ���������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetFloat(row, col);
            return true;
        }

        /// <summary>
        /// ��ȡ���˫���ȸ���������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetDouble(row, col);
            return true;
        }

        /// <summary>
        /// ��ȡ��񲼶�������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetBool(row, col);
            return true;
        }

        /// <summary>
        /// ��ȡ����ַ���������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetString(row, col);
            return true;
        }

        /// <summary>
        /// ��ȡ����ά����������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetVector2(row, col);
            return true;
        }

        /// <summary>
        /// ��ȡ�����ά����������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return false;
            }

            value = record.GetVector3(row, col);
            return true;
        }


        /// <summary>
        /// ������������
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
        /// �����Ӷ�������
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
        /// ����Ƴ�������
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
        /// ����Ƴ���������
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
        /// ��ȡ�����������
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
        /// ���ұ�������������
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
        /// ���ұ�������������
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
                GameLog.Error("��ȡ�������ʧ�ܡ�---> ���{recName}�������б�ǩ{colTag}!");
                return count;
            }

            return record.FindRows(col, dataVariant, out rowList);
        }

        /// <summary>
        /// ��ȡ��ʹ�õ���
        /// </summary>
        /// <param name="recName">�������</param>
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
        /// �������ı��¼�
        /// </summary>
        public void ListenRecordChanged(Action<DataList> func)
        {
            // ��֤��������Ψһ
            _recordChanged -= func;
            _recordChanged += func;
        }

        /// <summary>
        /// �Ƴ����ı��¼�
        /// </summary>
        public void CancelRecordChanged(Action<DataList> func)
        {
            _recordChanged -= func;
        }

        /// <summary>
        /// ���ı�ص�
        /// </summary>
        private void OnRecordChanged<T>(ulong UID, string recName, int row, int column, eRecordOperation op, T oldValue)
        {
            DataList args = DataList.Get();

            int objectType = GetInt(UID, "Type");
            if (objectType == 0)
            {
                int configID = GetInt(UID, "ID");
                GameLog.Error($"������ص�����ʧ�ܡ�---> ����{configID}��ObjectTypeΪ{objectType}��");
                return;
            }

            args.AddULong(UID);
            args.AddString(recName);
            args.AddInt(row);
            args.AddInt(column);
            args.AddInt((int)op);
            args.Add(oldValue);

            // ֱ���¼��ص�
            _recordChanged?.Invoke(args);
            DataList.Recyle(args);
        }

        #endregion
    }
}
