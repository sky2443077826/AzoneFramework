using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{

    public interface IDataManager
    {
        /// <summary>
        /// �Ƿ����һ������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns></returns>
        bool Find(string name);

        /// <summary>
        /// ���һ������
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="data">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool Add(string name, Property data);

        /// <summary>
        /// �Ƴ�һ������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool Remove(string name);

        /// <summary>
        /// ��ȡһ������
        /// </summary>
        /// <param name="name">����</param>
        /// <returns>��������</returns>
        Property GetProperty(string name);

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <returns>���ݸ���</returns>
        int Count();

        /// <summary>
        /// ��ȡָ�����Ե�����
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>��������</returns>
        eDataType GetType(string name);

        /// <summary>
        /// ��ȡint��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        int GetInt(string name);

        /// <summary>
        /// ��ȡlong��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        long GetLong(string name);

        /// <summary>
        /// ��ȡulong��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        ulong GetULong(string name);

        /// <summary>
        /// ��ȡfloat��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        float GetFloat(string name);

        /// <summary>
        /// ��ȡdouble��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        double GetDouble(string name);

        /// <summary>
        /// ��ȡbool��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        bool GetBool(string name);

        /// <summary>
        /// ��ȡstring��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        string GetString(string name);

        /// <summary>
        /// ��ȡvector2��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        Vector2 GetVector2(string name);

        /// <summary>
        /// ��ȡvector3��������
        /// </summary>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
        Vector3 GetVector3(string name);

        /// <summary>
        /// ����int����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetInt(string name, int val);

        /// <summary>
        /// ����long����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetLong(string name, long val);

        /// <summary>
        /// ����ulong����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetULong(string name, ulong val);

        /// <summary>
        /// ����float����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetFloat(string name, float val);

        /// <summary>
        /// ����doule����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetDouble(string name, double val);

        /// <summary>
        /// ����bool����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetBool(string name, bool val);

        /// <summary>
        /// ����string����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetString(string name, string val);

        /// <summary>
        /// ����vector2����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetVector2(string name, Vector2 val);

        /// <summary>
        /// ����vector3����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="val">����ֵ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        bool SetVector3(string name, Vector3 val);
    }
}
