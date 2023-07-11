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

        // ��ʼ��ģ��
        InitManager.Instance.Create();

        LoadAllSaves();
    }

    protected override void OnDispose()
    {
        base.OnDispose();

        InitManager.Instance.Dispose();

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
        XmlElement roleNode = save.SelectSingleNode("RoleData") as XmlElement;
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

    /// <summary>
    /// �����´浵
    /// </summary>
    public void CreateNewSave(string nickName, eGender gender)
    {
        if (string.IsNullOrEmpty(nickName))
        {
            GameLog.Error($"�����´浵ʧ�ܣ�---> ��ɫ���ơ�{nickName}�����Ϸ���");
            return;
        }

        SaveData saveData = new SaveData();
        saveData.NickName = nickName;
        saveData.RID = DataUtility.GenerateRID();

        // �����Ա��ȡָ��������ID
        int config = DataUtility.Instance.GetRoleConfig(gender);
        if (config == 0)
        {
            GameLog.Error("û��Ĭ�ϵĽ�ɫ����ʹ�á�");
            return;
        }
        saveData.Config = config;

        // ������ɫ


        _saveDatas[saveData.RID] = saveData;
        WriteSaveData(saveData.RID);
    }

    /// <summary>
    /// ���л���ɫ��Ϣ
    /// </summary>
    /// <param name="root"></param>
    private bool SerializeRole(SaveData saveData)
    {
        if (saveData == null || saveData.roleData == null) return false;

        /*
         *      ���л���ɫ�����Լ����
         */

        // ������ɫ
        RoleObject roleObject = new RoleObject();

        // ��ʼ����������
        roleObject.Init(saveData.Config);
        roleObject.SetString("NickName", saveData.NickName);
        roleObject.SetInt("RID", saveData.RID);
        roleObject.SetLong("LastSave", DateTimeOffset.Now.ToUnixTimeSeconds());

        // ���л���ɫ
        roleObject.SerializeToXml(saveData.roleData);

        /*
         *      ��ʼ�����������Ϣ
         */
        RecordManager initRecMgr = InitManager.Instance.InitRecMgr;
        if (initRecMgr != null)
        {
            // ��ȡ�����
            initRecMgr.GetRecordList(out List<string> records, true);
            foreach (string recName in records)
            {
                if (string.IsNullOrEmpty(recName)) continue;

                // ��ȡ���ڵ�
                XmlNode recNode = saveData.roleData.SelectSingleNode("Role/Record/" + recName) as XmlElement;
                if (recNode == null) continue;

                // ��ȡ���ṹ
                Record rec = initRecMgr.GetRecord(recName);
                if (rec == null) continue;

                for (int row = 0; row < rec.MaxRowCount; ++row)
                {
                    if (!rec.CheckRowUse(row)) continue;

                    // ��ȡ�������
                    DataList rowData = rec.GetRowData(row);
                    XmlElement rowNode = saveData.roleData.OwnerDocument.CreateElement("Row");

                    for (int index = 0; index < rowData.Count; ++index)
                    {
                        VariableData data = rowData.ReadVar(index);
                        // ��ȡtag
                        string tag = rec.GetTagByColumn(index);
                        if (string.IsNullOrEmpty(tag) || data == null) continue;

                        rowNode.SetAttribute(tag, data.ToString());
                    }

                    recNode.AppendChild(rowNode);
                }
            }
        }

        /*
         *      ���л���ɫ����
         */
        // ��ȡ��ɫ�ڵ�
        //XmlElement roleNode = root.SelectSingleNode("Role") as XmlElement;
        //if (roleNode == null) return false;
        //Inventory bag = new Inventory();
        //int bagID = DataUtility.Instance.GetViewPortConfig(eViewPort.Inventory);
        //bag.Init(bagID);

        //XmlElement bagNode = root.OwnerDocument.CreateElement(bag.GetString("Class"));
        //// ��ʼ����������
        //bag.SerializeProperty(bagNode);

        //foreach (var kv in QSInit.Instance.DefaultItems)
        //{
        //    // ����id
        //    int itemID = kv.Key;
        //    // ��������
        //    int itemCount = kv.Value;
        //    if (itemCount <= 0)
        //    {
        //        GameLog.Error($"����{itemID}����Ϊ0����ʼ����ɫ��ӱ�������ʧ��");
        //        continue;
        //    }
        //    DataObject item = new DataObject();
        //    item.Init(itemID);
        //    item.SetInt("Count", itemCount);
        //    item.SerializeToXml(bagNode);
        //}

        //// ������������ӵ���ɫ
        //roleNode.AppendChild(bagNode);

        /*
         *      װ����ͼ
         */
        // todo

        /*
         *      ����������ͼ/�����ͼ
         */
        /*
        SkillBox skillBox = new SkillBox();
        skillBox.Init(QSCommon.Instance.GetViewPortConfig(eViewPort.SkillBox));
        XmlElement skillBoxNode = root.OwnerDocument.CreateElement(skillBox.GetString("Class"));
        skillBox.SerializeProperty(skillBoxNode);
        roleNode.AppendChild(skillBoxNode);

        CursorBox cursorBox = new CursorBox();
        cursorBox.Init(QSCommon.Instance.GetViewPortConfig(eViewPort.CursorBox));
        XmlElement cursorBoxNode = root.OwnerDocument.CreateElement(cursorBox.GetString("Class"));
        cursorBox.SerializeProperty(cursorBoxNode);
        roleNode.AppendChild(cursorBoxNode);
        */
        return true;
    }

    public void WriteSaveData(int RID)
    {
        if (!_saveDatas.TryGetValue(RID, out SaveData saveData))
        {
            GameLog.Error($"д��浵ʧ�ܣ�---> ��ǰ�浵���ݡ�{RID}��Ϊ�ա�");
            return;
        }

        string saveFile = $"{GameConstant.STORE_SAVE_PATH}/{CurSaveData.NickName}_{CurSaveData.RID}.xml";

        // �����ĵ����
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
