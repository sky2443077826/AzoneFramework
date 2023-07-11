using AzoneFramework;
/// <summary>
/// ��Ϸ��������
/// </summary>
public static class GameConstant
{

    #region �߼�����·��
    /// <summary>
    /// ��ɫ��ʼ������
    /// </summary>
    public const string ROLE_INIT_DATA_CONFIG_PATH = "Logic/Role/Init.xml";


    /// <summary>
    /// ���ػ�����
    /// </summary>
    public const string kLocalizationConfig = "Logic/Local/Localization.xml";

    #endregion

    #region ������ֵ

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

    #region �ַ���
    /// <summary>
    /// ��xml�н����Ӷ����xpath·��
    /// </summary>
    public const string kParseChildXPath = "./*[not (name() = 'Record') and not (name() = 'Property')]";

    #endregion

    #region �洢���

    /// <summary>
    /// �浵·��
    /// </summary>
    public static readonly string STORE_SAVE_PATH = ApplicationPath.PersistentDataPath + "/Record";

    #endregion
}