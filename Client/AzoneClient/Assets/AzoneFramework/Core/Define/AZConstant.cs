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
    /// �������󻺴�����
    /// </summary>
    public const int kMaxPoolCacheCount = 10;

    /// <summary>
    /// ÿ֡����Mono�����������
    /// </summary>
    public const int kMaxLoadMonoCountPerFrame = 10;
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

    #endregion
}