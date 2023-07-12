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
            if (!LoadSave(document))
            {
                GameLog.Error($"加载存档错误！--->存档【{fileName}】的Save结点数据不合法。");
                return;
            }
        }
    }

    /// <summary>
    /// 加载存档信息
    /// </summary>
    /// <param name="save"></param>
    /// <returns></returns>
    private bool LoadSave(XmlDocument document)
    {
        XmlNode save = document.DocumentElement.SelectSingleNode("Save");
        if (save == null)
        {
            GameLog.Error($"加载存档错误！--->存档【{document.Name}】的Save结点不存在。");
            return false;
        }

        SaveData saveData = new SaveData();

        XmlNode nickNameNode = save.SelectSingleNode("NickName");
        XmlNode RIDNode = save.SelectSingleNode("RID");
        XmlNode genderNode = save.SelectSingleNode("Gender");

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

        // 角色性别
        if (genderNode == null || genderNode.InnerText == null)
        {
            return false;
        }
        eGender gender = (eGender)ConvertUtility.IntConvert(genderNode.InnerText);
        saveData.Gender = gender;

        // 角色数据
        XmlElement roleNode = document.DocumentElement.SelectSingleNode("Role") as XmlElement;
        if (roleNode == null)
        {
            GameLog.Error($"加载存档错误！--->角色数据不存在{rID}。");
            return false;
        }

        saveData.document = document;
        _saveDatas.Add(rID, saveData);
        return true;
    }

    #endregion

    #region 解析存档

    /// <summary>
    /// 反序列化数据
    /// </summary>
    /// <param name="RID"></param>
    public void DeSerializeRoleFromData(int RID)
    {

    }

    #endregion

    #region 写入文件

    /// <summary>
    /// 创建新存档
    /// </summary>
    public bool CreateNewSave(string nickName, eGender gender)
    {
        if (string.IsNullOrEmpty(nickName))
        {
            GameLog.Error($"创建新存档失败！---> 角色名称【{nickName}】不合法。");
            return false;
        }

        SaveData saveData = new SaveData();
        saveData.NickName = nickName;
        saveData.RID = DataUtility.GenerateRID();
        saveData.Gender = gender;

        // 根据性别获取指定的配置ID
        int config = DataUtility.Instance.GetRoleConfig(gender);
        if (config == 0)
        {
            GameLog.Error("没有默认的角色可以使用。");
            return false;
        }

        // 创建文档结点
        XmlDocument xmlDocument = new XmlDocument();
        XmlDeclaration decNode = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDocument.AppendChild(decNode);
        // 创建根节点
        XmlElement rootNode = xmlDocument.CreateElement("XML");
        xmlDocument.AppendChild(rootNode);
        // 创建存档结点   
        XmlElement saveNode = xmlDocument.CreateElement("Save");
        rootNode.AppendChild(saveNode);
        // 创建昵称
        XmlElement nickNode = xmlDocument.CreateElement("NickName");
        nickNode.InnerText = saveData.NickName;
        saveNode.AppendChild(nickNode);
        // 创建角色ID
        XmlNode rIDNode = xmlDocument.CreateElement("RID");
        rIDNode.InnerText = XmlConvert.ToString(saveData.RID);
        saveNode.AppendChild(rIDNode);
        // 创建角色ID
        XmlNode genderNode = xmlDocument.CreateElement("Gender");
        genderNode.InnerText = XmlConvert.ToString((int)saveData.Gender);
        saveNode.AppendChild(genderNode);

        saveData.document = xmlDocument;

        // 创建角色
        if (!SerializeInitRole(saveData))
        {
            GameLog.Error($"创建新存档失败！---> 角色【{nickName}】数据序列化失败。");
            return false;
        }

        // 保存存档
        _saveDatas[saveData.RID] = saveData;
        // 设置当前存档
        CurSaveData = saveData;

        GameLog.Normal($"===创建新角色成功：【{saveData.NickName}】===");
        return WriteSaveData(saveData.RID);
    }

    /// <summary>
    /// 序列化初始角色信息
    /// </summary>
    /// <param name="root"></param>
    private bool SerializeInitRole(SaveData saveData)
    {
        if (saveData == null || saveData.document == null) return false;

        // 选择根节点
        XmlElement rootNode = saveData.document.SelectSingleNode("XML") as XmlElement;
        if (rootNode == null)
        {
            return false;
        }

        /*
         *      序列化角色属性以及表格
         */

        // 创建角色
        RoleObject roleObject = new RoleObject();

        // 初始化基础数据
        int config = DataUtility.Instance.GetRoleConfig(saveData.Gender);
        if (!ConfigManager.Instance.HasConfig(config))
        {
            return false;
        }


        roleObject.Init(config);
        roleObject.SetString("NickName", saveData.NickName);
        roleObject.SetInt("RID", saveData.RID);
        roleObject.SetInt("Gender", (int)saveData.Gender);
        roleObject.SetLong("LastSave", DateTimeOffset.Now.ToUnixTimeSeconds());

        // 序列化角色
        roleObject.SerializeToXml(rootNode);

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
                XmlNode recNode = rootNode.SelectSingleNode("Role/Record/" + recName) as XmlElement;
                if (recNode == null) continue;

                // 获取表格结构
                Record rec = initRecMgr.GetRecord(recName);
                if (rec == null) continue;

                for (int row = 0; row < rec.MaxRowCount; ++row)
                {
                    if (!rec.CheckRowUse(row)) continue;

                    // 获取表格数据
                    DataList rowData = rec.GetRowData(row);
                    XmlElement rowNode = rootNode.OwnerDocument.CreateElement("Row");

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

    /// <summary>
    /// 写入存档文件
    /// </summary>
    /// <param name="RID"></param>
    private bool WriteSaveData(int RID)
    {
        if (!_saveDatas.TryGetValue(RID, out SaveData saveData))
        {
            GameLog.Error($"写入存档失败！---> 当前存档数据【{RID}】为空。");
            return false;
        }

        if (saveData.document == null)
        {
            GameLog.Error($"写入存档失败！---> 当前存档数据【{RID}】序列化数据为空。");
            return false;
        }

        string saveFile = $"{GameConstant.STORE_SAVE_PATH}/{saveData.NickName}_{saveData.RID}.xml";
        // 写入文件
        byte[] data = new UTF8Encoding(false).GetBytes(saveData.document.OuterXml);
        File.WriteAllBytes(saveFile, data);

        return true;
    }

    #endregion
}
