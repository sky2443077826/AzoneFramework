using System.Xml;
using System.Collections.Generic;
using AzoneFramework;
using System;

public class ObjectFactory : Singleton<ObjectFactory>
{
    // ���󴴽���
    private delegate ulong CreateObjectFunc(int config, EMap map, float x, float y, DataList args);
    private Dictionary<eObjectType, CreateObjectFunc> _objectCreator;
    // ���󴴽���
    private delegate ulong CreateObjectFromFunc(EMap map, string data);
    private Dictionary<eObjectType, CreateObjectFromFunc> _objectCreatorFrom;

    // ���󴴽�֮ǰ�¼�
    protected event Action<eObjectType, DataList> _beforeCreatingActions;
    // ���󴴽��¼�
    protected event Action<eObjectType, DataList> _createdActions;
    // ��������֮ǰ�¼�
    protected event Action<eObjectType, DataList> _beforeDestoryingActions;
    // ���������¼�
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
    /// �ڸ��Ӵ���һ������
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
            GameLog.Error($"��Ч�Ķ���ID������ʧ��{config}");
            return 0;
        }

        // ��ȡ����
        eObjectType type = ConfigManager.Instance.GetType(config);
        if (_objectCreator.ContainsKey(type))
        {
            // ����д����ص�����ָ���ص�
            return _objectCreator[type].Invoke(config, map, x, y, args);
        }

        // ���û�лص�����ͨ�ô���
        return CreateCommonObject(config, map, x, y, args);
    }

    /// <summary>
    /// ��xml��������
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

        // ����
        string clsName = node.Name;
        if (string.IsNullOrEmpty(clsName)) { return 0; }

        // ��������
        if (!Enum.TryParse(clsName, out eObjectType type))
        {
            GameLog.Error($"��Ч�Ķ������ͣ���������ʧ�ܡ�{clsName}");
            return 0;
        }

        if (_objectCreatorFrom.ContainsKey(type))
        {
            return _objectCreatorFrom[type].Invoke(map, data);
        }

        // ���û�лص�����ͨ�ô���
        return CreateCommonObjectFrom(map, data);
    }

    /// <summary>
    /// ������ɫ
    /// </summary>
    /// <returns></returns>
    public ulong CreateMainRole()
    {
        // ��ʼ��������
        XmlNode roleNode = StoreManager.Instance.CurSaveData?.roleData;
        if (roleNode == null)
        {
            return 0;
        }

        // ��ȡ��ɫ����ID
        int config = Convert.ToInt32(roleNode.SelectSingleNode("Property/ID").InnerText);
        // ������ɫ���ݶ���
        ulong roleUID = ObjectManager.Instance.CreateObject<RoleObject>(config);
        if (roleUID == 0)
        {
            return 0;
        }

        // ��ȡ��ɫ���ݶ���
        if (!ObjectManager.Instance.TryGetObject(roleUID, out RoleObject role))
        {
            return 0;
        }

        // �ӽڵ��У����л���ɫ
        if (!role.ParseFrom(roleNode.OuterXml))
        {
            GameLog.Error($"�����������ʧ�ܡ�{config}");
            return 0;
        }

        // ���õ�ͼ
        role.SetInt("Map", (int)EMap.Main);

        // ��ɫ�����¼��ɷ�
        DataList args = DataList.Get();
        DispathCreatedEvent(eObjectType.Role, args.AddULong(roleUID));
        args.Dispose();

        return roleUID;
    }

    #region �����������

    /// <summary>
    /// ע����󴴽���
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
    /// ����ͨ�ö���
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
            GameLog.Error($"�����������ʧ��{config}");
            return 0;
        }

        // ��������
        obj.SetInt("Map", (int)map);
        obj.SetFloat("X", x);
        obj.SetFloat("Y", y);

        // �ɷ��¼�
        DataList args2 = DataList.Get().Add(uid);
        DispathCreatedEvent(obj.Type, args2);
        args.Dispose();

        return uid;
    }

    /// <summary>
    /// �������д���ͨ�ö���
    /// </summary>
    /// <param name="map"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private ulong CreateCommonObjectFrom(EMap map, string data)
    {
        return 0;
    }

    #endregion

    #region �ɷ��¼�
    /// <summary>
    /// �ɷ�����֮ǰ�¼�
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="args"></param>
    public virtual void DispathBeforeCreatingEvent(eObjectType type, DataList args)
    {
        _beforeCreatingActions?.Invoke(type, args);
    }

    /// <summary>
    /// �ɷ������¼�
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="args"></param>
    public virtual void DispathCreatedEvent(eObjectType type, DataList args)
    {
        _createdActions?.Invoke(type, args);
    }

    /// <summary>
    /// �ɷ�����֮ǰ�¼�
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="args"></param>
    public virtual void DispathBeforeDestroyingEvent(eObjectType type, DataList args)
    {
        _beforeDestoryingActions?.Invoke(type, args);
    }

    /// <summary>
    /// �ɷ������¼�
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="args"></param>
    public virtual void DispathDestroyedEvent(eObjectType type, DataList args)
    {
        _destroyedActions?.Invoke(type, args);
    }
    #endregion

    #region �¼�������Ƴ�
    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddBeforeCreatingEvent(Action<eObjectType, DataList> action)
    {
        _beforeCreatingActions -= action;
        _beforeCreatingActions += action;
    }

    /// <summary>
    /// �Ƴ������¼�
    /// </summary>
    /// <param name="action"></param>
    public virtual void RemoveBeforeCreatingEvent(Action<eObjectType, DataList> action)
    {
        _beforeCreatingActions -= action;
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddCreatedEvent(Action<eObjectType, DataList> action)
    {
        _createdActions -= action;
        _createdActions += action;
    }

    /// <summary>
    /// �Ƴ������¼�
    /// </summary>
    /// <param name="action"></param>
    public virtual void RemoveCreatedEvent(Action<eObjectType, DataList> action)
    {
        _createdActions -= action;
    }

    /// <summary>
    /// ��������ǰ�¼�
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddBeforeDestroyingEvent(Action<eObjectType, DataList> action)
    {
        _beforeDestoryingActions -= action;
        _beforeDestoryingActions += action;
    }

    /// <summary>
    /// �Ƴ�����ǰ�¼�
    /// </summary>
    /// <param name="action"></param>
    public virtual void RemoveBeforeDestroyingEvent(Action<eObjectType, DataList> action)
    {
        _beforeDestoryingActions -= action;
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddDestroyedEvent(Action<eObjectType, DataList> action)
    {
        _destroyedActions -= action;
        _destroyedActions += action;
    }

    /// <summary>
    /// �Ƴ������¼�
    /// </summary>
    /// <param name="action"></param>
    public virtual void RemoveDestroyedEvent(Action<eObjectType, DataList> action)
    {
        _destroyedActions -= action;
    }
    #endregion
}
