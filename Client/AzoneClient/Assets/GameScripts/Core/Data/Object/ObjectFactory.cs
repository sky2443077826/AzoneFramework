using System.Xml;
using System.Collections.Generic;
using AzoneFramework;
using System;

public class ObjectFactory : Singleton<ObjectFactory>
{
    // 对象创建器
    private delegate ulong CreateObjectFunc(int config, EMap map, float x, float y, DataList args);
    private Dictionary<eObjectType, CreateObjectFunc> _objectCreator;
    // 对象创建器
    private delegate ulong CreateObjectFromFunc(EMap map, string data);
    private Dictionary<eObjectType, CreateObjectFromFunc> _objectCreatorFrom;

    // 对象创建之前事件
    protected event Action<eObjectType, DataList> _beforeCreatingActions;
    // 对象创建事件
    protected event Action<eObjectType, DataList> _createdActions;
    // 对象销毁之前事件
    protected event Action<eObjectType, DataList> _beforeDestoryingActions;
    // 对象销毁事件
    protected event Action<eObjectType, DataList> _destroyedActions;

    protected override void OnCreate()
    {
        base.OnCreate();
        BindObjectCreator();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }

    /// <summary>
    /// 在格子创建一个对象
    /// </summary>
    /// <param name="config"></param>
    /// <param name="map"></param>
    /// <param name="pos"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public ulong CreateObject(int config, EMap map, float x, float y, DataList args)
    {
        if (!ConfigManager.Instance.HasConfig(config))
        {
            GameLog.Error($"无效的对象ID，创建失败{config}");
            return 0;
        }

        // 获取类型
        eObjectType type = ConfigManager.Instance.GetType(config);
        if (_objectCreator.ContainsKey(type))
        {
            // 如果有创建回调，走指定回调
            return _objectCreator[type].Invoke(config, map, x, y, args);
        }

        // 如果没有回调，走通用创建
        return CreateCommonObject(config, map, x, y, args);
    }

    /// <summary>
    /// 从xml创建对象
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public ulong CreateObjectFrom(EMap map, string data)
    {
        // if (node == null) { return 0; }
        if (string.IsNullOrEmpty(data)) { return 0; }

        XmlDocument doc = new XmlDocument();

        doc.LoadXml(data);
        XmlNode node = doc.DocumentElement;

        // 类型
        string clsName = node.Name;
        if (string.IsNullOrEmpty(clsName)) { return 0; }

        // 解析类型
        if (!Enum.TryParse(clsName, out eObjectType type))
        {
            GameLog.Error($"无效的对象类型，解析类型失败。{clsName}");
            return 0;
        }

        if (_objectCreatorFrom.ContainsKey(type))
        {
            return _objectCreatorFrom[type].Invoke(map, data);
        }

        // 如果没有回调，走通用创建
        return CreateCommonObjectFrom(map, data);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <returns></returns>
    public ulong CreateMainRole()
    {
        // 开始加载配置
        XmlNode roleNode = StoreManager.Instance.CurSaveData?.roleData;
        if (roleNode == null)
        {
            return 0;
        }

        // 获取角色配置ID
        int config = Convert.ToInt32(roleNode.SelectSingleNode("Property/ID").InnerText);
        // 创建角色数据对象
        ulong roleUID = ObjectManager.Instance.CreateObject<RoleObject>(config);
        if (roleUID == 0)
        {
            return 0;
        }

        // 获取角色数据对象
        if (!ObjectManager.Instance.TryGetObject(roleUID, out RoleObject role))
        {
            return 0;
        }

        // 从节点中，序列化角色
        if (!role.ParseFrom(roleNode.OuterXml))
        {
            GameLog.Error($"解析玩家数据失败。{config}");
            return 0;
        }

        // 设置地图
        role.SetInt("Map", (int)EMap.Main);

        // 角色创建事件派发
        DataList args = DataList.Get();
        DispathCreatedEvent(eObjectType.Role, args.AddULong(roleUID));
        args.Dispose();

        return roleUID;
    }

    #region 创建具体对象

    /// <summary>
    /// 注册对象创建器
    /// </summary>
    private void BindObjectCreator()
    {
        _objectCreator = new Dictionary<eObjectType, CreateObjectFunc>()
        {
        };

        _objectCreatorFrom = new Dictionary<eObjectType, CreateObjectFromFunc>()
        {
        };
    }

    /// <summary>
    /// 创建通用对象
    /// </summary>
    /// <param name="config"></param>
    /// <param name="map"></param>
    /// <param name="pos"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private ulong CreateCommonObject(int config, EMap map, float x, float y, DataList args)
    {
        if (!ConfigManager.Instance.HasConfig(config)) { return 0; }

        ulong uid = ObjectManager.Instance.CreateObject<DataObject>(config);
        if (uid == 0) { return 0; }

        if (!ObjectManager.Instance.TryGetObject(uid, out DataObject obj))
        {
            GameLog.Error($"创建场景物件失败{config}");
            return 0;
        }

        // 设置属性
        obj.SetInt("Map", (int)map);
        obj.SetFloat("X", x);
        obj.SetFloat("Y", y);

        // 派发事件
        DataList args2 = DataList.Get().Add(uid);
        DispathCreatedEvent(obj.Type, args2);
        args.Dispose();

        return uid;
    }

    /// <summary>
    /// 从配置中创建通用对象
    /// </summary>
    /// <param name="map"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private ulong CreateCommonObjectFrom(EMap map, string data)
    {
        return 0;
    }

    #endregion

    #region 派发事件
    /// <summary>
    /// 派发创建之前事件
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="args"></param>
    public virtual void DispathBeforeCreatingEvent(eObjectType type, DataList args)
    {
        _beforeCreatingActions?.Invoke(type, args);
    }

    /// <summary>
    /// 派发创建事件
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="args"></param>
    public virtual void DispathCreatedEvent(eObjectType type, DataList args)
    {
        _createdActions?.Invoke(type, args);
    }

    /// <summary>
    /// 派发销毁之前事件
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="args"></param>
    public virtual void DispathBeforeDestroyingEvent(eObjectType type, DataList args)
    {
        _beforeDestoryingActions?.Invoke(type, args);
    }

    /// <summary>
    /// 派发销毁事件
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="args"></param>
    public virtual void DispathDestroyedEvent(eObjectType type, DataList args)
    {
        _destroyedActions?.Invoke(type, args);
    }
    #endregion

    #region 事件添加与移除
    /// <summary>
    /// 监听创建事件
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddBeforeCreatingEvent(Action<eObjectType, DataList> action)
    {
        _beforeCreatingActions -= action;
        _beforeCreatingActions += action;
    }

    /// <summary>
    /// 移除监听事件
    /// </summary>
    /// <param name="action"></param>
    public virtual void RemoveBeforeCreatingEvent(Action<eObjectType, DataList> action)
    {
        _beforeCreatingActions -= action;
    }

    /// <summary>
    /// 监听创建事件
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddCreatedEvent(Action<eObjectType, DataList> action)
    {
        _createdActions -= action;
        _createdActions += action;
    }

    /// <summary>
    /// 移除监听事件
    /// </summary>
    /// <param name="action"></param>
    public virtual void RemoveCreatedEvent(Action<eObjectType, DataList> action)
    {
        _createdActions -= action;
    }

    /// <summary>
    /// 监听销毁前事件
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddBeforeDestroyingEvent(Action<eObjectType, DataList> action)
    {
        _beforeDestoryingActions -= action;
        _beforeDestoryingActions += action;
    }

    /// <summary>
    /// 移除销毁前事件
    /// </summary>
    /// <param name="action"></param>
    public virtual void RemoveBeforeDestroyingEvent(Action<eObjectType, DataList> action)
    {
        _beforeDestoryingActions -= action;
    }

    /// <summary>
    /// 监听销毁事件
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddDestroyedEvent(Action<eObjectType, DataList> action)
    {
        _destroyedActions -= action;
        _destroyedActions += action;
    }

    /// <summary>
    /// 移除销毁事件
    /// </summary>
    /// <param name="action"></param>
    public virtual void RemoveDestroyedEvent(Action<eObjectType, DataList> action)
    {
        _destroyedActions -= action;
    }
    #endregion
}
