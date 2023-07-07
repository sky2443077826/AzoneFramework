/// <summary>
/// ��Ϸ��������
/// </summary>
public class AZConstant
{
    #region ����·��
    /// <summary>
    /// ��������
    /// </summary>
    public const string kDropConfig = "Logic/Drop.xml";
    /// <summary>
    /// ����������
    /// </summary>
    public const string kDropGroupConfig = "Logic/DropGroup.xml";
    /// <summary>
    /// ��������
    /// </summary>
    public const string kConditionCfg = "Logic/Condition.xml";

    /// <summary>
    /// ��ʼ������
    /// </summary>
    public const string kInitialDataConfig = "Logic/InitialData.xml";

    /// <summary>
    /// ���ػ�����
    /// </summary>
    public const string kLocalizationConfig = "Logic/Local/Localization.xml";

    /// <summary>
    /// Buff ����
    /// </summary>
    public const string kBuffConfig = "Logic/Battle/Buff.xml";

    /// <summary>
    /// TimeSequence ����
    /// </summary>
    public const string kTimeSequenceConfig = "Logic/Battle/TimeSequence.xml";

    /// <summary>
    /// Skill ����
    /// </summary>
    public const string kSkillConfig = "Logic/Battle/Skill.xml";

    /// <summary>
    /// ��ͼ���������ļ���
    /// </summary>
    public const string kBaseMapDataPath = "Logic/Map/";

    /// <summary>
    /// ��̬��ͼ�ļ�����
    /// </summary>
    public const string kStaticMapDataConfig = "Logic/Map/StaticMap.xml";

    /// <summary>
    /// ��̬��ͼֲ������
    /// </summary>
    public const string kStaticPlantDataConfig = "Logic/Map/StaticPlants.xml";

    /// <summary>
    /// ��̬��ͼ��Դ����
    /// </summary>
    public const string kStaticResourceDataConfig = "Logic/Map/StaticResource.xml";

    /// <summary>
    /// Weather ����
    /// </summary>
    public const string kWeatherConfig = "Logic/Weather/Weather.xml";

    /// <summary>
    /// Make ����
    /// </summary>
    public const string kMakeConfig = "Logic/Make/Make.xml";

    /// <summary>
    /// ��ͼ������������
    /// </summary>
    public const string kGeneratorConfig = "Logic/Map/Generator.xml";

    #endregion

    #region ������ֵ

    /// <summary>
    /// ��������
    /// </summary>
    public const int kBaseRate = 10000;

    /// <summary>
    /// ����һ�е�����
    /// </summary>
    public const int kInventoryMaxColumn = 12;

    /// <summary>
    /// ���������������
    /// </summary>
    public const int kInventoryMaxRow = 3;

    /// <summary>
    /// ��Ϸ��1���Ӷ�Ӧ��ʵ�ж�����
    /// ��Ϸ1���� -> 1s 
    /// ��Ϸ1���� -> 0.8s
    /// ��Ϸ1���� -> 0.6s
    /// NOTE: 
    /// 1GameDay = 1GameHour * kGameHourPerDay(24) = 24GameHour
    /// ��Ч��Ϸʱ�� = 1GameDay - 4GameHour(2��ʱ��) = 24GameHour - 4GameHour= 20GameHour
    /// 1s -> 1GameHour = 1s * 60 = 60s = 1min   GameDay:24min  ��Ч��Ϸʱ��:20min
    /// 0.8s -> 1GameHour = 0.8s * 60 = 0.48min   GameDay:19.2min  ��Ч��Ϸʱ��:16min
    /// 0.6s -> 1GameHour = 0.6s * 60 = 0.36min   GameDay:14.4min  ��Ч��Ϸʱ��:12min
    /// </summary>
    public const float kGameMin2RealSecond = 0.8f;

    /// <summary>
    /// ��Ϸ��һ��Сʱ���ٷ���
    /// </summary>
    public const int kGameMinPerHour = 60;

    /// <summary>
    /// ��Ϸʱ��һ����ٸ�Сʱ
    /// </summary>
    public const int kGameHourPerDay = 24;

    /// <summary>
    /// ��Ϸʱ��һ���¶�����
    /// </summary>
    public const int kGameDayPerMonth = 30;

    /// <summary>
    /// ��Ϸ��һ�꼸����
    /// </summary>
    public const int kGameMonthPerYear = 4;

    /// <summary>
    /// ��Ϸ��ʼСʱ îʱ 5 - 7
    /// </summary>
    public const int kStartGameHour = 5;

    /// <summary>
    /// 11����ʾ���
    /// </summary>
    public const int kAlertGameHour = 11;

    /// <summary>
    /// ǿ��˯��ʱ��
    /// </summary>
    public const int kForceSleepHour = 1;

    /// <summary>
    /// �������󻺴�����
    /// </summary>
    public const int kMaxPoolCacheCount = 10;

    /// <summary>
    /// ÿ֡����Mono�����������
    /// </summary>
    public const int kMaxLoadMonoCountPerFrame = 10;

    /// <summary>
    /// ���䷶Χ���ӣ�1��ʾ[��ɫ]��Χ1�񣬼��Ź���
    /// </summary>
    public const int kDropRange = 1;

    /// <summary>
    /// ������������ߵ���С���
    /// </summary>
    public const float kDropItemDistance = 0.3f;

    /// <summary>
    /// ���ɳ����ƽ׶Σ���Ҫ�ж��ܷ������ˡ�
    /// </summary>
    public const int kTreeLimitPhase = 2;

    /// <summary>
    /// �����ռ�ذ뾶
    /// </summary>
    public const int kMaxTreeRange = 1;

    /// <summary>
    /// �����������뾶
    /// </summary>
    public const int kMaxAdventureRange = 2;
    #endregion

    #region ���
    /// <summary>
    /// �ϳɱ��
    /// </summary>
    public const string kMakeRecName = "MakeRec";
    #endregion

    #region �ַ���
    /// <summary>
    /// ��xml�н����Ӷ����xpath·��
    /// </summary>
    public const string kParseChildXPath = "./*[not (name() = 'Record') and not (name() = 'Property')]";

    /// <summary>
    /// ������Դ��ַ
    /// </summary>
    public const string kSpriteAddressStr = "[{0}]";

    #endregion
}