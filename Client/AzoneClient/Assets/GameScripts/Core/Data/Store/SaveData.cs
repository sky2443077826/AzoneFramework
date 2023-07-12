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
    // 性别
    public eGender Gender { get; set; }

    //存档Xml文档
    public XmlDocument document { get; set; }

    public SaveData() { }
}
