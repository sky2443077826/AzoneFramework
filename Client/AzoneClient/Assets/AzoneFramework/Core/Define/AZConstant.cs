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
    /// 对象池最大缓存数量
    /// </summary>
    public const int kMaxPoolCacheCount = 10;

    /// <summary>
    /// 每帧加载Mono对象最大数量
    /// </summary>
    public const int kMaxLoadMonoCountPerFrame = 10;
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

    #endregion
}