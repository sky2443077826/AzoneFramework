using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using System.Xml;

/// <summary>
/// 初始化管理器
/// </summary>
public class InitManager : Singleton<InitManager>
{
    /// <summary>
    /// 初始表格管理器
    /// </summary>
    public RecordManager InitRecMgr { get; private set; }



    protected override void OnCreate()
    {
        base.OnCreate();

        InitRecMgr = new RecordManager();
        LoadInitConfig();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }

    /// <summary>
    /// 加载初始化配置
    /// </summary>
    private void LoadInitConfig()
    {
        string classMapPath = StringUtility.Concat(ApplicationPath.configPath, GameConstant.ROLE_INIT_DATA_CONFIG_PATH);
        XmlDocument doc = new XmlDocument();
        doc.Load(classMapPath);

        // 第一个元素
        XmlElement rootNode = doc.DocumentElement;

        //XmlNode inventoryNode = rootNode.SelectSingleNode("Inventory");
        //if (inventoryNode != null)
        //{
        //    // 背包道具
        //    LoadInventory(inventoryNode);
        //}

        // 加载初始化表格
        XmlNode recordNode = rootNode.SelectSingleNode("Record");
        if (recordNode != null)
        {
            // 初始化表格
            LoadRecord(recordNode);
        }

        GameLog.Normal("===加载角色初始化配置完成===");
    }

    /// <summary>
    /// 加载初始化表格
    /// </summary>
    /// <param name="recordNode"></param>
    private void LoadRecord(XmlNode recordNode)
    {
        if (recordNode == null) return;

        // 获取表格管理器
        if (!ConfigManager.Instance.GetClassRecMgr("Role", out RecordManager initRecMgr)) return;
        if (initRecMgr == null) return;

        foreach (XmlNode node in recordNode.ChildNodes)
        {
            // 获取表格名
            string recName = node.Name;
            if (string.IsNullOrEmpty(recName)) continue;

            // 获取表格
            Record rec = initRecMgr.GetRecord(recName);
            if (rec == null) continue;

            Record newRec = new Record(rec);

            // 向初始化表中添加表
            InitRecMgr.Add(recName, newRec);

            int row = newRec.AddRow();
            if (row == -1) continue;

            // 初始化内容
            foreach (XmlNode rowNode in node.ChildNodes)
            {
                if (rowNode == null) continue;
                // 获取tag值
                foreach (XmlAttribute attr in rowNode.Attributes)
                {
                    // 获取name以及value
                    string tag = attr.Name;
                    string value = attr.Value;

                    if (string.IsNullOrEmpty(tag)) continue;
                    // 是否是有效的tag
                    if (!newRec.CheckColumnValid(tag)) continue;
                    // 获取列类型
                    newRec.FromString(row, tag, value);
                }
            }
        }
    }
}
