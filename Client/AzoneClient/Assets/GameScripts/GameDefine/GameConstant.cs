using AzoneFramework;
/// <summary>
/// 游戏常量定义
/// </summary>
public static class GameConstant
{

    #region 逻辑配置路径
    /// <summary>
    /// 角色初始化数据
    /// </summary>
    public const string ROLE_INIT_DATA_CONFIG_PATH = "Logic/Role/Init.xml";


    /// <summary>
    /// 本地化配置
    /// </summary>
    public const string kLocalizationConfig = "Logic/Local/Localization.xml";

    #endregion

    #region 常量数值

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

    #region 字符串
    /// <summary>
    /// 从xml中解析子对象的xpath路径
    /// </summary>
    public const string kParseChildXPath = "./*[not (name() = 'Record') and not (name() = 'Property')]";

    #endregion

    #region 存储相关

    /// <summary>
    /// 存档路径
    /// </summary>
    public static readonly string STORE_SAVE_PATH = ApplicationPath.PersistentDataPath + "/Record";

    #endregion
}