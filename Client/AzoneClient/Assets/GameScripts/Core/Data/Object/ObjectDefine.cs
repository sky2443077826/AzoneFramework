/// <summary>
/// ��������
/// </summary>
public enum eObjectType
{
    None = 0,
    // �����
    Role = 0x1,      // ��ɫ 1
    Resident = 0x2,      // ���� 2
    Merchant = 0x3,      // ���� 3
    Mount = 0x4,      // ���� 4
    Pet = 0x5,      // ���� 5
    Animal = 0x6,      // ���� 6
    Monster = 0x7,      // ���� 7

    // 8-9
    // ���ɻ����                            
    Tool = 0xa,      // ���� 10
    Weapon = 0xb,      // ���� 11
    Blueprint = 0xc,      // ͼֽ 12
    Machine = 0xd,      // �豸 13
    Ornament = 0xe,      // װ���� 14
    Equipment = 0xf,      // װ�� 15
    Building = 0x10,     // ���� 16

    Seed = 0x11,     // ���� 17
    Plant = 0x12,     // ֲ��  18
    Mineral = 0x13,     // ������ 19
    Resource = 0x14,     // ��Դ 20
    Container = 0x15,     // ���� 21

    // �����Ӷ���
    ViewPort = 0x20,      // ��ͼ 32
    Skill = 0x21,      // ���� 33
}

/// <summary>
/// ��ͼ����
/// </summary>
public enum eViewPort
{
    None = 0,
    Inventory = 1,  // ����

    Max,
}

/// <summary>
/// �Ա�
/// </summary>
public enum eGender
{
    None = 0,
    Male = 1,   // ����
    Female = 2,   // Ů��
}