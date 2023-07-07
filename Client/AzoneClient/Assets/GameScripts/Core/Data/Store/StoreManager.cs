using AzoneFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

/// <summary>
/// 数据存储管理器
/// </summary>
public class StoreManager : Singleton<StoreManager>
{
    /// <summary>
    /// 存档数据列表
    /// </summary>
    private Dictionary<int, SaveData> _saveDatas;

    /// <summary>
    /// 当前存档数据
    /// </summary>
    public SaveData CurSaveData { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        _saveDatas = new Dictionary<int, SaveData>();


        // 检测存储路径是否存在，不存在则创建
        if (!Directory.Exists(GameConstant.STORE_SAVE_PATH))
        {
            Directory.CreateDirectory(GameConstant.STORE_SAVE_PATH);
            return;
        }

        LoadAllSaves();
    }

    protected override void OnDispose()
    {
        base.OnDispose();

        _saveDatas.Clear();
        _saveDatas = null;

        CurSaveData = null;
    }

    #region 加载存档

    /// <summary>
    /// 加载所有的存档
    /// </summary>
    private void LoadAllSaves()
    {
        // 加载所有符合条件的存档文件
        string[] fileNames = Directory.GetFiles(GameConstant.STORE_SAVE_PATH, "*.xml", SearchOption.TopDirectoryOnly);
        if (fileNames.Length <= 0)
        {
            return;
        }

        foreach (var fileName in fileNames)
        {
            if (!fileName.EndsWith(".xml"))
            {
                continue;
            }

            XmlDocument document = new XmlDocument();
            document.Load(fileName);
            XmlNode save = document.DocumentElement.SelectSingleNode("Save");
            if (save == null)
            {
                GameLog.Error($"加载存档错误！--->存档【{document.Name}】的Save结点不存在。");
                return;
            }
            if (!LoadSave(save))
            {
                GameLog.Error($"加载存档错误！--->存档【{document.Name}】的Save结点数据不合法。");
                return;
            }
        }
    }

    /// <summary>
    /// 加载存档信息
    /// </summary>
    /// <param name="save"></param>
    /// <returns></returns>
    private bool LoadSave(XmlNode save)
    {
        if (save.Attributes == null || save.Attributes.Count <= 0)
        {
            GameLog.Error($"加载存档错误！--->存档Save结点没有属性值。");
            return false;
        }

        SaveData saveData = new SaveData();

        XmlNode nickNameNode = save.SelectSingleNode("NickName");
        XmlNode genderNode = save.SelectSingleNode("Gender");
        XmlNode RIDNode = save.SelectSingleNode("RID");
        XmlNode configNode = save.SelectSingleNode("Config");
        XmlNode lastSaveTimeNode = save.SelectSingleNode("LastSaveTime");

        // 昵称
        string nickName = nickNameNode?.InnerText;
        if (string.IsNullOrEmpty(nickName))
        {
            return false;
        }
        saveData.NickName = nickName;

        // 角色ID
        if (RIDNode == null || RIDNode.InnerText == null)
        {
            return false;
        }
        int rID = ConvertUtility.IntConvert(RIDNode.InnerText);
        saveData.RID = rID;

        // 配置
        if (configNode == null || configNode.InnerText == null)
        {
            return false;
        }
        int config = ConvertUtility.IntConvert(configNode.InnerText);
        if (!ConfigManager.Instance.HasConfig(config))
        {
            return false;
        }
        saveData.Config = config;

        // 上次保存时间
        if (lastSaveTimeNode == null || lastSaveTimeNode.InnerText == null)
        {
            return false;
        }
        long lastSaveTime = ConvertUtility.LongConvert(lastSaveTimeNode.InnerText);
        saveData.LastSaveTime = lastSaveTime;

        // 角色数据
        XmlNode roleNode = save.SelectSingleNode("RoleData");
        if (roleNode == null)
        {
            GameLog.Error($"加载存档错误！--->角色数据不存在{rID}。");
            return false;
        }
        saveData.roleData = roleNode;

        _saveDatas.Add(rID, saveData);
        return true;
    }

    #endregion

    #region 解析存档



    #endregion

    #region 写入文件

    public void WriteSaveData()
    {
        if (CurSaveData == null)
        {
            GameLog.Error("写入存档失败！---> 当前存档数据为空。");
            return;
        }

        string saveFile = $"{GameConstant.STORE_SAVE_PATH}/{CurSaveData.NickName}_{CurSaveData.RID}.xml";

        XmlDocument xmlDocument = new XmlDocument();
        XmlNode decNode = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDocument.AppendChild(decNode);
        // 创建根节点
        XmlElement rootNode = xmlDocument.CreateElement("XML");

        // 写入存档信息   
        XmlNode saveNode = rootNode.AppendChild(xmlDocument.CreateElement("Save"));

        // 昵称
        XmlNode nickNode = xmlDocument.CreateElement("NickName");
        nickNode.InnerText = CurSaveData.NickName;
        saveNode.AppendChild(nickNode);

        // 角色ID
        XmlNode rIDNode = xmlDocument.CreateElement("RID");
        rIDNode.InnerText = XmlConvert.ToString(CurSaveData.RID);
        saveNode.AppendChild(rIDNode);

        // 上次保存时间
        XmlNode lastSaveTimeNode = xmlDocument.CreateElement("LastSaveTime");
        lastSaveTimeNode.InnerText = XmlConvert.ToString(CurSaveData.LastSaveTime);
        saveNode.AppendChild(lastSaveTimeNode);

        // 写入角色数据


        // 写入文件
        byte[] data = new UTF8Encoding(false).GetBytes(xmlDocument.OuterXml);
        File.WriteAllBytes(saveFile, data);
    }

    #endregion
}
