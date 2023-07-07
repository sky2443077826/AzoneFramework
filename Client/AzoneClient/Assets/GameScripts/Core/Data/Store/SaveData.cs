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
    // ����
    public int Config { get; set; }

    // �ϴα���ʱ��
    public long LastSaveTime { get; set; }

    // ��ɫ����
    public XmlNode roleData { get; set; }

    // ��������
    public XmlNode worldData { get; set; }

    public SaveData() 
    {
        NickName = "����С��";
    }
}
