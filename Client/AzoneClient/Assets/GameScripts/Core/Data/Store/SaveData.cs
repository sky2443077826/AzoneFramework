using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// 存档数据
/// </summary>
public class SaveData
{
    // 昵称
    public string NickName { get; set; }
    // RID
    public int RID { get; set; }
    // 配置
    public int Config { get; set; }

    // 上次保存时间
    public long LastSaveTime { get; set; }

    // 角色数据
    public XmlElement roleData { get; set; }

    // 世界数据
    public XmlElement worldData { get; set; }

    public SaveData() { }
}
