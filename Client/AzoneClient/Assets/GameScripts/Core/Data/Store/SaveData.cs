using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// �浵����
/// </summary>
public class SaveData
{
    // �ǳ�
    public string NickName { get; set; }
    // RID
    public int RID { get; set; }
    // �Ա�
    public eGender Gender { get; set; }

    //�浵Xml�ĵ�
    public XmlDocument document { get; set; }

    public SaveData() { }
}
