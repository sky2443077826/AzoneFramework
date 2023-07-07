using AzoneFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

/// <summary>
/// ���ݴ洢������
/// </summary>
public class StoreManager : Singleton<StoreManager>
{
    /// <summary>
    /// �浵�����б�
    /// </summary>
    private Dictionary<int, SaveData> _saveDatas;

    /// <summary>
    /// ��ǰ�浵����
    /// </summary>
    public SaveData CurSaveData { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        _saveDatas = new Dictionary<int, SaveData>();


        // ���洢·���Ƿ���ڣ��������򴴽�
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

    #region ���ش浵

    /// <summary>
    /// �������еĴ浵
    /// </summary>
    private void LoadAllSaves()
    {
        // �������з��������Ĵ浵�ļ�
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
                GameLog.Error($"���ش浵����--->�浵��{document.Name}����Save��㲻���ڡ�");
                return;
            }
            if (!LoadSave(save))
            {
                GameLog.Error($"���ش浵����--->�浵��{document.Name}����Save������ݲ��Ϸ���");
                return;
            }
        }
    }

    /// <summary>
    /// ���ش浵��Ϣ
    /// </summary>
    /// <param name="save"></param>
    /// <returns></returns>
    private bool LoadSave(XmlNode save)
    {
        if (save.Attributes == null || save.Attributes.Count <= 0)
        {
            GameLog.Error($"���ش浵����--->�浵Save���û������ֵ��");
            return false;
        }

        SaveData saveData = new SaveData();

        XmlNode nickNameNode = save.SelectSingleNode("NickName");
        XmlNode genderNode = save.SelectSingleNode("Gender");
        XmlNode RIDNode = save.SelectSingleNode("RID");
        XmlNode configNode = save.SelectSingleNode("Config");
        XmlNode lastSaveTimeNode = save.SelectSingleNode("LastSaveTime");

        // �ǳ�
        string nickName = nickNameNode?.InnerText;
        if (string.IsNullOrEmpty(nickName))
        {
            return false;
        }
        saveData.NickName = nickName;

        // ��ɫID
        if (RIDNode == null || RIDNode.InnerText == null)
        {
            return false;
        }
        int rID = ConvertUtility.IntConvert(RIDNode.InnerText);
        saveData.RID = rID;

        // ����
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

        // �ϴα���ʱ��
        if (lastSaveTimeNode == null || lastSaveTimeNode.InnerText == null)
        {
            return false;
        }
        long lastSaveTime = ConvertUtility.LongConvert(lastSaveTimeNode.InnerText);
        saveData.LastSaveTime = lastSaveTime;

        // ��ɫ����
        XmlNode roleNode = save.SelectSingleNode("RoleData");
        if (roleNode == null)
        {
            GameLog.Error($"���ش浵����--->��ɫ���ݲ�����{rID}��");
            return false;
        }
        saveData.roleData = roleNode;

        _saveDatas.Add(rID, saveData);
        return true;
    }

    #endregion

    #region �����浵



    #endregion

    #region д���ļ�

    public void WriteSaveData()
    {
        if (CurSaveData == null)
        {
            GameLog.Error("д��浵ʧ�ܣ�---> ��ǰ�浵����Ϊ�ա�");
            return;
        }

        string saveFile = $"{GameConstant.STORE_SAVE_PATH}/{CurSaveData.NickName}_{CurSaveData.RID}.xml";

        XmlDocument xmlDocument = new XmlDocument();
        XmlNode decNode = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDocument.AppendChild(decNode);
        // �������ڵ�
        XmlElement rootNode = xmlDocument.CreateElement("XML");

        // д��浵��Ϣ   
        XmlNode saveNode = rootNode.AppendChild(xmlDocument.CreateElement("Save"));

        // �ǳ�
        XmlNode nickNode = xmlDocument.CreateElement("NickName");
        nickNode.InnerText = CurSaveData.NickName;
        saveNode.AppendChild(nickNode);

        // ��ɫID
        XmlNode rIDNode = xmlDocument.CreateElement("RID");
        rIDNode.InnerText = XmlConvert.ToString(CurSaveData.RID);
        saveNode.AppendChild(rIDNode);

        // �ϴα���ʱ��
        XmlNode lastSaveTimeNode = xmlDocument.CreateElement("LastSaveTime");
        lastSaveTimeNode.InnerText = XmlConvert.ToString(CurSaveData.LastSaveTime);
        saveNode.AppendChild(lastSaveTimeNode);

        // д���ɫ����


        // д���ļ�
        byte[] data = new UTF8Encoding(false).GetBytes(xmlDocument.OuterXml);
        File.WriteAllBytes(saveFile, data);
    }

    #endregion
}
