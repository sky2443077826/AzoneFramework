using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{

    public interface IDataManager
    {
        /// <summary>
        /// 是否存在一个数据
        /// </summary>
        /// <param name="name">数据名</param>
        /// <returns></returns>
        bool Find(string name);

        /// <summary>
        /// 添加一个数据
        /// </summary>
        /// <param name="name">数据名</param>
        /// <param name="data">数据值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool Add(string name, Property data);

        /// <summary>
        /// 移除一个数据
        /// </summary>
        /// <param name="name">数据名</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool Remove(string name);

        /// <summary>
        /// 获取一个数据
        /// </summary>
        /// <param name="name">属性</param>
        /// <returns>属性类型</returns>
        Property GetProperty(string name);

        /// <summary>
        /// 获取数据数量
        /// </summary>
        /// <returns>数据个数</returns>
        int Count();

        /// <summary>
        /// 获取指定属性的类型
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性类型</returns>
        eDataType GetType(string name);

        /// <summary>
        /// 获取int类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        int GetInt(string name);

        /// <summary>
        /// 获取long类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        long GetLong(string name);

        /// <summary>
        /// 获取ulong类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        ulong GetULong(string name);

        /// <summary>
        /// 获取float类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        float GetFloat(string name);

        /// <summary>
        /// 获取double类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        double GetDouble(string name);

        /// <summary>
        /// 获取bool类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        bool GetBool(string name);

        /// <summary>
        /// 获取string类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        string GetString(string name);

        /// <summary>
        /// 获取vector2类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        Vector2 GetVector2(string name);

        /// <summary>
        /// 获取vector3类型数据
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        Vector3 GetVector3(string name);

        /// <summary>
        /// 设置int属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetInt(string name, int val);

        /// <summary>
        /// 设置long属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetLong(string name, long val);

        /// <summary>
        /// 设置ulong属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetULong(string name, ulong val);

        /// <summary>
        /// 设置float属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetFloat(string name, float val);

        /// <summary>
        /// 设置doule属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetDouble(string name, double val);

        /// <summary>
        /// 设置bool属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetBool(string name, bool val);

        /// <summary>
        /// 设置string属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetString(string name, string val);

        /// <summary>
        /// 设置vector2属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetVector2(string name, Vector2 val);

        /// <summary>
        /// 设置vector3属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool SetVector3(string name, Vector3 val);
    }
}
