using System.Xml;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    // 属性改变回调
    public delegate void ObjectPropChangedCallback(DataList args);
    // 表格改变回调
    public delegate void ObjectRecordChangedCallback(DataList args);

    /// <summary>
    /// 所有数据对象的基类接口
    /// </summary>
    public interface IDataObject : ISerializable
    {
        // 配置ID
        int ConfigID { get; set; }
        // 唯一ID
        ulong UID { get; set; }
        // 子对象索引
        int Pos { get; set; }
        // 父对象
        ulong Parent { get; set; }
        // 容量
        int Capacity { get; set; }
        // 对象类型
        eObjectType Type { get; set; }

        // 属性改变回调
        ObjectPropChangedCallback PropChagnedCallback { get; set; }

        /// <summary>
        /// 初始化数据对象
        /// </summary>
        /// <param name="configID">配置ID</param>
        /// <returns></returns>
        bool Init(int configID);

        /// <summary>
        /// 数据对象销毁
        /// </summary>
        void Dispose();

        /// <summary>
        /// 设置属性回调
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        void SetPropChangedCallback(ObjectPropChangedCallback func);

        /// <summary>
        /// 是否是玩家
        /// </summary>
        /// <returns></returns>
        bool IsRole();

        #region 游戏子对象    
        /// <summary>
        /// 子对象数量
        /// </summary>
        int ChildCount();

        /// <summary>
        /// 检查子对象是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool FindChild(int pos);

        /// <summary>
        /// 检查子对象是否存在
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool FindChild(IDataObject obj);

        /// <summary>
        /// 添加一个子对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool AddChild(IDataObject obj, int pos = -1);

        /// <summary>
        /// 创建子对象
        /// </summary>
        /// <returns></returns>
        ulong CreateChild(int config, int pos = -1);

        /// <summary>
        /// 创建子对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        ulong CreateChild<T>(int config, int pos) where T : class, IDataObject, new();

        /// <summary>
        /// 移除指定uid对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool RemoveChild(int pos);

        /// <summary>
        /// 移除子对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool RemoveChild(IDataObject obj);

        /// <summary>
        /// 根据位置获取对象
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        ulong GetChild(int pos);

        /// <summary>
        /// 根据confiid获取第一个子对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        ulong GetFirstChild(int config);

        /// <summary>
        /// 根据类型获取第一个子对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ulong GetFirstChild(eObjectType type);

        /// <summary>
        /// 根据configID获取所有子对象
        /// </summary>
        /// <param name="config"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        int GetChildren(int config, out List<ulong> children);

        /// <summary>
        /// 根据类型获取所有子对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        int GetChildren(eObjectType type, out List<ulong> children);

        #endregion

        #region 属性管理器

        /// <summary>
        /// 获取需要存储的属性的名字
        /// </summary>
        /// <param name="propList"></param>
        /// <returns></returns>
        int GetPropertyList(ref List<string> propList, bool saved);

        /// <summary>
        /// 获取属性类型
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        eDataType GetPropertyType(string name);

        /// <summary>
        /// 是否存在属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasProperty(string name);

        #region  Set操作集
        /// <summary>
        /// 设置int类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetInt(string name, int val);

        /// <summary>
        /// 设置long类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetLong(string name, long val);

        /// <summary>
        /// 设置ulong类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetULong(string name, ulong val);

        /// <summary>
        /// 设置float类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetFloat(string name, float val);

        /// <summary>
        /// 设置double类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetDouble(string name, double val);

        /// <summary>
        /// 设置string类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetString(string name, string val);

        /// <summary>
        /// 设置bool类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetBool(string name, bool val);

        /// <summary>
        /// 设置vector2属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetVector2(string name, Vector2 val);

        /// <summary>
        /// 设置vector3类型属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="val">属性值</param>
        void SetVector3(string name, Vector3 val);

        #endregion

        #region Get操作集
        /// <summary>
        /// 获取int属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        int GetInt(string name);

        /// <summary>
        /// 获取long属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        long GetLong(string name);

        /// <summary>
        /// 获取ulong属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        ulong GetULong(string name);

        /// <summary>
        /// 获取float属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        float GetFloat(string name);

        /// <summary>
        /// 获取double属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        double GetDouble(string name);

        /// <summary>
        /// 获取string属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        string GetString(string name);

        /// <summary>
        /// 获取bool属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        bool GetBool(string name);

        /// <summary>
        /// 获取vector属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        Vector2 GetVector2(string name);

        /// <summary>
        /// 获取vector3属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        Vector3 GetVector3(string name);
        #endregion
        #endregion

        #region 临时属性管理器

        /// <summary>
        /// 是否存在临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasData(string name);

        /// <summary>
        /// 获取临时数据类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        eDataType GetDataType(string name);

        /// <summary>
        /// 添加一个临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool AddData(string name, eDataType type);

        /// <summary>
        /// 移除临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool RemoveData(string name);

        /// <summary>
        /// 设置一个int类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataInt(string name, int val);

        /// <summary>
        /// 设置一个ulong类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataULong(string name, ulong val);

        /// <summary>
        /// 设置一个long类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataLong(string name, long val);

        /// <summary>
        /// 设置一个float类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataFloat(string name, float val);

        /// <summary>
        /// 设置一个ulong类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataDouble(string name, double val);

        /// <summary>
        /// 设置一个bool类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataBool(string name, bool val);

        /// <summary>
        /// 设置一个string类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataString(string name, string val);

        /// <summary>
        /// 设置一个vector2类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataVector2(string name, Vector2 val);

        /// <summary>
        /// 设置一个vector3类型的临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetDataVector3(string name, Vector3 val);

        /// <summary>
        /// 获取一个临时属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetDataInt(string name);

        /// <summary>
        /// 获取一个ulong数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ulong GetDataULong(string name);

        /// <summary>
        /// 获取一个long数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        long GetDataLong(string name);

        /// <summary>
        /// 获取一个float数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        float GetDataFloat(string name);

        /// <summary>
        /// 获取一个double数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        double GetDataDouble(string name);

        /// <summary>
        /// 获取一个bool数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetDataBool(string name);

        /// <summary>
        /// 获取一个string数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetDataString(string name);

        /// <summary>
        /// 获取一个vector2数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Vector2 GetDataVector2(string name);

        /// <summary>
        /// 获取一个vector3数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Vector3 GetDataVector3(string name);

        #endregion

        #region 表格管理器

        /// <summary>
        /// 是否存在表格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasRecord(string name);

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Record GetRecord(string name);

        /// <summary>
        /// 获取需要存储的列表的名字
        /// </summary>
        /// <param name="recList"></param>
        /// <returns></returns>
        public int GetRecordList(out List<string> recList, bool onlySaved);

        #endregion
    }
}