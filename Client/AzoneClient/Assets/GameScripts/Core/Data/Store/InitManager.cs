using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AzoneFramework;
using System.Xml;

/// <summary>
/// ��ʼ��������
/// </summary>
public class InitManager : Singleton<InitManager>
{
    /// <summary>
    /// ��ʼ��������
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
    /// ���س�ʼ������
    /// </summary>
    private void LoadInitConfig()
    {
        string classMapPath = StringUtility.Concat(ApplicationPath.configPath, GameConstant.ROLE_INIT_DATA_CONFIG_PATH);
        XmlDocument doc = new XmlDocument();
        doc.Load(classMapPath);

        // ��һ��Ԫ��
        XmlElement rootNode = doc.DocumentElement;

        //XmlNode inventoryNode = rootNode.SelectSingleNode("Inventory");
        //if (inventoryNode != null)
        //{
        //    // ��������
        //    LoadInventory(inventoryNode);
        //}

        // ���س�ʼ�����
        XmlNode recordNode = rootNode.SelectSingleNode("Record");
        if (recordNode != null)
        {
            // ��ʼ�����
            LoadRecord(recordNode);
        }

        GameLog.Normal("===���ؽ�ɫ��ʼ���������===");
    }

    /// <summary>
    /// ���س�ʼ�����
    /// </summary>
    /// <param name="recordNode"></param>
    private void LoadRecord(XmlNode recordNode)
    {
        if (recordNode == null) return;

        // ��ȡ��������
        if (!ConfigManager.Instance.GetClassRecMgr("Role", out RecordManager initRecMgr)) return;
        if (initRecMgr == null) return;

        foreach (XmlNode node in recordNode.ChildNodes)
        {
            // ��ȡ�����
            string recName = node.Name;
            if (string.IsNullOrEmpty(recName)) continue;

            // ��ȡ���
            Record rec = initRecMgr.GetRecord(recName);
            if (rec == null) continue;

            Record newRec = new Record(rec);

            // ���ʼ��������ӱ�
            InitRecMgr.Add(recName, newRec);

            int row = newRec.AddRow();
            if (row == -1) continue;

            // ��ʼ������
            foreach (XmlNode rowNode in node.ChildNodes)
            {
                if (rowNode == null) continue;
                // ��ȡtagֵ
                foreach (XmlAttribute attr in rowNode.Attributes)
                {
                    // ��ȡname�Լ�value
                    string tag = attr.Name;
                    string value = attr.Value;

                    if (string.IsNullOrEmpty(tag)) continue;
                    // �Ƿ�����Ч��tag
                    if (!newRec.CheckColumnValid(tag)) continue;
                    // ��ȡ������
                    newRec.FromString(row, tag, value);
                }
            }
        }
    }
}
