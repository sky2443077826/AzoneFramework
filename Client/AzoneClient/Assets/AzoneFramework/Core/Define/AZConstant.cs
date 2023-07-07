/// <summary>
/// 游戏常量定义
/// </summary>
public class AZConstant
{
    #region 配置路径
    /// <summary>
    /// 掉落配置
    /// </summary>
    public const string kDropConfig = "Logic/Drop.xml";
    /// <summary>
    /// 掉落组配置
    /// </summary>
    public const string kDropGroupConfig = "Logic/DropGroup.xml";
    /// <summary>
    /// 条件配置
    /// </summary>
    public const string kConditionCfg = "Logic/Condition.xml";

    /// <summary>
    /// 初始化数据
    /// </summary>
    public const string kInitialDataConfig = "Logic/InitialData.xml";

    /// <summary>
    /// 本地化配置
    /// </summary>
    public const string kLocalizationConfig = "Logic/Local/Localization.xml";

    /// <summary>
    /// Buff 配置
    /// </summary>
    public const string kBuffConfig = "Logic/Battle/Buff.xml";

    /// <summary>
    /// TimeSequence 配置
    /// </summary>
    public const string kTimeSequenceConfig = "Logic/Battle/TimeSequence.xml";

    /// <summary>
    /// Skill 配置
    /// </summary>
    public const string kSkillConfig = "Logic/Battle/Skill.xml";

    /// <summary>
    /// 地图基础数据文件夹
    /// </summary>
    public const string kBaseMapDataPath = "Logic/Map/";

    /// <summary>
    /// 静态地图文件配置
    /// </summary>
    public const string kStaticMapDataConfig = "Logic/Map/StaticMap.xml";

    /// <summary>
    /// 静态地图植物配置
    /// </summary>
    public const string kStaticPlantDataConfig = "Logic/Map/StaticPlants.xml";

    /// <summary>
    /// 静态地图资源配置
    /// </summary>
    public const string kStaticResourceDataConfig = "Logic/Map/StaticResource.xml";

    /// <summary>
    /// Weather 配置
    /// </summary>
    public const string kWeatherConfig = "Logic/Weather/Weather.xml";

    /// <summary>
    /// Make 配置
    /// </summary>
    public const string kMakeConfig = "Logic/Make/Make.xml";

    /// <summary>
    /// 地图对象生成配置
    /// </summary>
    public const string kGeneratorConfig = "Logic/Map/Generator.xml";

    #endregion

    #region 常量数值

    /// <summary>
    /// 基础概率
    /// </summary>
    public const int kBaseRate = 10000;

    /// <summary>
    /// 背包一行的数量
    /// </summary>
    public const int kInventoryMaxColumn = 12;

    /// <summary>
    /// 背包格子最大行数
    /// </summary>
    public const int kInventoryMaxRow = 3;

    /// <summary>
    /// 游戏中1分钟对应现实中多少秒
    /// 游戏1分钟 -> 1s 
    /// 游戏1分钟 -> 0.8s
    /// 游戏1分钟 -> 0.6s
    /// NOTE: 
    /// 1GameDay = 1GameHour * kGameHourPerDay(24) = 24GameHour
    /// 有效游戏时间 = 1GameDay - 4GameHour(2个时辰) = 24GameHour - 4GameHour= 20GameHour
    /// 1s -> 1GameHour = 1s * 60 = 60s = 1min   GameDay:24min  有效游戏时间:20min
    /// 0.8s -> 1GameHour = 0.8s * 60 = 0.48min   GameDay:19.2min  有效游戏时间:16min
    /// 0.6s -> 1GameHour = 0.6s * 60 = 0.36min   GameDay:14.4min  有效游戏时间:12min
    /// </summary>
    public const float kGameMin2RealSecond = 0.8f;

    /// <summary>
    /// 游戏内一个小时多少分钟
    /// </summary>
    public const int kGameMinPerHour = 60;

    /// <summary>
    /// 游戏时间一天多少个小时
    /// </summary>
    public const int kGameHourPerDay = 24;

    /// <summary>
    /// 游戏时间一个月多少天
    /// </summary>
    public const int kGameDayPerMonth = 30;

    /// <summary>
    /// 游戏中一年几个月
    /// </summary>
    public const int kGameMonthPerYear = 4;

    /// <summary>
    /// 游戏起始小时 卯时 5 - 7
    /// </summary>
    public const int kStartGameHour = 5;

    /// <summary>
    /// 11点提示玩家
    /// </summary>
    public const int kAlertGameHour = 11;

    /// <summary>
    /// 强制睡觉时间
    /// </summary>
    public const int kForceSleepHour = 1;

    /// <summary>
    /// 对象池最大缓存数量
    /// </summary>
    public const int kMaxPoolCacheCount = 10;

    /// <summary>
    /// 每帧加载Mono对象最大数量
    /// </summary>
    public const int kMaxLoadMonoCountPerFrame = 10;

    /// <summary>
    /// 掉落范围格子，1表示[角色]周围1格，即九宫格。
    /// </summary>
    public const int kDropRange = 1;

    /// <summary>
    /// 掉落的两个道具的最小间距
    /// </summary>
    public const float kDropItemDistance = 0.3f;

    /// <summary>
    /// 树成长限制阶段，需要判断能否生长了。
    /// </summary>
    public const int kTreeLimitPhase = 2;

    /// <summary>
    /// 树最大占地半径
    /// </summary>
    public const int kMaxTreeRange = 1;

    /// <summary>
    /// 最大奇遇掉落半径
    /// </summary>
    public const int kMaxAdventureRange = 2;
    #endregion

    #region 表格
    /// <summary>
    /// 合成表格
    /// </summary>
    public const string kMakeRecName = "MakeRec";
    #endregion

    #region 字符串
    /// <summary>
    /// 从xml中解析子对象的xpath路径
    /// </summary>
    public const string kParseChildXPath = "./*[not (name() = 'Record') and not (name() = 'Property')]";

    /// <summary>
    /// 精灵资源地址
    /// </summary>
    public const string kSpriteAddressStr = "[{0}]";

    #endregion
}