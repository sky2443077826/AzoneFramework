
using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ��Ϸ����������
    /// </summary>
    public class DataObject : IDataObject
    {
        // ����ID
        public int ConfigID { get; set; }
        // ΨһID
        public ulong UID { get; set; }
        // �Ӷ�������
        public int Pos { get; set; }
        // ������
        public ulong Parent { get; set; }
        // ����
        public int Capacity { get; set; }
        // ��������
        public eObjectType Type { get; set; }

        // ���Ըı�ص�
        public ObjectPropChangedCallback PropChagnedCallback { get; set; }

        // ��Ϸ�������Թ�����
        protected PropertyManager _propMgr;
        // ��Ϸ��ʱ���ݹ�����
        protected DataManager _dataMgr;
        // ��Ϸ�����������
        protected RecordManager _recMgr;
        // ��Ϸ�Ӷ���
        protected ulong[] _childObjects;

        /// <summary>
        /// ��ʼ������
        /// </summary>
        public virtual bool Init(int configID)
        {
            if (!ConfigManager.Instance.HasConfig(configID))
            {
                GameLog.Error("û�з�������ID����ʼ������ʧ�ܡ�{0}", configID);
                return false;
            }

            // ����
            eObjectType type = ConfigManager.Instance.GetType(configID);
            if (type == eObjectType.None || !Enum.IsDefined(typeof(eObjectType), type))
            {
                GameLog.Error($"��Ч������: {configID} -> {type}");
                return false;
            }

            this.ConfigID = configID;

            _propMgr = new PropertyManager();
            _dataMgr = new DataManager();
            _recMgr = new RecordManager();

            // ��ʼ�����Թ�����
            if (!ConfigManager.Instance.CloneTo(this.ConfigID, ref _propMgr, ref _recMgr))
            {
                GameLog.Error("�������Ժͱ��ʧ�ܣ���ʼ������ʧ�ܡ�{0}", this.ConfigID);
                return false;
            }

            // ��������
            _propMgr.SetInt("Type", (int)type);
            this.Type = type;

            // ��ȡ��������
            this.Capacity = _propMgr.GetInt("Capacity");

            // ������Ϸ�Ӷ���
            _childObjects = new ulong[Capacity];

            // �������е�����
            this.Pos = -1;

            // ������
            this.Parent = 0;

            return true;
        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void Dispose()
        {
            // 1.���������Ӷ���
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
        /// �Ƿ��ǽ�ɫ
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRole()
        {
            return false;
        }

        /// <summary>
        /// �������Իص�
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public void SetPropChangedCallback(ObjectPropChangedCallback func)
        {
            PropChagnedCallback = func;
        }

        /// <summary>
        /// ���Ըı���Ŷ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="val"></param>
        private void OnPropertyChanged<T>(string name, T val)
        {
            if (PropChagnedCallback == null) { return; }

            DataList args = DataList.Get();
            // ��������
            args.AddInt((int)Type);
            // ����UID
            args.AddULong(UID);
            // ������
            args.AddString(name);
            // ��ֵ
            args.Add(val);

            // �ص�
            PropChagnedCallback.Invoke(args);
            args.Dispose();
        }

        #region ��Ϸ�Ӷ���    

        /// <summary>
        /// �Ӷ�������
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
        /// ����Ӷ����Ƿ����
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
        /// �������Ƿ����
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
        /// ����Ӷ����Ƿ����
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
        /// ���һ���Ӷ���
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
        /// �����Ӷ���
        /// </summary>
        /// <returns></returns>
        public ulong CreateChild(int config, int pos)
        {
            if (ChildCount() > Capacity || !ConfigManager.Instance.HasConfig(config))
            {
                return 0;
            }

            // ����һ�����ʵ�λ��
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
                GameLog.Error($"�����Ӷ���ʧ�ܣ�λ�ò��Ϸ�{pos} -> {config}");
                return 0;
            }

            // �������ݶ���
            ulong uid = ObjectManager.Instance.CreateObject(config);
            if (0 == uid)
            {
                GameLog.Error($"����dataobjectʧ�ܡ�{config}");
                return 0;
            }

            // ��ȡ���ݶ���
            if (!ObjectManager.Instance.TryGetObject(uid, out IDataObject childObj))
            {
                ObjectManager.Instance.DestoryObject(uid);
                return 0;
            }

            // �����Ӷ�������
            childObj.Pos = pos;
            childObj.Parent = this.UID;

            // ��ӵ�������
            _childObjects[pos] = uid;

            return uid;
        }

        /// <summary>
        /// �����Ӷ���
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

            // ����һ�����ʵ�λ��
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
                GameLog.Error($"�����Ӷ���ʧ�ܣ�λ�ò��Ϸ�{pos} -> {config}");
                return 0;
            }

            // �������ݶ���
            ulong uid = ObjectManager.Instance.CreateObject<T>(config);
            if (0 == uid)
            {
                GameLog.Error($"����dataobjectʧ�ܡ�{config}");
                return 0;
            }

            // ��ȡ���ݶ���
            if (!ObjectManager.Instance.TryGetObject(uid, out T childObj))
            {
                ObjectManager.Instance.DestoryObject(uid);
                return 0;
            }

            // �����Ӷ�������
            childObj.Pos = pos;
            childObj.Parent = this.UID;

            // ��ӵ�������
            _childObjects[pos] = uid;

            return uid;
        }

        /// <summary>
        /// �Ƴ�ָ��uid����
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

            // ��ȡ�Ӷ���
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
        /// �Ƴ��Ӷ���
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
        /// ���������Ӷ���λ��
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

            // �������Ƿ����
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

            // ��������
            _childObjects[src] = destUID;
            _childObjects[dest] = srcUID;

            srcObj.Pos = dest;
            destObj.Pos = src;

            return true;
        }

        /// <summary>
        /// ����λ�û�ȡ����
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public ulong GetChild(int pos)
        {
            if (pos < 0 || pos >= _childObjects.Length)
            {
                return 0;
            }

            // ��ȡuid
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
        /// ����confiid��ȡ��һ���Ӷ���
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

                // ��ȡ����
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
        /// �������ͻ�ȡ��һ���Ӷ���
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

                // ��ȡ����
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
        /// ����configID��ȡ�����Ӷ���
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
        /// �������ͻ�ȡ�����Ӷ���
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

        #region ���Թ�����

        /// <summary>
        /// ��ȡ��Ҫ�洢�����Ե�����
        /// </summary>
        /// <param name="propList"></param>
        /// <returns></returns>
        public int GetPropertyList(ref List<string> propList, bool onlySaved)
        {
            return _propMgr.GetPropertyList(ref propList, onlySaved);
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="name">������</param>
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
        /// �Ƿ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasProperty(string name)
        {
            return _propMgr.Find(name);
        }

        #region  Set������

        /// <summary>
        /// ����int��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetInt(string name, int val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��������ʧ�ܣ�û��{0}��", name);
                return;
            }

            int oldVal = _propMgr.GetInt(name);
            if (oldVal == val) { return; }

            _propMgr.SetInt(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// ����long��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetLong(string name, long val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��������ʧ�ܣ�û��{0}��", name);
                return;
            }

            long oldVal = _propMgr.GetLong(name);
            if (oldVal == val) { return; }

            _propMgr.SetLong(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// ����ulong��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetULong(string name, ulong val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error($"��������ʧ�ܣ�{ConfigID}û��{name}��");
                return;
            }

            ulong oldVal = _propMgr.GetULong(name);
            if (oldVal == val) { return; }

            _propMgr.SetULong(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// ����float��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetFloat(string name, float val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��������ʧ�ܣ�û��{0}��", name);
                return;
            }

            float oldVal = _propMgr.GetFloat(name);
            if (oldVal == val) { return; }

            _propMgr.SetFloat(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// ����double��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetDouble(string name, double val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��������ʧ�ܣ�û��{0}��", name);
                return;
            }

            double oldVal = _propMgr.GetDouble(name);
            if (oldVal == val) { return; }

            _propMgr.SetDouble(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// ����string��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetString(string name, string val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��������ʧ�ܣ�û��{0}��", name);
                return;
            }

            string oldVal = _propMgr.GetString(name);
            if (oldVal == val) { return; }
            _propMgr.SetString(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// ����bool��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetBool(string name, bool val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��������ʧ�ܣ�û��{0}��", name);
                return;
            }
            bool oldVal = _propMgr.GetBool(name);
            if (oldVal == val) { return; }
            _propMgr.SetBool(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// ����vector2����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetVector2(string name, Vector2 val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��������ʧ�ܣ�û��{0}��", name);
                return;
            }

            Vector2 oldVal = _propMgr.GetVector2(name);
            if (oldVal == val) { return; }
            _propMgr.SetVector2(name, val);
            OnPropertyChanged(name, oldVal);
        }

        /// <summary>
        /// ����vector3��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        public void SetVector3(string name, Vector3 val)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��������ʧ�ܣ�û��{0}��", name);
                return;
            }
            Vector3 oldVal = _propMgr.GetVector3(name);
            if (oldVal == val) { return; }
            _propMgr.SetVector3(name, val);
            OnPropertyChanged(name, oldVal);
        }
        #endregion

        #region Get������
        /// <summary>
        /// ��ȡint����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public int GetInt(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return 0;
            }

            return _propMgr.GetInt(name);
        }

        /// <summary>
        /// ��ȡlong����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public long GetLong(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return 0;
            }

            return _propMgr.GetLong(name);
        }

        /// <summary>
        /// ��ȡulong����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public ulong GetULong(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return 0;
            }

            return _propMgr.GetULong(name);
        }

        /// <summary>
        /// ��ȡfloat����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public float GetFloat(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return 0.0f;
            }

            return _propMgr.GetFloat(name);
        }

        /// <summary>
        /// ��ȡdouble����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public double GetDouble(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return 0.0;
            }

            return _propMgr.GetDouble(name);
        }

        /// <summary>
        /// ��ȡstring����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public string GetString(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return default;
            }

            return _propMgr.GetString(name);
        }

        /// <summary>
        /// ��ȡbool����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public bool GetBool(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return default;
            }
            return _propMgr.GetBool(name);
        }

        /// <summary>
        /// ��ȡvector����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public Vector2 GetVector2(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return Vector2.zero;
            }

            return _propMgr.GetVector2(name);
        }

        /// <summary>
        /// ��ȡvector3����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        public Vector3 GetVector3(string name)
        {
            if (!_propMgr.Find(name))
            {
                GameLog.Error("��ȡ����ʧ�ܣ�û��{0}��", name);
                return Vector3.zero;
            }

            return _propMgr.GetVector3(name);
        }
        #endregion
        #endregion

        #region ��ʱ���Թ�����

        /// <summary>
        /// �Ƿ������ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasData(string name)
        {
            return _dataMgr.Find(name);
        }

        /// <summary>
        /// ��ȡ��ʱ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public eDataType GetDataType(string name)
        {
            return _dataMgr.GetType(name);
        }

        /// <summary>
        /// ���һ����ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool AddData(string name, eDataType type)
        {
            if (_dataMgr.Find(name))
            {
                GameLog.Error("��ʱ�����Ѿ����ڣ����ʧ�ܡ�{0}", name);
                return false;
            }

            if (!_dataMgr.Add(name, new Property(name, type)))
            {
                GameLog.Error("�����ʱ����ʧ�ܡ�{0} -> {1}", name, type);
                return false;
            }

            return true;
        }

        /// <summary>
        /// �Ƴ���ʱ����
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
        /// ����һ��int���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetDataInt(string name, int val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetInt(name, val);
        }

        public bool SetDataULong(string name, ulong val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetULong(name, val);
        }

        public bool SetDataLong(string name, long val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetLong(name, val);
        }

        public bool SetDataFloat(string name, float val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetFloat(name, val);
        }

        public bool SetDataDouble(string name, double val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetDouble(name, val);
        }

        public bool SetDataBool(string name, bool val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetBool(name, val);
        }

        public bool SetDataString(string name, string val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetString(name, val);
        }

        public bool SetDataVector2(string name, Vector2 val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetVector2(name, val);
        }

        public bool SetDataVector3(string name, Vector3 val)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("������ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.SetVector3(name, val);
        }

        /// <summary>
        /// ��ȡһ����ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetDataInt(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return 0;
            }

            return _dataMgr.GetInt(name);
        }

        public ulong GetDataULong(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return 0;
            }

            return _dataMgr.GetULong(name);
        }

        public long GetDataLong(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return 0;
            }

            return _dataMgr.GetLong(name);
        }

        public float GetDataFloat(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return 0.0f;
            }

            return _dataMgr.GetFloat(name);
        }

        public double GetDataDouble(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return 0.0;
            }

            return _dataMgr.GetDouble(name);
        }

        public bool GetDataBool(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return false;
            }

            return _dataMgr.GetBool(name);
        }

        public string GetDataString(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return default;
            }

            return _dataMgr.GetString(name);
        }

        public Vector2 GetDataVector2(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return Vector2.zero;
            }

            return _dataMgr.GetVector2(name);
        }


        public Vector3 GetDataVector3(string name)
        {
            if (!_dataMgr.Find(name))
            {
                GameLog.Error("��ȡ��ʱ����ʧ�ܣ�û���ҵ�����{0}", name);
                return Vector3.zero;
            }

            return _dataMgr.GetVector3(name);
        }

        #endregion

        #region ���л�

        /// <summary>
        /// ���л���xml
        /// </summary>
        /// <returns></returns>
        public virtual bool SerializeToXml(XmlElement root)
        {
            string clsName = _propMgr.GetString("Class");
            if (string.IsNullOrEmpty(clsName))
            {
                GameLog.Error("���л�����ʧ�ܣ�û���ҵ�class��");
                return false;
            }

            if (root == null)
            {
                GameLog.Error($"���л�����ʧ�ܣ�xml���ڵ�Ϊnull��Class:{clsName}, ConfigID:{ConfigID}");
                return false;
            }

            // �������ͽڵ�
            XmlElement node = root.OwnerDocument.CreateElement(clsName);
            if (node == null)
            {
                GameLog.Error($"�л�����ʧ�ܣ��������ݽڵ�ʧ�ܡ�Class:{clsName}, ConfigID: {ConfigID}");
                return false;
            }

            /**
             *      ���л�����
             **/
            SerializeProperty(node);

            /**
             *      ���л����
             **/
            SerializeRecord(node);

            /**
             *      ���л��Ӷ���
             **/
            SerializeChildren(node);

            /**
             *      ���ڵ���ӵ�root��
             **/
            root.AppendChild(node);

            return true;
        }

        /// <summary>
        /// ���ⲿ����
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
                GameLog.Error($"û���ҵ�����{node.Name}");
                return false;
            }

            /**
             *      ������������
             */
            if (!ParsePropertyFromXML(node))
            {
                GameLog.Error($"��������ʧ��{node.Name}");
            }

            /**
             *      �������б��
             */

            if (!ParseRecordFromXML(node))
            {
                GameLog.Error($"�������ʧ��{node.Name}");
            }

            /**
             *      �����Ӷ���
             */
            if (!ParseChildrenFromXML(node))
            {
                GameLog.Error($"�����Ӷ���ʧ��{node.Name}");
            }
            return true;
        }

        /// <summary>
        /// ��������
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
                GameLog.Error($"{node.Name}û���ҵ����Թ������� �����洢ʧ�ܣ�");
                return false;
            }

            // 2.���Խڵ��Ƿ����
            XmlNode propNode = node.SelectSingleNode("Property");
            if (propNode == null)
            {
                GameLog.Error($"����浵�쳣��û��Property�ڵ㡣");
                return false;
            }

            // 3.������������
            foreach (XmlNode childNode in propNode.ChildNodes)
            {
                if (childNode == null)
                {
                    continue;
                }

                // ������
                string propName = childNode.Name;
                // ����ֵ
                string propValue = childNode.InnerText;

                if (string.IsNullOrEmpty(propName))
                {
                    continue;
                }

                // ��������
                eDataType propType = propMgr.GetType(propName);
                // ת������
                ConfigManager.ConverToProperty(propType, propName, propValue, out Property prop);
                // ���ô洢����
                prop.Save = propMgr.IsSaved(propName);

                // ��ӵ����Թ�����
                _propMgr.Set(propName, prop);
            }

            return true;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool ParseRecordFromXML(XmlNode node)
        {
            if (node == null) return false;

            XmlNode recNode = node.SelectSingleNode("Record");
            if (recNode == null)
            {
                // GameLog.Error($"����浵�쳣��û��Record�ڵ㡣");
                return true;
            }

            // �������б��
            foreach (XmlNode childNode in recNode.ChildNodes)
            {
                if (childNode == null)
                {
                    continue;
                }

                // �����
                string recName = childNode.Name;
                if (!_recMgr.HasRecord(recName))
                {
                    // û���ҵ����
                    continue;
                }

                Record rec = _recMgr.GetRecord(recName);
                if (rec == null)
                {
                    continue;
                }

                // ������
                foreach (XmlNode rowNode in childNode.ChildNodes)
                {
                    // ���һ��
                    int row = rec.AddRow();
                    if (row < 0) { continue; }
                    // ��ȡ���е�����ֵ
                    foreach (XmlAttribute columnAttr in rowNode.Attributes)
                    {
                        // ��ȡ������
                        string tag = columnAttr.Name;
                        // ��ȡ����ֵ
                        string val = columnAttr.Value;
                        if (string.IsNullOrEmpty(tag))
                        {
                            GameLog.Error("������ݴ���tagΪ��");
                            continue;
                        }

                        // ���ַ��������е��������
                        rec.FromString(row, tag, val);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// �����ӽڵ�
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
                // ��ȡ������
                string childClassName = childNode.Name;
                if (string.IsNullOrEmpty(childClassName))
                {
                    continue;
                }

                // �����Ӷ���
                int childConfigID = Convert.ToInt32(childNode.SelectSingleNode("Property/ID").InnerText);

                ulong childUID = CreateChild(childConfigID, -1);
                // ��ȡ�Ӷ���
                if (!ObjectManager.Instance.TryGetObject(childUID, out IDataObject childObject))
                {
                    continue;
                }
                if (childObject == null)
                {
                    GameLog.Error("�����Ӷ���ʧ�ܡ�");
                    continue;
                }
                childObject.ParseFrom(childNode.OuterXml);
            }

            return true;
        }

        /// <summary>
        /// ���л���������
        /// </summary>
        /// <param name="rootNode"></param>
        public bool SerializeProperty(XmlElement rootNode)
        {
            List<string> storePropList = new List<string>();
            if (_propMgr.GetPropertyList(ref storePropList, true) == 0)
            {
                // GameLog.Error("�л�����ʧ�ܣ���ɫû����Ҫ�洢�����ԣ���������ʧ�ܡ�");
                return false;
            }

            // �������Խڵ�
            XmlElement propertyNode = rootNode.OwnerDocument.CreateElement("Property");
            foreach (string propName in storePropList)
            {
                SerializeProperty(propertyNode, propName);
            }
            rootNode.AppendChild(propertyNode);
            return true;
        }

        /// <summary>
        /// ���л�ָ������
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rootNode"></param>
        public void SerializeProperty(XmlElement rootNode, string propName)
        {
            if (rootNode == null)
            {
                return;
            }

            // ��ȡ��������
            eDataType propType = _propMgr.GetType(propName);

            // �������Խڵ�
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
                        // ���������ӽڵ�  x��y��Ȼ�����
                        childNode.InnerText = _propMgr.GetVector3(propName).x.ToString()
                            + "," + _propMgr.GetVector3(propName).y.ToString()
                            + "," + _propMgr.GetVector3(propName).z.ToString();
                    }
                    break;
                default:
                    {
                        GameLog.Error("��������ʧ�ܣ�δʶ�������{0} -> {1}", propName, propType);
                        break;
                    }
            }

            rootNode.AppendChild(childNode);
        }

        /// <summary>
        /// ���л����
        /// </summary>
        public bool SerializeRecord(XmlElement rootNode)
        {
            // �������ڵ�
            XmlElement recordNode = rootNode.OwnerDocument.CreateElement("Record");
            // ���л�����߼�
            if (_recMgr.GetRecordList(out List<string> records, true) != 0)
            {
                // ����Ҫ����ı��
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

                    // �Ƿ��Ǳ���ı��
                    if (!rec.Save)
                    {
                        continue;
                    }

                    // ��ȡ���
                    SerializeRecord(recordNode, rec);
                }
            }

            rootNode.AppendChild(recordNode);

            return true;
        }

        /// <summary>
        /// ���л����xml
        /// </summary>
        /// <param name="recordNode"></param>
        /// <param name="rec"></param>
        public void SerializeRecord(XmlElement recordNode, Record rec)
        {
            if (recordNode == null || rec == null)
            {
                return;
            }

            // ��ȡ�����
            string recName = rec.Name;
            if (string.IsNullOrEmpty(recName))
            {
                return;
            }

            // �����ڵ�
            XmlElement node = recordNode.OwnerDocument.CreateElement(recName);
            // �������
            int maxRows = rec.MaxRowCount;

            for (int row = 0; row < maxRows; ++row)
            {
                if (!rec.CheckRowUse(row))
                {
                    continue;
                }

                // �����нڵ�
                XmlElement rowNode = node.OwnerDocument.CreateElement("Row");

                for (int column = 0; column < rec.ColumnCount; ++column)
                {
                    // ��ȡtag����
                    string tag = rec.GetTagByColumn(column);
                    if (string.IsNullOrEmpty(tag))
                    {
                        continue;
                    }

                    // �ַ���ֵ
                    string val = rec.ToString(row, column);

                    // ��������
                    rowNode.SetAttribute(tag, val);
                }

                // ����нڵ�
                node.AppendChild(rowNode);
            }

            // ��ӽڵ�
            recordNode.AppendChild(node);
        }

        /// <summary>
        /// ���л��Ӷ���
        /// </summary>
        /// <param name="rootNode"></param>
        public virtual bool SerializeChildren(XmlElement rootNode)
        {
            if (rootNode == null)
            {
                return false;
            }

            // ��ȡ�ӽڵ㣬Ȼ��������л�
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

        #region ������

        /// <summary>
        /// �Ƿ���ڱ��
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasRecord(string name)
        {
            return _recMgr.HasRecord(name);
        }

        /// <summary>
        /// ��ȡ���
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Record GetRecord(string name)
        {
            return _recMgr.GetRecord(name);
        }

        /// <summary>
        /// ��ȡ��Ҫ�洢���б������
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