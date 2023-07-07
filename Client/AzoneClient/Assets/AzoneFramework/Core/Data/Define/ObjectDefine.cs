/// <summary>
/// 对象类型
/// </summary>
public enum eObjectType
{
    None = 0,
    // 活动对象
    Role = 0x1,      // 角色 1
    Resident = 0x2,      // 居民 2
    Merchant = 0x3,      // 商人 3
    Mount = 0x4,      // 坐骑 4
    Pet = 0x5,      // 宠物 5
    Animal = 0x6,      // 动物 6
    Monster = 0x7,      // 怪物 7

    // 8-9
    // 不可活动对象                            
    Tool = 0xa,      // 工具 10
    Weapon = 0xb,      // 武器 11
    Blueprint = 0xc,      // 图纸 12
    Machine = 0xd,      // 设备 13
    Ornament = 0xe,      // 装饰物 14
    Equipment = 0xf,      // 装备 15
    Building = 0x10,     // 房屋 16

    Seed = 0x11,     // 种子 17
    Plant = 0x12,     // 植物  18
    Mineral = 0x13,     // 矿物质 19
    Resource = 0x14,     // 资源 20
    Container = 0x15,     // 容器 21

    // 不可视对象
    ViewPort = 0x20,      // 视图 32
    Skill = 0x21,      // 技能 33
}

/// <summary>
/// 视图类型
/// </summary>
public enum eViewPort
{
    None = 0,
    Inventory = 1,  // 背包

    Max,
}

/// <summary>
/// 性别
/// </summary>
public enum eGender
{
    None = 0,
    Male = 1,   // 男性
    Female = 2,   // 女性
}