using System.Xml;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    // ���Ըı�ص�
    public delegate void ObjectPropChangedCallback(DataList args);
    // ���ı�ص�
    public delegate void ObjectRecordChangedCallback(DataList args);

    /// <summary>
    /// �������ݶ���Ļ���ӿ�
    /// </summary>
    public interface IDataObject : ISerializable
    {
        // ����ID
        int ConfigID { get; set; }
        // ΨһID
        ulong UID { get; set; }
        // �Ӷ�������
        int Pos { get; set; }
        // ������
        ulong Parent { get; set; }
        // ����
        int Capacity { get; set; }
        // ��������
        eObjectType Type { get; set; }

        // ���Ըı�ص�
        ObjectPropChangedCallback PropChagnedCallback { get; set; }

        /// <summary>
        /// ��ʼ�����ݶ���
        /// </summary>
        /// <param name="configID">����ID</param>
        /// <returns></returns>
        bool Init(int configID);

        /// <summary>
        /// ���ݶ�������
        /// </summary>
        void Dispose();

        /// <summary>
        /// �������Իص�
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        void SetPropChangedCallback(ObjectPropChangedCallback func);

        /// <summary>
        /// �Ƿ������
        /// </summary>
        /// <returns></returns>
        bool IsRole();

        #region ��Ϸ�Ӷ���    
        /// <summary>
        /// �Ӷ�������
        /// </summary>
        int ChildCount();

        /// <summary>
        /// ����Ӷ����Ƿ����
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool FindChild(int pos);

        /// <summary>
        /// ����Ӷ����Ƿ����
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool FindChild(IDataObject obj);

        /// <summary>
        /// ���һ���Ӷ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool AddChild(IDataObject obj, int pos = -1);

        /// <summary>
        /// �����Ӷ���
        /// </summary>
        /// <returns></returns>
        ulong CreateChild(int config, int pos = -1);

        /// <summary>
        /// �����Ӷ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        ulong CreateChild<T>(int config, int pos) where T : class, IDataObject, new();

        /// <summary>
        /// �Ƴ�ָ��uid����
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool RemoveChild(int pos);

        /// <summary>
        /// �Ƴ��Ӷ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool RemoveChild(IDataObject obj);

        /// <summary>
        /// ����λ�û�ȡ����
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        ulong GetChild(int pos);

        /// <summary>
        /// ����confiid��ȡ��һ���Ӷ���
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        ulong GetFirstChild(int config);

        /// <summary>
        /// �������ͻ�ȡ��һ���Ӷ���
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ulong GetFirstChild(eObjectType type);

        /// <summary>
        /// ����configID��ȡ�����Ӷ���
        /// </summary>
        /// <param name="config"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        int GetChildren(int config, out List<ulong> children);

        /// <summary>
        /// �������ͻ�ȡ�����Ӷ���
        /// </summary>
        /// <param name="type"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        int GetChildren(eObjectType type, out List<ulong> children);

        #endregion

        #region ���Թ�����

        /// <summary>
        /// ��ȡ��Ҫ�洢�����Ե�����
        /// </summary>
        /// <param name="propList"></param>
        /// <returns></returns>
        int GetPropertyList(ref List<string> propList, bool saved);

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns></returns>
        eDataType GetPropertyType(string name);

        /// <summary>
        /// �Ƿ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasProperty(string name);

        #region  Set������
        /// <summary>
        /// ����int��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetInt(string name, int val);

        /// <summary>
        /// ����long��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetLong(string name, long val);

        /// <summary>
        /// ����ulong��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetULong(string name, ulong val);

        /// <summary>
        /// ����float��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetFloat(string name, float val);

        /// <summary>
        /// ����double��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetDouble(string name, double val);

        /// <summary>
        /// ����string��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetString(string name, string val);

        /// <summary>
        /// ����bool��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetBool(string name, bool val);

        /// <summary>
        /// ����vector2����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetVector2(string name, Vector2 val);

        /// <summary>
        /// ����vector3��������ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        void SetVector3(string name, Vector3 val);

        #endregion

        #region Get������
        /// <summary>
        /// ��ȡint����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        int GetInt(string name);

        /// <summary>
        /// ��ȡlong����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        long GetLong(string name);

        /// <summary>
        /// ��ȡulong����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        ulong GetULong(string name);

        /// <summary>
        /// ��ȡfloat����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        float GetFloat(string name);

        /// <summary>
        /// ��ȡdouble����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        double GetDouble(string name);

        /// <summary>
        /// ��ȡstring����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        string GetString(string name);

        /// <summary>
        /// ��ȡbool����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        bool GetBool(string name);

        /// <summary>
        /// ��ȡvector����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        Vector2 GetVector2(string name);

        /// <summary>
        /// ��ȡvector3����ֵ
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        Vector3 GetVector3(string name);
        #endregion
        #endregion

        #region ��ʱ���Թ�����

        /// <summary>
        /// �Ƿ������ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasData(string name);

        /// <summary>
        /// ��ȡ��ʱ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        eDataType GetDataType(string name);

        /// <summary>
        /// ���һ����ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool AddData(string name, eDataType type);

        /// <summary>
        /// �Ƴ���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool RemoveData(string name);

        /// <summary>
        /// ����һ��int���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataInt(string name, int val);

        /// <summary>
        /// ����һ��ulong���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataULong(string name, ulong val);

        /// <summary>
        /// ����һ��long���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataLong(string name, long val);

        /// <summary>
        /// ����һ��float���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataFloat(string name, float val);

        /// <summary>
        /// ����һ��ulong���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataDouble(string name, double val);

        /// <summary>
        /// ����һ��bool���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataBool(string name, bool val);

        /// <summary>
        /// ����һ��string���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataString(string name, string val);

        /// <summary>
        /// ����һ��vector2���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataVector2(string name, Vector2 val);

        /// <summary>
        /// ����һ��vector3���͵���ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataVector3(string name, Vector3 val);

        /// <summary>
        /// ��ȡһ����ʱ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetDataInt(string name);

        /// <summary>
        /// ��ȡһ��ulong����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ulong GetDataULong(string name);

        /// <summary>
        /// ��ȡһ��long����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        long GetDataLong(string name);

        /// <summary>
        /// ��ȡһ��float����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        float GetDataFloat(string name);

        /// <summary>
        /// ��ȡһ��double����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        double GetDataDouble(string name);

        /// <summary>
        /// ��ȡһ��bool����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetDataBool(string name);

        /// <summary>
        /// ��ȡһ��string����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetDataString(string name);

        /// <summary>
        /// ��ȡһ��vector2����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Vector2 GetDataVector2(string name);

        /// <summary>
        /// ��ȡһ��vector3����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Vector3 GetDataVector3(string name);

        #endregion

        #region ��������

        /// <summary>
        /// �Ƿ���ڱ��
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasRecord(string name);

        /// <summary>
        /// ��ȡ���
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Record GetRecord(string name);

        /// <summary>
        /// ��ȡ��Ҫ�洢���б������
        /// </summary>
        /// <param name="recList"></param>
        /// <returns></returns>
        public int GetRecordList(out List<string> recList, bool onlySaved);

        #endregion
    }
}