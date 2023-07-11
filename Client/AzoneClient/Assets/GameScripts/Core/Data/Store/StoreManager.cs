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

        // 初始化模块
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
        XmlElement roleNode = save.SelectSingleNode("RoleData") as XmlElement;
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

    /// <summary>
    /// 创建新存档
    /// </summary>
    public void CreateNewSave(string nickName, eGender gender)
    {
        if (string.IsNullOrEmpty(nickName))
        {
            GameLog.Error($"创建新存档失败！---> 角色名称【{nickName}】不合法。");
            return;
        }

        SaveData saveData = new SaveData();
        saveData.NickName = nickName;
        saveData.RID = DataUtility.GenerateRID();

        // 根据性别获取指定的配置ID
        int config = DataUtility.Instance.GetRoleConfig(gender);
        if (config == 0)
        {
            GameLog.Error("没有默认的角色可以使用。");
            return;
        }
        saveData.Config = config;

        // 创建角色


        _saveDatas[saveData.RID] = saveData;
        WriteSaveData(saveData.RID);
    }

    /// <summary>
    /// 序列化角色信息
    /// </summary>
    /// <param name="root"></param>
    private bool SerializeRole(SaveData saveData)
    {
        if (saveData == null || saveData.roleData == null) return false;

        /*
         *      序列化角色属性以及表格
         */

        // 创建角色
        RoleObject roleObject = new RoleObject();

        // 初始化基础数据
        roleObject.Init(saveData.Config);
        roleObject.SetString("NickName", saveData.NickName);
        roleObject.SetInt("RID", saveData.RID);
        roleObject.SetLong("LastSave", DateTimeOffset.Now.ToUnixTimeSeconds());

        // 序列化角色
        roleObject.SerializeToXml(saveData.roleData);

        /*
         *      初始化基础表格信息
         */
        RecordManager initRecMgr = InitManager.Instance.InitRecMgr;
        if (initRecMgr != null)
        {
            // 获取表格名
            initRecMgr.GetRecordList(out List<string> records, true);
            foreach (string recName in records)
            {
                if (string.IsNullOrEmpty(recName)) continue;

                // 获取表格节点
                XmlNode recNode = saveData.roleData.SelectSingleNode("Role/Record/" + recName) as XmlElement;
                if (recNode == null) continue;

                // 获取表格结构
                Record rec = initRecMgr.GetRecord(recName);
                if (rec == null) continue;

                for (int row = 0; row < rec.MaxRowCount; ++row)
                {
                    if (!rec.CheckRowUse(row)) continue;

                    // 获取表格数据
                    DataList rowData = rec.GetRowData(row);
                    XmlElement rowNode = saveData.roleData.OwnerDocument.CreateElement("Row");

                    for (int index = 0; index < rowData.Count; ++index)
                    {
                        VariableData data = rowData.ReadVar(index);
                        // 获取tag
                        string tag = rec.GetTagByColumn(index);
                        if (string.IsNullOrEmpty(tag) || data == null) continue;

                        rowNode.SetAttribute(tag, data.ToString());
                    }

                    recNode.AppendChild(rowNode);
                }
            }
        }

        /*
         *      序列化角色背包
         */
        // 获取角色节点
        //XmlElement roleNode = root.SelectSingleNode("Role") as XmlElement;
        //if (roleNode == null) return false;
        //Inventory bag = new Inventory();
        //int bagID = DataUtility.Instance.GetViewPortConfig(eViewPort.Inventory);
        //bag.Init(bagID);

        //XmlElement bagNode = root.OwnerDocument.CreateElement(bag.GetString("Class"));
        //// 初始化背包属性
        //bag.SerializeProperty(bagNode);

        //foreach (var kv in QSInit.Instance.DefaultItems)
        //{
        //    // 道具id
        //    int itemID = kv.Key;
        //    // 道具数量
        //    int itemCount = kv.Value;
        //    if (itemCount <= 0)
        //    {
        //        GameLog.Error($"道具{itemID}数量为0，初始化角色添加背包道具失败");
        //        continue;
        //    }
        //    DataObject item = new DataObject();
        //    item.Init(itemID);
        //    item.SetInt("Count", itemCount);
        //    item.SerializeToXml(bagNode);
        //}

        //// 将背包格子添加到角色
        //roleNode.AppendChild(bagNode);

        /*
         *      装备视图
         */
        // todo

        /*
         *      创建技能视图/鼠标视图
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
            GameLog.Error($"写入存档失败！---> 当前存档数据【{RID}】为空。");
            return;
        }

        string saveFile = $"{GameConstant.STORE_SAVE_PATH}/{CurSaveData.NickName}_{CurSaveData.RID}.xml";

        // 创建文档结点
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
